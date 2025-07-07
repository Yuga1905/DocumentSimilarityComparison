using DocumentSimilarityComparison.AgentHelper;

using DocumentSimilarityComparison.AzureHelper;

using DocumentSimilarityComparison.DTO;
using DocumentSimilarityComparison.HubHelper;
using DocumentSimilarityComparison.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel;

using System.IO;


namespace DocumentSimilarityComparison.Controllers

{

    [Route("api/[controller]")]

    [ApiController]

    public class DocSimilarityComparisonController : ControllerBase

    {

        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<ResumeHub> _hubContext;

        public DocSimilarityComparisonController(IWebHostEnvironment env,IHubContext<ResumeHub> hubContext)

        {
            _hubContext = hubContext;
            _env = env;

        }

        // GET: api/<DocSimilarityComparisonController>

        [HttpGet]

        //public IEnumerable<string> Get()

        //public async Task<List<ResumeDTO>> Get()

        //{

        //}

        // GET api/<DocSimilarityComparisonController>/5

        [HttpGet("{id}")]

        public string Get(int id)

        {

            return "value";

        }

        // POST api/<DocSimilarityComparisonController>

        [HttpPost]

        public async Task<IActionResult> Post(IFormFile file, string RequestorEmailId)

        {

            if (file == null || file.Length == 0)

                return BadRequest("No file uploaded.");

            // 1. Save JD file to Resources/JobDescription

            string jobDescriptionDir = Path.Combine(_env.ContentRootPath, "Resources", "JobDescription");

            if (!Directory.Exists(jobDescriptionDir))

                Directory.CreateDirectory(jobDescriptionDir);

            string jobDescriptionPath = Path.Combine(jobDescriptionDir, file.FileName);

            using (var stream = new FileStream(jobDescriptionPath, FileMode.Create))

            {
                await file.CopyToAsync(stream);
            }

            // 2. Get Resume folder path (Resources/JobApplicantsResume)

            string resumesFolderPath = Path.Combine(_env.ContentRootPath, "Resources", "JobApplicantsResume");

            // 3. Pass both paths to ComparisonAgent
            #region Testing SignalR

            JobDescriptionDTO compareAllResumes = new JobDescriptionDTO();

            string[] pdfFiles = Directory.GetFiles(resumesFolderPath, "*.pdf");

            Job_Description_Model job_Description_Model = new Job_Description_Model();
            (string resultJobDescriptionText, job_Description_Model) =
                await AzureHelper.AzureAIClientService.GetJobDescription(jobDescriptionPath, job_Description_Model);
            await _hubContext.Clients.All.SendAsync("JobDescriptionUploaded");
            foreach (string pdfPath in pdfFiles)
            {
                ResumeDTO resumeDTO = new ResumeDTO
                {
                    PdfPath = pdfPath
                };
                await AzureHelper.AzureAIClientService.GetComparisonScoreAsync(pdfPath, resumeDTO, resultJobDescriptionText, job_Description_Model);
                compareAllResumes.Resumes.Add(resumeDTO);
                await _hubContext.Clients.All.SendAsync("ResumeUpdated");
                await Task.Delay(1000);
            }

            
            #endregion

            JobDescriptionDTO matchedResumes = await ComparisonAgent.MatchResumesWithJobDescription(jobDescriptionPath, resumesFolderPath);

            JobDescriptionDTO rankedResumes = await RankingAgent.RankResumesWithScore(matchedResumes);

            string communicationSent = await CommunicationAgent.SendEmailWithRank(rankedResumes, RequestorEmailId);

            var requestor_Model = new Requestor_Model

            {

                ComparisonStatus = "Comparison Completed",

                CommunicationStatus = communicationSent,

            };

            if (matchedResumes.JdID > 0)

            {

                requestor_Model.JdId = matchedResumes.JdID;

                await AzureAIClientService.InsertRequestorDetails(requestor_Model);

            }

            // 4. Delete uploaded JD file (optional)

            if (System.IO.File.Exists(jobDescriptionPath))

            {

                System.IO.File.Delete(jobDescriptionPath);

            }
            return Ok(new
            {
                message = "Upload and processing completed successfully",
                jdId = matchedResumes.JdID,
                status = requestor_Model.CommunicationStatus
            });


        }

       


        // PUT api/<DocSimilarityComparisonController>/5

        [HttpPut("{id}")]

        public void Put(int id, [FromBody] string value)

        {

        }

        // DELETE api/<DocSimilarityComparisonController>/5

        [HttpDelete("{id}")]

        public void Delete(int id)

        {

        }



    }

}

