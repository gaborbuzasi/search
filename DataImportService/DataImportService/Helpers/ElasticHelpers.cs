using DataImportService.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataImportService.Helpers
{
    public class ElasticHelpers
    {
        private static readonly Uri Uri = new Uri("http://localhost:9200");
        private static readonly ConnectionSettings settings = new ConnectionSettings(Uri);
        private static readonly ElasticClient client = new ElasticClient(settings);
        private const string documentsIndex = "documents";

        public static void InsertDocument(string txt, string title, List<string> keywords)
        {
            if (client.IndexExists(title).Exists)
                client.DeleteIndex(title);

            var doc = new DocumentModel
            {
                Title = title,
                Body = txt,
                Keywords = keywords,
                SourceId = "1"
            };

            title = title.Replace(",", "").Replace("/", "").Replace("\\", "").Replace(" ", "").ToLower();

            var result = client.Index(doc, index => index.Index(title));

            if (result.Result != Result.Created)
            {
                Console.WriteLine($"Error with file - {doc.Title} - {result.OriginalException.Message}");
            }
        }
    }
}
