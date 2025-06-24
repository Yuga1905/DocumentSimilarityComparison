using DocumentSimilarityComparison;
using DocumentSimilarityComparison.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly DocumentDbContext _context;

    public UserController(DocumentDbContext context) => _context = context;

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserModel request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);
        if (user == null) return Unauthorized("Invalid email or password.");
        return Ok(user);
    }

    [HttpPost]
    public async Task<string> Create(UserModel user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return "Registered Successfully";
    }
}
