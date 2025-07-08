using DocumentSimilarityComparison;
using DocumentSimilarityComparison.AgentHelper;
using DocumentSimilarityComparison.Models;
using DocumentSimilarityComparison.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

[Route("api/[controller]")]
[ApiController]
public class ResumeDetailController : ControllerBase
{
    private readonly DocumentDbContext _context;

    public ResumeDetailController(DocumentDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var allResumes = await _context.ResumeDetails.ToListAsync();

        var distinctResumes = allResumes
            .GroupBy(r => r.Email)                       // Group by email
            .Select(g => g.OrderByDescending(r => r.Score).First()) // Take highest score
            .ToList();

        return Ok(distinctResumes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var requestor = await _context.Requestors.FindAsync(id);
        return requestor == null ? NotFound() : Ok(requestor);
    }

    [HttpPost]

    public async Task<string> SendEmail(int id)
    {
        var jdID = await _context.ResumeDetails
                    .Where(u => u.Id == id)
                    .Select(u => u.JdId)
                    .FirstOrDefaultAsync();
        string applicantName = await _context.ResumeDetails
                    .Where(u => u.Id == id)
                    .Select(u => u.Name)
                    .FirstOrDefaultAsync();
        string requestorMailId = await _context.ResumeDetails
                    .Where(u => u.Id == id)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();
        string jobtitle = await _context.JobDescriptions
                            .Where(u => u.JdId == jdID)
                            .Select(u => u.JdTitle)
                            .FirstOrDefaultAsync();
        string communicationSent = await CommunicationAgent.SendEmailWithRank(applicantName, jobtitle, requestorMailId);
        var log = await _context.ResumeDetails.FindAsync(id);      

        log.UserCommunicationStatus = communicationSent;

        _context.ResumeDetails.Update(log);
        await _context.SaveChangesAsync();
        return communicationSent;
    }

    //[HttpPost]
    //public async Task<ActionResult> Create(Resume_Details_Model resumeDetail)
    //{
    //    _requestService.CreateAsync(resumeDetail);
    //    //_context.ResumeDetails.Add(resumeDetail);
    //    await _context.SaveChangesAsync();
    //    return CreatedAtAction(nameof(Get), new { id = resumeDetail.Id }, resumeDetail);
    //}

    //[HttpPost]
    //public async Task<ActionResult<Resume_Details_Model>> Post(Resume_Details_Model resumeDetail)
    //{
    //    var result = await _requestService.CreateResumeAsync(resumeDetail);
    //    return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    //}
}
