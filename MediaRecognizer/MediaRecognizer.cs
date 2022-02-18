using com.cyberinternauts.all.MediaRecognizer.MetaSources;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.cyberinternauts.all.MediaRecognizer
{
    class MediaRecognizer
    {

        public async Task RecognizeMedias(string path)
        {
            await (new Imdb()).LoadMetaDataAsync();
            return;
        }
    }
}
