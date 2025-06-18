using DocumentSimilarityComparison.AgentHelper;
using DocumentSimilarityComparison.DTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

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
        public async Task<List<ResumeDTO>> Get()
        {
            //string jobDescription = "Looking for a developer who has experience in c#,.NET and AI development ";
            string jobDescription = "Looking for a developer who has experience in python ";
            List<ResumeDTO> resumeList = new List<ResumeDTO>();
            resumeList.Add(new ResumeDTO { ApplicantName = "John", JobDescription = "Experience in c#,.NET and AI development", ApplicantEmailId="test@gmail.com" });
            resumeList.Add(new ResumeDTO { ApplicantName = "Kevin", JobDescription = "Skilled in Python, Machine Learning and Data Science", ApplicantEmailId = "yugashini1905@gmail.com" });
            resumeList.Add(new ResumeDTO { ApplicantName = "Sara", JobDescription = "Expertise in Java, Spring boot and Microservices", ApplicantEmailId = "sample@gmail.com" });
            
            List<ResumeDTO> matchedResumes = await ComparisonAgent.MatchResumesWithJobDescription(resumeList, jobDescription);
            List<ResumeDTO> RankedResumes = await RankingAgent.RankResumesWithScore(matchedResumes);
            bool communicationSent = await CommunicationAgent.SendEmailWithRank(RankedResumes);
            return RankedResumes;
        }

        // GET api/<DocSimilarityComparisonController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DocSimilarityComparisonController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
