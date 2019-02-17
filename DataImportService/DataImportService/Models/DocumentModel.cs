using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataImportService.Models
{
    public class DocumentModel
    {
        public string Url { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        [JsonProperty(PropertyName = "meta_description")]
        public string MetaDescription { get; set; }

        public IList<string> Keywords { get; set; }

        public string Category { get; set; }

        [Date(Name = "last_updated_date")]
        public DateTimeOffset LastUpdatedDate { get; set; }

        [JsonProperty(PropertyName = "source_id")]
        public string SourceId { get; set; }
    }
}
