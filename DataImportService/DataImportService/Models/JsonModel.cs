using System;
using System.Collections.Generic;
using System.Text;

namespace DataImportService.Models
{
    public class JsonModel
    {
        public string Folder { get; set; }
        public string Filename { get; set; }
        public IEnumerable<string> Text { get; set; }
    }
}
