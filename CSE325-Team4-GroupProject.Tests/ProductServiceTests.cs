using CSE325_Team4_GroupProject.Models;
using CSE325_Team4_GroupProject.Services;

namespace CSE325_Team4_GroupProject.Tests;

public class ProductServiceTests
{
    private static ProductService CreateService(out CSE325_Team4_GroupProject.Data.ShopDbContext context)
    {
        context = TestDb.CreateContext();
        return new ProductService(context);
    }

    private static Product MakeProduct(string name = "Test Product", string category = "Clothing")
    {
        return new Product
        {
            Name = name,
            Price = 9.99M,
            Description = "A test product",
            Category = category,
            SellerName = "Test Seller",
            Rating = 4.0
        };
    }

    [Fact]
    public async Task SeedProductsAsync_WhenEmpty_Seeds()
    {
        var service = CreateService(out _);

        await service.SeedProductsAsync();

        var products = await service.GetAllProductsAsync();
        Assert.NotEmpty(products);
    }

    [Fact]
    public async Task SeedProductsAsync_IsIdempotent()
    {
        var service = CreateService(out _);

        await service.SeedProductsAsync();
        await service.SeedProductsAsync();

        var products = await service.GetAllProductsAsync();
        Assert.Equal(8, products.Count);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsAll_OrderedByName()
    {
        var service = CreateService(out var db);

        db.Products.AddRange(MakeProduct("Banana"), MakeProduct("Apple"));
        await db.SaveChangesAsync();

        var products = await service.GetAllProductsAsync();

        Assert.Equal(2, products.Count);
        Assert.Equal("Apple", products[0].Name);
        Assert.Equal("Banana", products[1].Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_ReturnsMatchingProduct()
    {
        var service = CreateService(out var db);

        var added = (await db.Products.AddAsync(MakeProduct("Solo"))).Entity;
        await db.SaveChangesAsync();

        var result = await service.GetProductByIdAsync(added.Id);

        Assert.NotNull(result);
        Assert.Equal("Solo", result.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_MissingId_ReturnsNull()
    {
        var service = CreateService(out _);

        var result = await service.GetProductByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProductAsync_AddsProduct()
    {
        var service = CreateService(out _);

        var created = await service.CreateProductAsync(MakeProduct("New Item"));

        Assert.True(created.Id > 0);
        var result = await service.GetProductByIdAsync(created.Id);
        Assert.NotNull(result);
        Assert.Equal("New Item", result.Name);
    }

    [Fact]
    public async Task UpdateProductAsync_UpdatesFields()
    {
        var service = CreateService(out var db);

        var existing = (await db.Products.AddAsync(MakeProduct("Old"))).Entity;
        await db.SaveChangesAsync();

        existing.Name = "New Name";
        existing.Price = 49.99M;

        var updated = await service.UpdateProductAsync(existing);

        Assert.NotNull(updated);
        Assert.Equal("New Name", updated.Name);
        Assert.Equal(49.99M, updated.Price);
    }

    [Fact]
    public async Task UpdateProductAsync_MissingProduct_ReturnsNull()
    {
        var service = CreateService(out _);

        var result = await service.UpdateProductAsync(MakeProduct("Ghost"));

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteProductAsync_RemovesProduct()
    {
        var service = CreateService(out var db);

        var existing = (await db.Products.AddAsync(MakeProduct("Doomed"))).Entity;
        await db.SaveChangesAsync();

        var deleted = await service.DeleteProductAsync(existing.Id);

        Assert.True(deleted);
        Assert.Null(await service.GetProductByIdAsync(existing.Id));
    }

    [Fact]
    public async Task DeleteProductAsync_MissingProduct_ReturnsFalse()
    {
        var service = CreateService(out _);

        var deleted = await service.DeleteProductAsync(1234);

        Assert.False(deleted);
    }
}
