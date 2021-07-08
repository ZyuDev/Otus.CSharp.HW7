using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace HW7.App
{
    class Program
    {
        private static bool _flagDone;

        static void Main(string[] args)
        {
            var folderName = "Documents";
            var waintingInterval = 10000;

            var folderPath = Path.Combine(Environment.CurrentDirectory, folderName);

            Directory.CreateDirectory(folderPath);

            var fileNames = new List<string>()
            {
                "Паспорт.jpg",
                "Заявление.txt", 
                "Фото.jpg"
            };


            using(var docsReceiver = new DocumentsReceiver(folderPath, waintingInterval, fileNames))
            {
                docsReceiver.DocumentsReady += OnReceiverDocumentsReady;
                docsReceiver.TimedOut += OnReceiverTimedOut;
                Console.Write("Waiting for documents");

                docsReceiver.Start();


                while (!_flagDone)
                {
                    Console.Write(".");
                    Thread.Sleep(100);
                }
            }

        }

        private static void OnReceiverDocumentsReady(object sender, EventArgs e)
        {

            if (!_flagDone)
            {
                Console.WriteLine("");
                Console.WriteLine("Documents ready.");
            }

            _flagDone = true;
        }

        private static void OnReceiverTimedOut(object sender, EventArgs e)
        {
            Console.WriteLine("");
            Console.WriteLine("Documents not found. Waiting stopped.");

            _flagDone = true;
        }
    }
}
