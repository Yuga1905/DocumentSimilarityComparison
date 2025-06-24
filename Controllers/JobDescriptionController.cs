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
    public async Task<ActionResult> GetAll() => Ok(await _context.JobDescriptions.ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var job = await _context.JobDescriptions.FindAsync(id);
        return job == null ? NotFound() : Ok(job);
    }

    [HttpPost]
    public async Task<ActionResult> Create(Job_Description_Model jobDescription)
    {
        _context.JobDescriptions.Add(jobDescription);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = jobDescription.JdId }, jobDescription);
    }    
}
