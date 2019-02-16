using DataImportService.Helpers;
using DataImportService.Models;
using Google.Cloud.Storage.V1;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace DataImportService
{
    class Program
    {
        static void Main(string[] args)
        {
            var storClient = StorageClient.Create();
            
            var files = storClient.ListObjects("bilfingerfiles", "sourceFiles/");
            foreach (var file in files.Where(file => file.ContentType == "application/pdf"))
            {
                Console.WriteLine($"Processing {file.Name}");
                DetectionHelper.DetectPdfText(@"gs://bilfingerfiles",
                                               file.Name,
                                               "bilfingerfiles",
                                               "exp/");
            }

            foreach (var file in files.Where(file => file.Name.EndsWith(".jpg") || file.Name.EndsWith(".png")))
            {
                Console.WriteLine($"Processing {file.Name}");
                DetectionHelper.DetectImageText(@"gs://bilfingerfiles",
                                                file.Name,
                                                "bilfingerfiles",
                                                "exp/");
            }
        }
    }
}
