using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProductsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> Get() => 
        await _context.Products.Include(p => p.User).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _context.Products.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Post(Product product)
    {
        if (product.Price <= 0)
        {
            return BadRequest("Invalid price");
        }

        if (product.UserId <= 0)
        {
            return BadRequest("UserId is required");
        }

        // Verificar se o usuário existe
        var userExists = await _context.Users.AnyAsync(u => u.Id == product.UserId);
        if (!userExists)
        {
            return BadRequest("User not found");
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        // Retornar o produto com informações do usuário
        var createdProduct = await _context.Products.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == product.Id);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, createdProduct);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Product product)
    {
        if (id != product.Id) return BadRequest();

        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null) return NotFound();

        // Verificar se o usuário é o proprietário do produto
        if (existingProduct.UserId != product.UserId)
        {
            return Forbid("You can only edit your own products");
        }

        _context.Entry(existingProduct).CurrentValues.SetValues(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        // Aqui você pode adicionar verificação de autorização se necessário
        // Por exemplo: verificar se o usuário logado é o proprietário

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadImage(int id, IFormFile image)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        if (image != null && image.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            product.Image = memoryStream.ToArray();
            await _context.SaveChangesAsync();
        }

        return Ok();
    }

    [HttpGet("{id}/image")]
    public async Task<IActionResult> GetImage(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null || product.Image == null) return NotFound();

        return File(product.Image, "image/jpeg");
    }
}

