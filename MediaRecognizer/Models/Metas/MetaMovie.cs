using com.cyberinternauts.all.MediaRecognizer.Models.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.cyberinternauts.all.MediaRecognizer.Models.Metas
{
    [Index(nameof(ExternalId), nameof(MetaSource), IsUnique = true), Index(nameof(Title)), Index(nameof(TotalMinutes)), Index(nameof(MetaSource))]
    public class MetaMovie : Model, IMovie
    {
        #region IMovie
        public string Title { get; set; }
        public DateTime? StartYear { get; set; }
        public DateTime? EndYear { get; set; }
        public int TotalMinutes { get; set; }
        public string Genres { get; set; }
        public string MovieType { get; set; }
        #endregion

        [NotMapped]
        public DateTime MaximumDate { get; set; }

        public string MetaSource { get; set; }

        public string ExternalId { get; set; }

        public virtual List<MetaTitle> Titles { get; set; }
    }
}
