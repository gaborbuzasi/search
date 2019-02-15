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
            //var storClient = StorageClient.Create();
            //
            //var files = storClient.ListObjects("bilfingerfiles", "sourceFiles/");
            //foreach (var file in files.Where(file => file.ContentType == "application/pdf"))
            //{
            //    Console.WriteLine($"Processing {file.Name}");
            //    DetectionHelper.DetectDocument(@"gs://bilfingerfiles",
            //                                   file.Name,
            //                                   "bilfingerfiles",
            //                                   "exp/");
            //}


            var file = new FileStream(@"C:\Users\Gabor\Downloads\Ex Zertikat_reduced.json", FileMode.Open);
            var sr = new StreamReader(file);

            var jsonObj = sr.ReadToEnd();
            var obj = JsonConvert.DeserializeObject<JsonModel>(jsonObj);

            ElasticHelpers.InsertDocument("test", obj.Filename.ToLower(), obj.Text.ToList());

        }
    }
}
