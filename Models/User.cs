using System.Text.Json.Serialization;

namespace ProductAPI.Models;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public DateTime? Birthdate { get; set; }
    public byte[]? Photo { get; set; }
    [JsonIgnore]
    public List<Product> Products { get; set; } = new();
    [JsonIgnore]
    public List<Favorite> Favorites { get; set; } = new();
}