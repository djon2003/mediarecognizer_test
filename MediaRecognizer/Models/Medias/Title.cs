using com.cyberinternauts.all.MediaRecognizer.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.cyberinternauts.all.MediaRecognizer.Models.Medias
{
    public class Title : Model, ITitle
    {
        #region ITitle
        [Column("Title")]
        public string Text { get; set; }

        public string Language { get; set; }

        public string Region { get; set; }
        #endregion
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }
    }
}
