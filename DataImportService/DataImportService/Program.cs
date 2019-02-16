using DataImportService.Helpers;
using DataImportService.Models;
using Google.Cloud.Storage.V1;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DataImportService
{
    class Program
    {
        static void Main(string[] args)
        {
            var storClient = StorageClient.Create();
            
            var files = storClient.ListObjects("bilfingerfiles", "newSourceFiles/");
            foreach (var file in files.Where(file => file.ContentType == "application/pdf"))
            {
                Console.WriteLine($"Processing {file.Name}");
                DetectionHelper.DetectPdfText(@"gs://bilfingerfiles",
                                               file.Name,
                                               "bilfingerfiles",
                                               "exp/");
            }

            foreach (var file in files.Where(file => file.Name.EndsWith(".txt")))
            {
                using (var stream = new MemoryStream())
                {
                    storClient.DownloadObject(file, stream);
                    var text = Encoding.UTF8.GetString(stream.ToArray());

                    var keyPhraseResult = TextExtraction.ProcessText(text);

                    if (keyPhraseResult.Documents.Any())
                    {
                        Console.WriteLine("Uploading extracted text to Azure Search...\r\n");
                        string fileNameOnly = System.IO.Path.GetFileName(file.Name);
                        string fileId = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(fileNameOnly));
                        ElasticHelpers.InsertDocument(text, file.Name, keyPhraseResult.Documents.SelectMany(x => x.KeyPhrases).ToList());
                    }

                    Console.WriteLine($"Finished uploading {file.Name}. ");
                }
            }

            foreach (var file in files.Where(file => file.Name.EndsWith(".jpg") || file.Name.EndsWith(".png")))
            {
                Console.WriteLine($"Processing {file.Name}");
                DetectionHelper.DetectImageText(@"gs://bilfingerfiles",
                                                file.Name,
                                                "bilfingerfiles",
                                                "exp/");
            }

            Console.ReadLine();
        }
    }
}
