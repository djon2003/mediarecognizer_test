using com.cyberinternauts.all.MediaRecognizer.Models.Medias;
using com.cyberinternauts.all.MediaRecognizer.Models.Metas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.cyberinternauts.all.MediaRecognizer.Database
{
    //Ref: https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=visual-studio
    //Ref: https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/complex-data-model?view=aspnetcore-6.0
    //Ref: https://docs.microsoft.com/en-us/ef/core/querying/
    class MediaRecognizerContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }

        [ForeignKey("Movie")]
        public DbSet<Title> Titles { get; set; }

        public DbSet<MetaMovie> MetaMovies { get; set; }

        [ForeignKey("MetaMovie")]
        public DbSet<MetaTitle> MetaTitles { get; set; }

        public string DbPath { get; }

        public MediaRecognizerContext()
        {
            //var folder = Environment.SpecialFolder.LocalApplicationData;
            //var path = Environment.GetFolderPath(folder);
            //DbPath = System.IO.Path.Join(path, "media_recognizer.db");
            DbPath = @"d:\temp\media_recognizer.db";
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseLazyLoadingProxies().UseSqlite($"Data Source={DbPath}");
    }
}
