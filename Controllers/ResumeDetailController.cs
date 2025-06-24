using DocumentSimilarityComparison;
using DocumentSimilarityComparison.Models;
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
    public async Task<ActionResult> GetAll() => Ok(await _context.ResumeDetails.ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var requestor = await _context.Requestors.FindAsync(id);
        return requestor == null ? NotFound() : Ok(requestor);
    }

    [HttpPost]
    public async Task<ActionResult> Create(Resume_Details_Model resumeDetail)
    {
        _context.ResumeDetails.Add(resumeDetail);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = resumeDetail.Id }, resumeDetail);
    }   
}
