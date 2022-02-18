using com.cyberinternauts.all.MediaRecognizer.Database;
using com.cyberinternauts.all.MediaRecognizer.Models.Metas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace com.cyberinternauts.all.MediaRecognizer.MetaSources
{
    class Imdb : MediaSource
    {
        private const string TITLES_FILE = "title.basics.tsv.gz";
        private const string AKAS_FILE = "title.akas.tsv.gz";
        private readonly string temporaryFolder = @"c:\temp\";
        private readonly string baseUrl = "https://datasets.imdbws.com/";
        private readonly WebClient webClient = new();

        MediaRecognizerContext db = new();

        private IQueryable<MetaMovie> imdbMovies = null;

        private async Task<bool> GatherFilesAsync()
        {
            var totalFilesGathered = 0;
            var filesToDownload = new string[] { AKAS_FILE, TITLES_FILE };
            foreach(var fileToDownload in filesToDownload)
            {
                var compressedFile = temporaryFolder + fileToDownload;
                if (!File.Exists(compressedFile) || !File.GetLastWriteTime(compressedFile).Date.Equals(DateTime.Today))
                {
                    await GatherFileAsync(fileToDownload);
                    totalFilesGathered++;
                }
            }

            return totalFilesGathered != 0;
        }

        private async Task GatherFileAsync(string fileName)
        {
            var compressedFile = temporaryFolder + fileName;
            var uncompressedFile = temporaryFolder + Path.GetFileNameWithoutExtension(compressedFile);
            await webClient.DownloadFileTaskAsync(baseUrl + fileName, compressedFile);

            using Stream fd = File.Create(uncompressedFile);
            using Stream fs = File.OpenRead(compressedFile);
            using Stream csStream = new GZipStream(fs, CompressionMode.Decompress);
            var buffer = new byte[1024];
            int nRead;
            while ((nRead = await csStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fd.WriteAsync(buffer, 0, nRead);
            }
        }

        public async Task LoadMetaDataAsync()
        {
            //return; //TODO: Remove this line

            //TODO: Reactivate this line
            //if (!await GatherFilesAsync()) return;


            var titlesFile = temporaryFolder + Path.GetFileNameWithoutExtension(TITLES_FILE);
            var akasFile = temporaryFolder + Path.GetFileNameWithoutExtension(AKAS_FILE);
            var dbLock = new SemaphoreSlim(1);
            var akasLock = new SemaphoreSlim(1);
            var currentTitlesAkasLock = new SemaphoreSlim(1);
            var associateLock = new SemaphoreSlim(1);

            using (var db = new MediaRecognizerContext())
            {
                db.ChangeTracker.AutoDetectChangesEnabled = false;

                var titles = new ConcurrentDictionary<string, MetaMovie>();
                var readTitles = Task.Factory.StartNew(() =>
                {
                    Parallel.ForEach(File.ReadLines(titlesFile), (titleLine, _, readingIndex) =>
                    {
                        if (readingIndex == 0) return; // Skipping columns titles line

                        var movieInfos = titleLine.Split("\t", StringSplitOptions.None);
                        dbLock.Wait();
                        MetaMovie metaMovie = db.MetaMovies.Where(m => m.ExternalId == movieInfos[0]).Include(m => m.Titles).FirstOrDefault();
                        dbLock.Release();
                        if (metaMovie == null)
                        {
                            int totalMinutes = -1;
                            if (!int.TryParse(movieInfos[7], out totalMinutes))
                            {
                                totalMinutes = -1;
                            }
                            metaMovie = new MetaMovie
                            {
                                ExternalId = movieInfos[0],
                                MetaSource = nameof(Imdb),
                                MovieType = movieInfos[1],
                                Title = movieInfos[3],
                                TotalMinutes = totalMinutes,
                                Genres = movieInfos[8]
                            };
                            metaMovie.Titles = new List<MetaTitle>();
                            if (int.TryParse(movieInfos[5], out int startYear))
                            {
                                metaMovie.StartYear = new DateTime(startYear, 1, 1);
                            }
                            else
                            {
                                metaMovie.StartYear = new DateTime(9999, 1, 1);
                            }
                            if (int.TryParse(movieInfos[6], out int endYear))
                            {
                                metaMovie.EndYear = new DateTime(endYear, 1, 1);
                            }
                            else
                            {
                                metaMovie.EndYear = metaMovie.StartYear;
                            }
                        }

                        titles.TryAdd(metaMovie.ExternalId, metaMovie);
                    });
                });

                var akas = new Dictionary<string, List<MetaTitle>>();
                var currentTitlesAkas = new OrderedDictionary();
                var readAkas = Task.Factory.StartNew(() =>
                {
                    Parallel.ForEach(File.ReadLines(akasFile), (akaLine, _, readingIndex) =>
                    {
                        if (readingIndex == 0) return; // Skipping columns titles line

                        currentTitlesAkasLock.Wait();
                        var titleInfos = akaLine.Split("\t", StringSplitOptions.None);
                        var externalId = titleInfos[0];
                        if (!currentTitlesAkas.Contains(externalId))
                        {
                            currentTitlesAkas.Add(externalId, 1);
                        }
                        else
                        {
                            currentTitlesAkas[externalId] = ((int)currentTitlesAkas[externalId]) + 1;
                        }
                        currentTitlesAkasLock.Release();

                        var metaTitle = new MetaTitle
                        {
                            MetaMovie = null,
                            Text = titleInfos[2],
                            Region = titleInfos[3],
                            Language = titleInfos[4]
                        };

                        akasLock.Wait();
                        List<MetaTitle> titleAkas;
                        if (!akas.ContainsKey(externalId))
                        {
                            titleAkas = new List<MetaTitle>();
                            akas.Add(externalId, titleAkas);
                        }
                        else
                        {
                            titleAkas = akas[externalId];
                        }
                        titleAkas.Add(metaTitle);
                        akasLock.Release();

                        currentTitlesAkasLock.Wait();
                        currentTitlesAkas[externalId] = ((int)currentTitlesAkas[externalId]) - 1;
                        currentTitlesAkasLock.Release();
                    });
                });

                var savingCounter = 0;
                var associate = Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Parallel.For(1, Environment.ProcessorCount * 10, async (_) =>
                    {
                        var isAssociating = true;
                        do
                        {
                            var externalId = string.Empty;
                            currentTitlesAkasLock.Wait();
                            foreach (object curExternalId in currentTitlesAkas.Keys)
                            {
                                if ((int)currentTitlesAkas[curExternalId] == 0)
                                {
                                    externalId = (string)curExternalId;
                                    break;
                                }
                            }
                            if (externalId != String.Empty)
                            {
                                currentTitlesAkas.Remove(externalId); // Removing so other threads won't take it
                            }
                            isAssociating = !readAkas.IsCompleted || !readTitles.IsCompleted || currentTitlesAkas.Count != 0;
                            currentTitlesAkasLock.Release();

                            if (String.IsNullOrEmpty(externalId)) continue;

                            if (titles.TryGetValue(externalId, out MetaMovie metaMovie))
                            {
                                akasLock.Wait();
                                var titleAkas = akas[externalId];
                                akas.Remove(externalId);
                                akasLock.Release();

                                var changedMovie = false;
                                var movieAkas = metaMovie.Titles.Select(t => t).ToList(); // Clone list
                                foreach (var metaTitle in titleAkas)
                                {
                                    var existingTitle = movieAkas.Where(t => t.Text == metaTitle.Text && t.Region == metaTitle.Region && t.Language == metaTitle.Language).FirstOrDefault();
                                    if (existingTitle == null)
                                    {
                                        changedMovie = true;
                                        metaMovie.Titles.Add(metaTitle);
                                    }
                                    else
                                    {
                                        movieAkas.Remove(existingTitle);
                                    }
                                }
                                foreach (var movieTitle in movieAkas)
                                {
                                    changedMovie = true;
                                    metaMovie.Titles.Remove(movieTitle);
                                }

                                dbLock.Wait();
                                if (metaMovie.Id == 0)
                                {
                                    db.Add(metaMovie);
                                }
                                else if (changedMovie)
                                {
                                    db.Update(metaMovie);
                                }
                                dbLock.Release();

                                currentTitlesAkasLock.Wait();
                                currentTitlesAkas.Remove(externalId); // Free memory
                                isAssociating = !readAkas.IsCompleted || !readTitles.IsCompleted || currentTitlesAkas.Count != 0;
                                currentTitlesAkasLock.Release();

                                titles.TryRemove(externalId, out MetaMovie uselessOut2); // Free memory

                                associateLock.Wait();
                                savingCounter++;
                                var localSavingCounter = savingCounter;
                                associateLock.Release();

                                if (localSavingCounter != 0 && localSavingCounter % 1000 == 0)
                                {
                                    dbLock.Wait();
                                    await db.SaveChangesAsync();
                                    dbLock.Release();
                                    Console.WriteLine("Saved " + localSavingCounter);
                                }
                            }
                            else if (!readTitles.IsCompleted) // If reading titles is not ended, then maybe it was not read yet... otherwise, it doesn't exist
                            {
                                currentTitlesAkasLock.Wait();
                                currentTitlesAkas.Add(externalId, 0); // Readd because still no movie associated
                                currentTitlesAkasLock.Release();
                            }
                        } while (isAssociating);
                    });
                });

                Task.WaitAll(readTitles, readAkas, associate);
                await db.SaveChangesAsync();
            }
        }
    }
}
