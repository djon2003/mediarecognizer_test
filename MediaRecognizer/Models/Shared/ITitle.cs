using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.cyberinternauts.all.MediaRecognizer.Models.Shared
{
    interface ITitle
    {
        public string Text { get; set; }

        public string Language { get; set; }

        public string Region { get; set; }
    }
}
