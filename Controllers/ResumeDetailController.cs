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
        var resume = await _context.ResumeDetails.FindAsync(id);
        return resume == null ? NotFound() : Ok(resume);
    }

    [HttpPost]
    public async Task<ActionResult> Create(Resume_Details_Model resumeDetail)
    {
        _context.ResumeDetails.Add(resumeDetail);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = resumeDetail.Id }, resumeDetail);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Resume_Details_Model resumeDetail)
    {
        if (id != resumeDetail.Id) return BadRequest();
        _context.Entry(resumeDetail).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var resume = await _context.ResumeDetails.FindAsync(id);
        if (resume == null) return NotFound();
        _context.ResumeDetails.Remove(resume);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
