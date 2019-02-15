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

            client.Index(doc, index => index.Index(string.Join("", title.Select(x => string.IsNullOrWhiteSpace(x.ToString()) ? string.Empty : x.ToString()))));
        }
    }
}
