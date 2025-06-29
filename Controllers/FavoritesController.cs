using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    private readonly AppDbContext _context;
    public FavoritesController(AppDbContext context) => _context = context;

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetUserFavorites(int userId)
    {
        var favorites = await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Product)
            .ThenInclude(p => p.User)
            .Select(f => f.Product)
            .ToListAsync();

        return Ok(favorites);
    }

    [HttpPost]
    public async Task<ActionResult<Favorite>> AddFavorite(Favorite favorite)
    {
        // Verificar se o usuário existe
        var userExists = await _context.Users.AnyAsync(u => u.Id == favorite.UserId);
        if (!userExists)
        {
            return BadRequest("User not found");
        }

        // Verificar se o produto existe
        var productExists = await _context.Products.AnyAsync(p => p.Id == favorite.ProductId);
        if (!productExists)
        {
            return BadRequest("Product not found");
        }

        // Verificar se já não está favoritado
        var existingFavorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == favorite.UserId && f.ProductId == favorite.ProductId);
        
        if (existingFavorite != null)
        {
            return BadRequest("Product already favorited");
        }

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserFavorites), new { userId = favorite.UserId }, favorite);
    }

    [HttpDelete("user/{userId}/product/{productId}")]
    public async Task<IActionResult> RemoveFavorite(int userId, int productId)
    {
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

        if (favorite == null)
        {
            return NotFound();
        }

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("user/{userId}/product/{productId}/check")]
    public async Task<ActionResult<bool>> CheckIfFavorited(int userId, int productId)
    {
        var isFavorited = await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.ProductId == productId);

        return Ok(isFavorited);
    }
}

