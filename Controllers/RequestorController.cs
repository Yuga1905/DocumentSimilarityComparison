using Azure.Core;
using DocumentSimilarityComparison;
using DocumentSimilarityComparison.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

[Route("api/[controller]")]
[ApiController]
public class RequestorController : ControllerBase
{
    private readonly DocumentDbContext _context;

    public RequestorController(DocumentDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult> GetAll() => Ok(await _context.Requestors.ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var requestor = await _context.Requestors.FindAsync(id);
        return requestor == null ? NotFound() : Ok(requestor);
    }

    [HttpPost]
    public async Task<ActionResult> Create(Requestor_Model requestor)
    {
        _context.Requestors.Add(requestor);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = requestor.Id }, requestor);
    }

   
}
