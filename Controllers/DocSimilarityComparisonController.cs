using DocumentSimilarityComparison.AgentHelper;
using DocumentSimilarityComparison.AzureHelper;
using DocumentSimilarityComparison.DTO;
using DocumentSimilarityComparison.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocumentSimilarityComparison.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocSimilarityComparisonController : ControllerBase
    {
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
            
            var jobDescriptionPath = Path.Combine("C:\\Users\\1000055632\\source\\repos\\DocumentSimilarityComparison\\DocumentSimilarityComparison\\Resources\\JobDescription", file.FileName);
            //var jobDescriptionPath = Path.Combine("C:\\Users\\2000112352\\source\\repos\\Backend\\DocumentSimilarityComparison\\Resources\\JobDescription", file.FileName);

            using (var stream = new FileStream(jobDescriptionPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            JobDescriptionDTO matchedResumes = await ComparisonAgent.MatchResumesWithJobDescription(jobDescriptionPath);
            JobDescriptionDTO RankedResumes = await RankingAgent.RankResumesWithScore(matchedResumes);
            string communicationSent = await CommunicationAgent.SendEmailWithRank(RankedResumes, RequestorEmailId);

            var requestor_Model = new Requestor_Model
            {
                ComparisonStatus = "Comparision Completed",
                CommunicationStatus = communicationSent,
            };

            if (matchedResumes.JdID > 0)
            {
                requestor_Model.JdId = matchedResumes.JdID;
                await AzureAIClientService.InsertRequestorDetails(requestor_Model);
            }

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

