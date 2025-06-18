using DocumentSimilarityComparison.AgentHelper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocumentSimilarityComparison.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocSimilarityComparisonController : ControllerBase
    {
        // GET: api/<DocSimilarityComparisonController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            List<string> resumes=new List<string>
            {
                "Resume 1: Experience in c#,.NET and AI development",
                "Resume 2: Skilled in Python, Machine Learning and Data Science",
                "Resume 3: Expertise in Java, Spring boot and Microservices",
            };

            string jobDescription = "Looking for a developer who has experience in c#,.NET and AI development ";
            var matchedResumes=ComparisonAgent.MatchResumesWithJobDescription(resumes, jobDescription);

            return new string[] { "value1", "value2" };
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
