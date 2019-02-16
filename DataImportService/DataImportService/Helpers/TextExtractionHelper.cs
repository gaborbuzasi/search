using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rest;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Google.Cloud.Vision.V1;
using System.Collections.Generic;

namespace DataImportService.Helpers
{
    class ApiKeyServiceClientCredentials : ServiceClientCredentials
    {
        private const string SubscriptionKey = "672719d149fd4a52a5d4304379cfa326";
        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }

    public class TextExtraction
    {
        public static KeyPhraseBatchResult ProcessText(AnnotateFileResponse response)
        {
            ITextAnalyticsClient client = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
            {
                Endpoint = "https://westeurope.api.cognitive.microsoft.com",
            };

            // Getting key-phrases
            Console.WriteLine("\n\n===== KEY-PHRASE EXTRACTION ======");

            if (response.Responses.Any(resp => resp.FullTextAnnotation != null))
            {
                var multiInputDocs = response.Responses.Select((resp, x) =>
                {
                    if (resp.FullTextAnnotation == null)
                        return null;

                    var maxConfidenceLang = resp.FullTextAnnotation.Pages.First().Property.DetectedLanguages.Max(lang => lang.Confidence);

                    var input = new MultiLanguageInput(resp.FullTextAnnotation.Pages.First()
                                                                        .Property.DetectedLanguages
                                                                        .First(lang => lang.Confidence == maxConfidenceLang)
                                                                        .LanguageCode,
                                                                        x.ToString(),
                                                                        resp.FullTextAnnotation.Text);

                    return input;
                }).ToList();

                var multiInput = new MultiLanguageBatchInput(multiInputDocs.Where(doc => doc != null).ToList());

                KeyPhraseBatchResult result = client.KeyPhrasesAsync(multiInput).Result;

                // Printing keyphrases
                foreach (var document in result.Documents)
                {
                    Console.WriteLine($"Document ID: {document.Id} ");

                    Console.WriteLine("\t Key phrases:");

                    foreach (string keyphrase in document.KeyPhrases)
                    {
                        Console.WriteLine($"\t\t{keyphrase}");
                    }
                }

                return result;
            }

            return null;
        }

        public static KeyPhraseBatchResult ProcessText(BatchAnnotateImagesResponse response)
        {
            ITextAnalyticsClient client = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
            {
                Endpoint = "https://westeurope.api.cognitive.microsoft.com",
            };

            // Getting key-phrases
            Console.WriteLine("\n\n===== KEY-PHRASE EXTRACTION ======");

            var multiInputDocs = response.Responses.Select((resp, x) =>
            {
                var maxConfidenceLang = resp.FullTextAnnotation.Pages.First().Property.DetectedLanguages.Max(lang => lang.Confidence);

                var input = new MultiLanguageInput(resp.FullTextAnnotation.Pages.First()
                                                                    .Property.DetectedLanguages
                                                                    .First(lang => lang.Confidence == maxConfidenceLang)
                                                                    .LanguageCode,
                                                                    x.ToString(),
                                                                    resp.FullTextAnnotation.Text);

                return input;
            }).ToList();


            var multiInput = new MultiLanguageBatchInput(multiInputDocs);

            KeyPhraseBatchResult result = client.KeyPhrasesAsync(multiInput).Result;

            // Printing keyphrases
            foreach (var document in result.Documents)
            {
                Console.WriteLine($"Document ID: {document.Id} ");

                Console.WriteLine("\t Key phrases:");

                foreach (string keyphrase in document.KeyPhrases)
                {
                    Console.WriteLine($"\t\t{keyphrase}");
                }
            }

            return result;
        }

        public static KeyPhraseBatchResult ProcessText(string text)
        {
            ITextAnalyticsClient client = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
            {
                Endpoint = "https://westeurope.api.cognitive.microsoft.com",
            };

            // Getting key-phrases
            Console.WriteLine("\n\n===== KEY-PHRASE EXTRACTION ======");

            var input = new List<MultiLanguageInput>
            {
                new MultiLanguageInput
                {
                     Id = Guid.NewGuid().ToString(),
                      Language = "de",
                       Text = text
                }
            };


            var multiInput = new MultiLanguageBatchInput(input);

            KeyPhraseBatchResult result = client.KeyPhrasesAsync(multiInput).Result;

            // Printing keyphrases
            foreach (var document in result.Documents)
            {
                Console.WriteLine($"Document ID: {document.Id} ");

                Console.WriteLine("\t Key phrases:");

                foreach (string keyphrase in document.KeyPhrases)
                {
                    Console.WriteLine($"\t\t{keyphrase}");
                }
            }

            return result;
        }
    }   
}
    