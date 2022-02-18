using com.cyberinternauts.all.MediaRecognizer.Models.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace com.cyberinternauts.all.MediaRecognizer.Models.Medias
{
    [Index(nameof(Title))]
    public class Movie : Media, IMovie
    {
        #region IMovie
        public string Title { get; set; }
        public DateTime? StartYear { get; set; }
        public DateTime? EndYear { get; set; }
        public int TotalMinutes { get; set; }
        public string Genres { get; set; }
        public string MovieType { get; set; }
        #endregion

        public virtual List<Title> Titles { get; set; }
    }
}
