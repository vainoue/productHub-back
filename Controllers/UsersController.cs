using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsersController(AppDbContext context) => _context = context;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            return BadRequest("Username and password are required");

        var normalizedUsername = user.Username.Trim().ToLower();
        var existUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == normalizedUsername);
        if (existUser != null)
            return BadRequest("Username already exists");

        user.Username = normalizedUsername;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User user)
    {
        if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            return BadRequest("Username and password are required");

        var normalizedUsername = user.Username.Trim().ToLower();
        var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Username.Trim().ToLower() == normalizedUsername && u.Password == user.Password);
        if (dbUser == null)
            return Unauthorized(new { message = "Invalid username or password" });

        return Ok(new { id = dbUser.Id, username = dbUser.Username, email = dbUser.Email, birthdate = dbUser.Birthdate, photo = dbUser.Photo });
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] User user)
    {
        var updateUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
        if (updateUser == null)
            return BadRequest("User not found");

        if (!string.IsNullOrWhiteSpace(user.Email))
            updateUser.Email = user.Email;

        if (user.Birthdate.HasValue)
        {
            var utcBirthdate = DateTime.SpecifyKind(user.Birthdate.Value, DateTimeKind.Utc);
            updateUser.Birthdate = utcBirthdate;
        }
            

        await _context.SaveChangesAsync();
        return Ok(new { message = "User updated successfully" });
    }

    [HttpPut("update-photo")]
    public async Task<IActionResult> UpdatePhoto([FromForm] int userId, [FromForm] IFormFile file)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound("User not found!");

        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        using var image = new MemoryStream();
        await file.CopyToAsync(image);
        user.Photo = image.ToArray();

        await _context.SaveChangesAsync();
        return Ok(new { message = "Photo uploaded successfully!" });
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users.Select(u => new
        {
            u.Id,
            u.Username,
            u.Email,
            u.Birthdate,
            u.Photo
        })
        .ToListAsync();

        return Ok(users);
    }

}
