using CSE325_Team4_GroupProject.Data;
using CSE325_Team4_GroupProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CSE325_Team4_GroupProject.Services;

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
            .Include(p => p.Seller)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetAllProductsIncludingInactiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.Rating)
            .Take(3)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetProductsBySellerAsync(int sellerId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.SellerId == sellerId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> CreateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        if (product == null || string.IsNullOrWhiteSpace(product.Name))
            return null;

        product.CreatedAt = DateTime.Now;
        product.UpdatedAt = null;
        product.IsActive = true;
        if (product.Rating < 0) product.Rating = 0;

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product?> UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Products.FindAsync(new object[] { product.Id }, cancellationToken);
        if (existing == null)
            return null;

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Description = product.Description;
        existing.ImageUrl = product.ImageUrl;
        existing.Category = product.Category;
        existing.Stock = product.Stock;
        existing.IsActive = product.IsActive;
        existing.SellerName = product.SellerName;
        existing.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteProductAsync(int productId, int? sellerId = null, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FindAsync(new object[] { productId }, cancellationToken);
        if (product == null)
            return false;

        // If sellerId provided, ensure ownership (admins can pass null to delete any)
        if (sellerId.HasValue && product.SellerId != sellerId.Value)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetProductActiveStatusAsync(int productId, bool isActive, int? sellerId = null, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FindAsync(new object[] { productId }, cancellationToken);
        if (product == null)
            return false;

        if (sellerId.HasValue && product.SellerId != sellerId.Value)
            return false;

        product.IsActive = isActive;
        product.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task SeedProductsAsync(CancellationToken cancellationToken = default)
    {
        // Seed users (admin + sellers) if none exist
        if (!await _context.Users.AnyAsync(cancellationToken))
        {
            var admin = new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@shophub.com",
                Password = "Admin123!",
                UserType = "Admin",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var fashionSeller = new User
            {
                FirstName = "Fashion",
                LastName = "Store",
                Email = "fashion@shophub.com",
                Password = "Seller123!",
                UserType = "Seller",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var techSeller = new User
            {
                FirstName = "Tech",
                LastName = "Store",
                Email = "tech@shophub.com",
                Password = "Seller123!",
                UserType = "Seller",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var furnitureSeller = new User
            {
                FirstName = "Furniture",
                LastName = "Store",
                Email = "furniture@shophub.com",
                Password = "Seller123!",
                UserType = "Seller",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var sportSeller = new User
            {
                FirstName = "Sport",
                LastName = "Store",
                Email = "sport@shophub.com",
                Password = "Seller123!",
                UserType = "Seller",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.AddRange(admin, fashionSeller, techSeller, furnitureSeller, sportSeller);
            await _context.SaveChangesAsync(cancellationToken);
        }

        if (await _context.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        var fashion = await _context.Users.FirstAsync(u => u.Email == "fashion@shophub.com", cancellationToken);
        var tech = await _context.Users.FirstAsync(u => u.Email == "tech@shophub.com", cancellationToken);
        var furniture = await _context.Users.FirstAsync(u => u.Email == "furniture@shophub.com", cancellationToken);
        var sport = await _context.Users.FirstAsync(u => u.Email == "sport@shophub.com", cancellationToken);

        var products = new List<Product>
        {
            new Product
            {
                Name = "Classic T-Shirt",
                Price = 20.00M,
                Description = "Comfortable cotton t-shirt",
                ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=300&h=300&fit=crop",
                Rating = 4.5,
                SellerId = fashion.Id,
                SellerName = "Fashion Store",
                Category = "Clothing",
                Stock = 50,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Wireless Headphones",
                Price = 45.00M,
                Description = "High quality sound",
                ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=300&h=300&fit=crop",
                Rating = 4.8,
                SellerId = tech.Id,
                SellerName = "Tech Store",
                Category = "Electronics",
                Stock = 30,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Wooden Chair",
                Price = 120.00M,
                Description = "Solid wood dining chair",
                ImageUrl = "https://images.unsplash.com/photo-1503602642458-232111445657?w=300&h=300&fit=crop",
                Rating = 4.2,
                SellerId = furniture.Id,
                SellerName = "Furniture Store",
                Category = "Furniture",
                Stock = 15,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Smartphone",
                Price = 300.00M,
                Description = "Latest model smartphone",
                ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=300&h=300&fit=crop",
                Rating = 4.9,
                SellerId = tech.Id,
                SellerName = "Tech Store",
                Category = "Electronics",
                Stock = 20,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Running Shoes",
                Price = 80.00M,
                Description = "Comfortable running shoes",
                ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=300&h=300&fit=crop",
                Rating = 4.3,
                SellerId = sport.Id,
                SellerName = "Sport Store",
                Category = "Sports",
                Stock = 40,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Coffee Table",
                Price = 150.00M,
                Description = "Modern coffee table",
                ImageUrl = "https://images.unsplash.com/photo-1499933374294-4584851497cc?w=300&h=300&fit=crop",
                Rating = 4.0,
                SellerId = furniture.Id,
                SellerName = "Furniture Store",
                Category = "Furniture",
                Stock = 10,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Leather Jacket",
                Price = 85.00M,
                Description = "Premium leather jacket",
                ImageUrl = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=300&h=300&fit=crop",
                Rating = 4.7,
                SellerId = fashion.Id,
                SellerName = "Fashion Store",
                Category = "Clothing",
                Stock = 25,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Gaming Mouse",
                Price = 35.00M,
                Description = "RGB gaming mouse",
                ImageUrl = "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=300&h=300&fit=crop",
                Rating = 4.4,
                SellerId = tech.Id,
                SellerName = "Tech Store",
                Category = "Electronics",
                Stock = 60,
                IsActive = true,
                CreatedAt = DateTime.Now
            }
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
