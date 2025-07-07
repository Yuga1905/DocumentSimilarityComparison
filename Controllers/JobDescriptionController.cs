using DocumentSimilarityComparison;
using DocumentSimilarityComparison.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

[Route("api/[controller]")]
[ApiController]
public class JobDescriptionController : ControllerBase
{
    private readonly DocumentDbContext _context;

    public JobDescriptionController(DocumentDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var jobDescriptions = await _context.JobDescriptions
        .Include(jd => jd.Requestors)
        .Include(jd => jd.ResumeDetails)
        .Select(jd => new
        {
            jd.JdId,
            jd.JdTitle,
            jd.Description,
            Requestors = jd.Requestors.Select(r => new { r.JdId,r.Id,r.CommunicationStatus,r.ComparisonStatus }),
            ResumeDetails = jd.ResumeDetails.Select(rd => new { rd.JdId,rd.Id,rd.Name,rd.Skills,rd.Email,rd.Experience,rd.Score })
        })
        .ToListAsync();

        return Ok(jobDescriptions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var jobDescription = await _context.JobDescriptions
        .Include(jd => jd.Requestors)
        .Include(jd => jd.ResumeDetails)
        .Where(jd => jd.JdId == id)
        .Select(jd => new
        {
            jd.JdId,
            jd.JdTitle,
            jd.Description,
            Requestors = jd.Requestors.Select(r => new
            {
                r.JdId,
                r.Id,
                r.CommunicationStatus,
                r.ComparisonStatus
            }),
            ResumeDetails = jd.ResumeDetails.Select(rd => new
            {
                rd.JdId,
                rd.Id,
                rd.Name,
                rd.Skills,
                rd.Email,
                rd.Experience,
                rd.Score
            })
        })
        .FirstOrDefaultAsync();

        if (jobDescription == null)
        {
            return NotFound($"Job Description with ID {id} not found.");
        }

        return Ok(jobDescription);
    }

    [HttpPost]
    public async Task<ActionResult> Create(Job_Description_Model jobDescription)
    {
        _context.JobDescriptions.Add(jobDescription);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = jobDescription.JdId }, jobDescription);
    }    
}
