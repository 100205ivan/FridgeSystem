using Microsoft.EntityFrameworkCore;
using FridgeSystem.Models;

namespace FridgeSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<FoodItem> Foods { get; set; }
}