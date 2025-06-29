using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace ProductAPI.Models;

public class Product
{
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Precision(10, 2)]
    public decimal Price { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public byte[]? Image { get; set; }
}

