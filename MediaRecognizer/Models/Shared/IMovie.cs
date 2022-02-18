using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.cyberinternauts.all.MediaRecognizer.Models.Shared
{
    interface IMovie
    {
        public string Title { get; set; }

        public DateTime? StartYear { get; set; }

        public DateTime? EndYear { get; set; }

        public int TotalMinutes { get; set; }

        public string Genres { get; set; }

        public string MovieType { get; set; }
    }
}
