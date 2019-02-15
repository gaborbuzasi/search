using Google.Cloud.Storage.V1;
using Google.Cloud.Vision.V1;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataImportService.Helpers
{
    class DetectionHelper
    {
        public static void DetectDocument(string gcsSourceBucketUri,
                                           string fileName,
                                           string gcsDestinationBucketName,
                                           string gcsDestinationPrefixName)
        {
            var client = ImageAnnotatorClient.Create();

            var asyncRequest = new AsyncAnnotateFileRequest
            {
                InputConfig = new InputConfig
                {
                    GcsSource = new GcsSource
                    {
                        Uri = gcsSourceBucketUri + "/" + fileName
                    },
                    // Supported mime_types are: 'application/pdf' and 'image/tiff'
                    MimeType = "application/pdf"
                },
                OutputConfig = new OutputConfig
                {
                    // How many pages should be grouped into each json output file.
                    BatchSize = 10,
                    GcsDestination = new GcsDestination()
                    {
                        Uri = $"gs://{gcsDestinationBucketName}/{fileName}.json"
                    }
                }
            };

            asyncRequest.Features.Add(new Feature
            {
                Type = Feature.Types.Type.DocumentTextDetection
            });

            List<AsyncAnnotateFileRequest> requests =
                new List<AsyncAnnotateFileRequest>();
            requests.Add(asyncRequest);

            var operation = client.AsyncBatchAnnotateFiles(requests);

            Console.WriteLine("Waiting for the operation to finish");

            operation.PollUntilCompleted();

            // Once the rquest has completed and the output has been
            // written to GCS, we can list all the output files.
            var storageClient = StorageClient.Create();

            // List objects with the given prefix.
            var blobList = storageClient.ListObjects(gcsDestinationBucketName);

            // Process the first output file from GCS.
            // Select the first JSON file from the objects in the list.
            var output = blobList.First(x => x.Name.Contains(fileName) && x.Name.EndsWith(".json"));

            var jsonString = "";
            using (var stream = new MemoryStream())
            {
                storageClient.DownloadObject(output, stream);
                jsonString = Encoding.UTF8.GetString(stream.ToArray());
            }

            var response = JsonParser.Default.Parse<AnnotateFileResponse>(jsonString);
            var recognisedText = string.Join("\n", response.Responses.Select(resp => resp.FullTextAnnotation.Text));

            var keyPhraseResult = TextExtraction.ProcessText(response);


            if (response.Responses.Any())
            {
                Console.WriteLine("Uploading extracted text to Azure Search...\r\n");
                string fileNameOnly = System.IO.Path.GetFileName(fileName);
                string fileId = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(fileNameOnly));
                ElasticHelpers.InsertDocument(recognisedText, fileName, keyPhraseResult.Documents.SelectMany(x => x.KeyPhrases).ToList());
            }

            Console.WriteLine($"Finished uploading {fileName}. ");
        }
    }
}
