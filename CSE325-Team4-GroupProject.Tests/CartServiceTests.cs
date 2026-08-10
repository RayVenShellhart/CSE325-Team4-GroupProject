using CSE325_Team4_GroupProject.Models;
using CSE325_Team4_GroupProject.Services;
using Microsoft.JSInterop;
using Moq;

namespace CSE325_Team4_GroupProject.Tests;

public class CartServiceTests
{
    // Moq returns default(ValueTask) / default(ValueTask<T>) for the interface
    // methods, i.e. a completed task with a null result — exactly what an empty
    // localStorage looks like. No explicit setups are required.
    private static CartService CreateService()
    {
        return new CartService(new Mock<IJSRuntime>().Object);
    }

    private static Product MakeProduct(int id, string name, decimal price)
    {
        return new Product { Id = id, Name = name, Price = price, ImageUrl = "img" };
    }

    [Fact]
    public async Task Add_NewItem_AddsQuantityOne()
    {
        var service = CreateService();

        await service.Add(MakeProduct(1, "Shoes", 50M));

        Assert.Single(service.Items);
        Assert.Equal(1, service.GetQuantity(1));
        Assert.Equal(1, service.ItemCount);
        Assert.Equal(50M, service.TotalPrice);
    }

    [Fact]
    public async Task Add_DuplicateItem_IncrementsQuantity()
    {
        var service = CreateService();
        var product = MakeProduct(1, "Shoes", 50M);

        await service.Add(product);
        await service.Add(product);

        Assert.Single(service.Items);
        Assert.Equal(2, service.GetQuantity(1));
        Assert.Equal(2, service.ItemCount);
        Assert.Equal(100M, service.TotalPrice);
    }

    [Fact]
    public async Task UpdateQuantity_Zero_RemovesItem()
    {
        var service = CreateService();
        await service.Add(MakeProduct(1, "Shoes", 50M));

        await service.UpdateQuantity(1, 0);

        Assert.Empty(service.Items);
    }

    [Fact]
    public async Task UpdateQuantity_IncreasesQuantity()
    {
        var service = CreateService();
        await service.Add(MakeProduct(1, "Shoes", 50M));

        await service.UpdateQuantity(1, 4);

        Assert.Equal(4, service.GetQuantity(1));
        Assert.Equal(200M, service.TotalPrice);
    }

    [Fact]
    public async Task Remove_RemovesItem()
    {
        var service = CreateService();
        await service.Add(MakeProduct(1, "Shoes", 50M));
        await service.Add(MakeProduct(2, "Shirt", 20M));

        await service.Remove(1);

        Assert.Single(service.Items);
        Assert.False(service.Contains(1));
        Assert.True(service.Contains(2));
    }

    [Fact]
    public async Task Clear_EmptiesCart()
    {
        var service = CreateService();
        await service.Add(MakeProduct(1, "Shoes", 50M));
        await service.Add(MakeProduct(2, "Shirt", 20M));

        await service.Clear();

        Assert.Empty(service.Items);
        Assert.Equal(0, service.ItemCount);
        Assert.Equal(0M, service.TotalPrice);
    }

    [Fact]
    public async Task LoadCartAsync_EmptyStorage_NoItems()
    {
        var service = CreateService();

        await service.LoadCartAsync();

        Assert.Empty(service.Items);
    }
}
