using CSE325_Team4_GroupProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CSE325_Team4_GroupProject.Data;

public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
}
