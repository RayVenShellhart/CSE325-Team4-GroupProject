using CSE325_Team4_GroupProject.Data;
using CSE325_Team4_GroupProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CSE325_Team4_GroupProject.Services;

/// <summary>
/// Data access for products: querying, seeding, and full CRUD operations.
/// </summary>
public class ProductService
{
    private readonly ShopDbContext _context;

    public ProductService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetProductByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .OrderByDescending(p => p.Rating)
            .Take(3)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product?> UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id, cancellationToken);

        if (existing == null)
        {
            return null;
        }

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Description = product.Description;
        existing.ImageUrl = product.ImageUrl;
        existing.Rating = product.Rating;
        existing.SellerName = product.SellerName;
        existing.Category = product.Category;

        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product == null)
        {
            return false;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task SeedProductsAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Classic T-Shirt", Price = 20.00M, Description = "Comfortable cotton t-shirt", ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=300&h=300&fit=crop", Rating = 4.5, SellerName = "Fashion Store", Category = "Clothing" },
            new Product { Id = 2, Name = "Wireless Headphones", Price = 45.00M, Description = "High quality sound", ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=300&h=300&fit=crop", Rating = 4.8, SellerName = "Tech Store", Category = "Electronics" },
            new Product { Id = 3, Name = "Wooden Chair", Price = 120.00M, Description = "Solid wood dining chair", ImageUrl = "https://images.unsplash.com/photo-1503602642458-232111445657?w=300&h=300&fit=crop", Rating = 4.2, SellerName = "Furniture Store", Category = "Furniture" },
            new Product { Id = 4, Name = "Smartphone", Price = 300.00M, Description = "Latest model smartphone", ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=300&h=300&fit=crop", Rating = 4.9, SellerName = "Tech Store", Category = "Electronics" },
            new Product { Id = 5, Name = "Running Shoes", Price = 80.00M, Description = "Comfortable running shoes", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=300&h=300&fit=crop", Rating = 4.3, SellerName = "Sport Store", Category = "Sports" },
            new Product { Id = 6, Name = "Coffee Table", Price = 150.00M, Description = "Modern coffee table", ImageUrl = "https://images.unsplash.com/photo-1499933374294-4584851497cc?w=300&h=300&fit=crop", Rating = 4.0, SellerName = "Furniture Store", Category = "Furniture" },
            new Product { Id = 7, Name = "Leather Jacket", Price = 85.00M, Description = "Premium leather jacket", ImageUrl = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=300&h=300&fit=crop", Rating = 4.7, SellerName = "Fashion Store", Category = "Clothing" },
            new Product { Id = 8, Name = "Gaming Mouse", Price = 35.00M, Description = "RGB gaming mouse", ImageUrl = "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=300&h=300&fit=crop", Rating = 4.4, SellerName = "Tech Store", Category = "Electronics" }
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
