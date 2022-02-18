using System;
using System.Threading.Tasks;

namespace com.cyberinternauts.all.MediaRecognizer
{
    class Program
    {
        public async static Task Main(string[] args)
        {
            Console.WriteLine("Beginning");

            var recognizer = new MediaRecognizer();
            await recognizer.RecognizeMedias("X:\\Films");
        }
    }
}
