using com.cyberinternauts.all.MediaRecognizer.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.cyberinternauts.all.MediaRecognizer.Models.Metas
{
    public class MetaTitle : Model,  ITitle
    {
        #region ITitle
        [Column("Title")]
        public string Text { get; set; }

        public string Language { get; set; }

        public string Region { get; set; }
        #endregion

        public int MetaMovieId { get; set; }
        public virtual MetaMovie MetaMovie { get; set; }
    }
}
