using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = "";

    [Required]
    public string PasswordHash { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<FoodItem> Foods { get; set; } = new List<FoodItem>();
}