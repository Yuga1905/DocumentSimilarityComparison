using DocumentSimilarityComparison;
using DocumentSimilarityComparison.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly DocumentDbContext _context;

    public UserController(DocumentDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult> GetAll() => Ok(await _context.Users.ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult> Create(UserModel user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UserModel user)
    {
        if (id != user.Id) return BadRequest();
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
