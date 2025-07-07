using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using OpenAI;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Text;
using Azure.AI.TextAnalytics;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using OpenAI.Embeddings;
using DocumentSimilarityComparison.DTO;
using UglyToad.PdfPig;
using System.Net.Http.Headers;
using System.Globalization;
using Newtonsoft.Json.Linq;
using DocumentSimilarityComparison.Utility;
using DocumentSimilarityComparison.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentSimilarityComparison.AzureHelper
{
    public static class AzureAIClientService
    {
        private static readonly string connectionString = "Server=tcp:innovatorssqlserver.database.windows.net;Initial Catalog=Innovators;Persist Security Info=False;User ID=AdminInnovators;Password=Admin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private static readonly string azureEndpoint = "https://innovatorsopenairesource.openai.azure.com/";
        private static readonly string azureAPIKey = "81TfYFL0O1bkvbNzvWaoqrq63dJUDYS0OS3O9RUiOl08FUkE1g2TJQQJ99BFACYeBjFXJ3w3AAABACOGCq2N";
        private static readonly string azureLanguageAPIPath = "/language/:analyze-text?api-version=2023-04-01";
        private static readonly string azureAPIVersion = "2023-10-01-preview";
        private static readonly string deploymentName = "InnovatorsOpenAIResource";
        private static readonly string formRecognizerEndpoint = "https://jdextractfromdocumentresource.cognitiveservices.azure.com/";
        private static readonly string formRecognizerApiKey = "BxshjNY2iWnTp5vzlplUE9cxnpl8WZYoNATJmlySyPKolUztdD2sJQQJ99BFACYeBjFXJ3w3AAALACOGDyHK";

        public static async Task<(string resultJobDescriptionText, Job_Description_Model job_Description_Model)> GetJobDescription(string jobDescriptionPath, Job_Description_Model job_Description_Model)
        {
            List<string> jobDescriptionText = await ExtractTextFromResume(jobDescriptionPath, formRecognizerEndpoint, formRecognizerApiKey);
            string resultJobDescriptionText = String.Join(" ", jobDescriptionText);
            string jobDescriptionPrompt = $"Extract only the job description skills from the following text:\n\n{resultJobDescriptionText}\n\nReturn as a comma-separated list.";
            string jobDescriptionSkills = await GetTechnicalSkillsFromOpenAI(jobDescriptionPrompt);
            await GetJobDescriptiondetails(resultJobDescriptionText, job_Description_Model);
            await InsertJobDescriptionDetails(job_Description_Model);
            return (resultJobDescriptionText,job_Description_Model);
        }

        public static async Task GetComparisonScoreAsync(string resumePath, ResumeDTO resumedto, string jobDescriptionSkills, Job_Description_Model job_Description_Model)
        {
            string textAnalyticsEndpoint = "https://resumecomparelanguageservice.cognitiveservices.azure.com/";
            string textAnalyticsApikey = "CHGOd28xTqvETk5r6BCkmdL9XMZWFtJiKYoWaHALaL24Dt84jI8UJQQJ99BFACYeBjFXJ3w3AAAaACOGklLL";

            
            List<string> resumeText = await ExtractTextFromResume(resumePath, formRecognizerEndpoint, formRecognizerApiKey);
            string resultResumeText=String.Join(" ", resumeText);
            string resumeExtractPrompt = $"Extract only the technical skills from the following resume:\n\n{resultResumeText}\n\nReturn as a comma-separated list.";
            string ApplicantSkills = await GetTechnicalSkillsFromOpenAI(resumeExtractPrompt);
            await GetApplicantdetails(resultResumeText, resumedto);

            List<float> resumeEmbedding = await GetEmbeddingAsync(deploymentName, ApplicantSkills);
            List<float> jobDescriptionEmbedding = await GetEmbeddingAsync(deploymentName, jobDescriptionSkills);
            double embedScore = CosineSimilarity(jobDescriptionEmbedding, resumeEmbedding);
            (double gptScore, string summary, List<string> missing) =await GetMatchingScore(ApplicantSkills, jobDescriptionSkills);
            resumedto.ProfileScore= (gptScore * 0.7) + (embedScore * 0.3);
            resumedto.Summary= summary;
            string[] arrayResult = resultResumeText.Split(',');
            List<string> resultList = new List<string>(arrayResult);

            ResumeAnalyzeHelper analyzeHelper = new ResumeAnalyzeHelper();
            Resume_Details_Model resume_Details = new Resume_Details_Model();
            resumedto.SkillsText = ApplicantSkills;
            analyzeHelper.PopulateResumeDTO(resume_Details, resumedto,job_Description_Model);
            var resumeResult = await InsertResumeDetails(resume_Details);
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

        private static double CosineSimilarity(List<float> a, List<float> b)
        {
            if (a.Count != b.Count) return 0;
            double dot = 0, magA = 0, magB = 0;

            for (int i = 0; i < a.Count; i++)
            {
                dot += a[i] * b[i];
                magA += a[i] * a[i];
                magB += b[i] * b[i];
            }

            return (magA == 0 || magB == 0) ? 0 : dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
        }

        static async Task<List<string>> ExtractTextFromResume(string filePath, string endpoint, string apikey)
        {
            var client = new DocumentAnalysisClient(new Uri(endpoint), new AzureKeyCredential(apikey));
            using var stream = File.OpenRead(filePath);
            AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-document", stream);
            var result = operation.Value;
            var skills = new List<string>();
            foreach (var pages in result.Pages)
            {
                foreach (var line in pages.Lines)
                {
                    skills.Add(line.Content);

                }
            }
            return skills;

        }

        static List<string> ExtractJobDescriptionSkills(string jobDescription, string endpoint, string apikey)
        {
            var client = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(apikey));
            var extractedSkills = ExtractKeySkillsUsingNER(jobDescription, client);
            return extractedSkills;
        }

        static double CalculateTextSimilarity(string text1, string text2, TextAnalyticsClient client)
        {
            var keyPhrase1 = client.ExtractKeyPhrases(text1).Value;
            var keyPhrase2 = client.ExtractKeyPhrases(text2).Value;

            var commonPhrases = keyPhrase1.Intersect(keyPhrase2).Count();
            var totalPhrases = keyPhrase1.Union(keyPhrase2).Count();

            return (double)commonPhrases / totalPhrases * 100;
        }

        static double CompareSkills(List<string> text1, List<string> text2)
        {
            var matchingSkills = text2.Intersect(text1, StringComparer.OrdinalIgnoreCase).ToList();

            return (double)matchingSkills.Count / text2.Count * 100;
        }

        static List<string> ExtractKeySkillsUsingNER(string jobDescription, TextAnalyticsClient client)///NER=>Named Entity Recognition
        {
            var response = client.RecognizeEntities(jobDescription);

            var extractedSkills = new List<string>();
            foreach (var entity in response.Value)
            {
                if (entity.Category == "Skill")
                {
                    extractedSkills.Add(entity.Text);
                }
            }

            return extractedSkills;
        }

        private static async Task<Resume_Details_Model> InsertResumeDetails(Resume_Details_Model resume_Details)
        {
            var options = new DbContextOptionsBuilder<DocumentDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            using var context = new DocumentDbContext(options);
            var docSimilarity = new DocSimilarity(context);
            var resumeResult = await docSimilarity.CreateResumeAsync(resume_Details);
            return resumeResult;
        }

        private static async Task<Job_Description_Model> InsertJobDescriptionDetails(Job_Description_Model job_Description_Model)
        {
            var options = new DbContextOptionsBuilder<DocumentDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            using var context = new DocumentDbContext(options);
            var docSimilarity = new DocSimilarity(context);
            var resumeResult = await docSimilarity.CreateResumeAsync(job_Description_Model);
            return resumeResult;
        }

        public static async Task<Requestor_Model> InsertRequestorDetails(Requestor_Model requestor_Model)
        {
            var options = new DbContextOptionsBuilder<DocumentDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            using var context = new DocumentDbContext(options);
            var docSimilarity = new DocSimilarity(context);
            var resumeResult = await docSimilarity.CreateResumeAsync(requestor_Model);
            return resumeResult;
        }


        #region using Azure open AI to get the text from pdf

        static async Task<string> GetTechnicalSkillsFromOpenAI(string prompt)
        {
            //string endpoint = "https://innovatorsopenairesource.openai.azure.com/";
            string endpoint = "https://innovatorsopenairesource.openai.azure.com/openai/deployments/gpt-4.1/chat/completions?api-version=2025-01-01-preview";
            string apiKey = "81TfYFL0O1bkvbNzvWaoqrq63dJUDYS0OS3O9RUiOl08FUkE1g2TJQQJ99BFACYeBjFXJ3w3AAABACOGCq2N";
            string deploymentName = "gpt-4.1"; // or your model deployment
            string apiVersion = "api-version=2025-01-01-preview"; // Or latest supported version
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.BaseAddress = new Uri(endpoint);

            var requestBody = new
            {
                messages = new[]
                {
                new { role = "system", content = "You are a helpful assistant that extracts technical skills from resumes." },
                new { role = "user", content = prompt }
            },
                temperature = 0.3,
                max_tokens = 8192,
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject(responseString);
            string commaSeparatedResult = result.choices[0].message.content;
            string[] arrayResult = commaSeparatedResult.Split(',');
            List<string> resultList = new List<string>(arrayResult);
            return commaSeparatedResult;
        }

        public static async Task GetJobDescriptiondetails(string jobDescriptionDetails, Job_Description_Model JdDetails)
        {
            string endpoint = "https://innovatorsopenairesource.openai.azure.com/openai/deployments/gpt-4.1/chat/completions?api-version=2025-01-01-preview";
            string apiKey = "81TfYFL0O1bkvbNzvWaoqrq63dJUDYS0OS3O9RUiOl08FUkE1g2TJQQJ99BFACYeBjFXJ3w3AAABACOGCq2N";
            string deploymentName = "gpt-4.1"; // or your model deployment
            string apiVersion = "api-version=2025-01-01-preview"; // Or latest supported version

            using var client = new HttpClient();
            client.BaseAddress = new Uri(endpoint);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var prompt = $@"
                Extract the following from this jobdescription text:
                - JobTitle
                - JobDescription

                Resume:
                {jobDescriptionDetails}

                Return the result only as JSON:
                {{ ""JobTitle"": ""..."", ""JobDescription"": ""..."" }}
                Make sure to escape any double quotes or slash or special character inside the values and return a valid json.
                Give response with key job description not more than 100 words.
                ";

            var payload = new
            {
                messages = new[]
                {
            new { role = "system", content = "You are a resume parser." },
            new { role = "user", content = prompt }
        },
                temperature = 0,
                max_tokens = 8192
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            string reply = json["choices"]?[0]?["message"]?["content"]?.ToString() ?? "{}";

            try
            {
                var data = JObject.Parse(reply);
                JdDetails.JdTitle = data["JobTitle"]?.ToString()?.Trim();
                JdDetails.Description = data["JobDescription"]?.ToString()?.Trim();
            }
            catch
            {
                Console.WriteLine("⚠️ GPT response parse failed. Raw: " + reply);
            }
        }

        public static async Task GetApplicantdetails(string resumeDetails,ResumeDTO applicant)
        {
            string endpoint = "https://innovatorsopenairesource.openai.azure.com/openai/deployments/gpt-4.1/chat/completions?api-version=2025-01-01-preview";
            string apiKey = "81TfYFL0O1bkvbNzvWaoqrq63dJUDYS0OS3O9RUiOl08FUkE1g2TJQQJ99BFACYeBjFXJ3w3AAABACOGCq2N";
            string deploymentName = "gpt-4.1"; // or your model deployment
            string apiVersion = "api-version=2025-01-01-preview"; // Or latest supported version

            using var client = new HttpClient();
            client.BaseAddress = new Uri(endpoint);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var prompt = $@"
                Extract the following from this resume text:
                - Full name
                - Email address
                - Years of professional experience (integer only)

                Resume:
                {resumeDetails}

                Return the result as JSON:
                {{ ""name"": ""..."", ""email"": ""..."", ""experience"": ... }}
                ";

            var payload = new
            {
                messages = new[]
                {
            new { role = "system", content = "You are a resume parser." },
            new { role = "user", content = prompt }
        },
                temperature = 0,
                max_tokens = 8192
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            string reply = json["choices"]?[0]?["message"]?["content"]?.ToString() ?? "{}";

            try
            {
                var data = JObject.Parse(reply);
                applicant.ApplicantName = data["name"]?.ToString()?.Trim() ?? applicant.ApplicantName;
                applicant.ApplicantEmailId = data["email"]?.ToString()?.Trim();
                applicant.Experience = int.TryParse(data["experience"]?.ToString(), out var years) ? years : 0;
            }
            catch
            {
                Console.WriteLine("⚠️ GPT response parse failed. Raw: " + reply);
            }
        }

        public static async Task<(double score, string summary, List<string> missingSkills)> GetMatchingScore(string resumeText, string jobDescription)
        {
            string endpoint = "https://innovatorsopenairesource.openai.azure.com/openai/deployments/gpt-4.1/chat/completions?api-version=2025-01-01-preview";
            string apiKey = "81TfYFL0O1bkvbNzvWaoqrq63dJUDYS0OS3O9RUiOl08FUkE1g2TJQQJ99BFACYeBjFXJ3w3AAABACOGCq2N";
            string deploymentName = "gpt-4.1"; // or your model deployment
            string apiVersion = "api-version=2025-01-01-preview"; // Or latest supported version
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://innovatorsopenairesource.openai.azure.com/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            string prompt = $@"
                    You are a resume screening assistant.

                    Job Description:
                    {jobDescription}

                    Resume:
                    {resumeText}

                    Evaluate the fit based on skills and experience. Return JSON:
                    {{
                      ""score"": 0.0-1.0,
                      ""summary"": ""One sentence why"",
                      ""missing_skills"": [""skill1"", ""skill2""]
                    }}";

            var payload = new
            {
                messages = new[]
                {
            new { role = "system", content = "You are a helpful job-matching assistant." },
            new { role = "user", content = prompt }
        },
                temperature = 0.3,
                max_tokens = 8192
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);

            var resultJson = await response.Content.ReadAsStringAsync();

            try
            {
                var parsed = JObject.Parse(resultJson);
                var reply = parsed["choices"]?[0]?["message"]?["content"]?.ToString();
                var json = JObject.Parse(reply);

                double score = double.TryParse(json["score"]?.ToString(), out double s) ? s : 0;
                string summary = json["summary"]?.ToString() ?? "";
                var missingSkills = json["missing_skills"]?.ToObject<List<string>>() ?? new List<string>();

                return (score, summary, missingSkills);
            }
            catch
            {
                Console.WriteLine("⚠️ Failed to parse GPT response: " + resultJson);
                return (0, "Parsing error", new List<string>());
            }
        }


        #endregion
    }

}

