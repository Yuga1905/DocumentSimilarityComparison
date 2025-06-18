using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using OpenAI;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Text;
using Azure.AI.TextAnalytics;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using OpenAI.Embeddings;

namespace DocumentSimilarityComparison.AzureHelper
{
    public static class AzureAIClientService
    {
        private static readonly string azureEndpoint = "https://innovatorsopenairesource.openai.azure.com/";
        private static readonly string azureAPIKey = "81TfYFL0O1bkvbNzvWaoqrq63dJUDYS0OS3O9RUiOl08FUkE1g2TJQQJ99BFACYeBjFXJ3w3AAABACOGCq2N";
        private static readonly string azureLanguageAPIPath = "/language/:analyze-text?api-version=2023-04-01";
        private static readonly string azureAPIVersion = "2023-10-01-preview";
        private static readonly string deploymentName = "InnovatorsOpenAIResource";

        public static async Task<double> GetComparisonScoreAsync(string text1, string text2)
        {
            var client = new OpenAIClient(new AzureKeyCredential(azureAPIKey));
            List<float> resumeEmbedding = await GetEmbeddingAsync(deploymentName, text1);
            List<float> jobDescriptionEmbedding = await GetEmbeddingAsync(deploymentName, text2);
            double score= CosineSimilarity(jobDescriptionEmbedding, resumeEmbedding);
            //using (HttpClient client = new HttpClient())
            //{
            //    var requestUrl = $"{azureEndpoint}language/:analyze-text?api-version={azureAPIVersion}";
            //    //client.BaseAddress = new Uri(azureEndpoint);
            //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", azureAPIKey);



            //    //var requestBody = new
            //    //{
            //    //    kind = "Embedding",
            //    //    parameters = new { modelVersion = "latest" },
            //    //    analysisInput = new
            //    //    {
            //    //        documents = new[]
            //    //    {
            //    //        new {id="1",language = "en", text=text1},
            //    //        //new {id="2",language = "en", text=text2}
            //    //    }
            //    //    }

            //    //};

            //    string jsonBody = JsonConvert.SerializeObject(requestBody);
            //    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            //    //HttpResponseMessage response = await client.PostAsync(requestUrl, content);
            //    string responseBody = await response.Content.ReadAsStringAsync();
            //    //HttpResponseMessage response=await client.PostAsync(azureLanguageAPIPath, content);
            //    response.EnsureSuccessStatusCode();

            //    //string responseBody = await response.Content.ReadAsStringAsync();
            //    dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

            //    double similarityScore = jsonResponse.document[0].similarityScore;
            //    return similarityScore;
            //}

            //            var azureendpoint = new Uri("https://resumecomparelanguageservice.cognitiveservices.azure.com/");
            //            var client = new TextAnalyticsClient(azureendpoint, new AzureKeyCredential(azureAPIKey));

            //            string jobDescription = "Looking for a backend developer with .NET and Azure experience.";
            //            List<string> resumes = new List<string>
            //{
            //    "I have 5 years experience in C# and Azure Functions.",
            //    "Expert in Java and cloud computing."
            //};

            //            var jobKeyPhrases = client.ExtractKeyPhrases(jobDescription);
            //            Console.WriteLine("Job Description Key Phrases:");
            //            foreach (var phrase in jobKeyPhrases.Value)
            //                Console.WriteLine($"- {phrase}");

            //            foreach (var resume in resumes)
            //            {
            //                var resumeKeyPhrases = client.ExtractKeyPhrases(resume);
            //                Console.WriteLine("\nResume Key Phrases:");
            //                foreach (var phrase in resumeKeyPhrases.Value)
            //                    Console.WriteLine($"- {phrase}");

            //                // Simple overlap comparison
            //                var overlap = resumeKeyPhrases.Value.Intersect(jobKeyPhrases.Value);
            //                //Console.WriteLine($"Overlap count: {overlap.Count()}");

            //            }
            return score;
        }

        static async Task<List<float>> GetEmbeddingAsync(string deploymentName, string text)
        {
            var requrl = "https://innovatorsopenairesource.openai.azure.com/openai/deployments/text-embedding-ada-002/embeddings?api-version=2023-05-15";
            var requestUrl = $"{azureEndpoint}/openai/deployments/{deploymentName}/embeddings?api-version={azureAPIVersion}";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", azureAPIKey);
            client.BaseAddress = new Uri(azureEndpoint);

            var requestBody = new
            {
                input = text
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);
            StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(requrl, content);
            string responseBody = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(responseBody);

            return json.data[0].embedding.ToObject<List<float>>();
        }

        private static double CosineSimilarity(List<float> v1, List<float> v2)
        {
            if (v1.Count != v2.Count)
                throw new ArgumentException("Vectors must be of same length");

            double dot = 0.0, normA = 0.0, normB = 0.0;
            for (int i = 0; i < v1.Count; i++)
            {
                dot += v1[i] * v2[i];
                normA += Math.Pow(v1[i], 2);
                normB += Math.Pow(v2[i], 2);
            }
            return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }
    }
}
