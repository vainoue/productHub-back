using Microsoft.OpenApi.Models;

namespace ProductAPI.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public byte[]? Image { get; set; }
}

