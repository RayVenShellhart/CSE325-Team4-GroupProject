using CSE325_Team4_GroupProject.Data;
using CSE325_Team4_GroupProject.Models;
using CSE325_Team4_GroupProject.Services;
using Xunit;

namespace CSE325_Team4_GroupProject.Tests;

public class OrderServiceTests
{
    private static OrderService CreateService(out ShopDbContext db)
    {
        db = TestDb.CreateContext();
        return new OrderService(db);
    }

    private static Order MakeOrder(int userId, decimal price, int qty)
    {
        return new Order
        {
            UserId = userId,
            CustomerName = "Test Buyer",
            Address = "1 Main St",
            City = "Testville",
            Country = "NG",
            Phone = "08012345678",
            PaymentMethod = "Card",
            Items = new List<OrderItem>
            {
                new OrderItem { ProductId = 1, Name = "Widget", Price = price, Quantity = qty }
            }
        };
    }

    [Fact]
    public async Task CreateOrderAsync_SavesOrderAndComputesTotal()
    {
        var service = CreateService(out var db);

        var id = await service.CreateOrderAsync(MakeOrder(1, 20m, 3));

        var saved = await db.Orders.FindAsync(id);
        Assert.NotNull(saved);
        Assert.Equal(60m, saved.Total);
        Assert.Equal(1, saved.UserId);
        Assert.NotEqual(default, saved.CreatedAt);
    }

    [Fact]
    public async Task GetOrderByIdAsync_IncludesItems()
    {
        var service = CreateService(out _);

        var id = await service.CreateOrderAsync(MakeOrder(1, 5m, 2));

        var order = await service.GetOrderByIdAsync(id);
        Assert.NotNull(order);
        Assert.Single(order.Items);
        Assert.Equal("Widget", order.Items[0].Name);
    }

    [Fact]
    public async Task GetOrderByIdAsync_Missing_ReturnsNull()
    {
        var service = CreateService(out _);

        var order = await service.GetOrderByIdAsync(999);

        Assert.Null(order);
    }

    [Fact]
    public async Task GetOrdersByUserAsync_OnlyReturnsThatUsersOrders()
    {
        var service = CreateService(out _);

        await service.CreateOrderAsync(MakeOrder(1, 5m, 1));
        await service.CreateOrderAsync(MakeOrder(2, 7m, 1));

        var orders = await service.GetOrdersByUserAsync(1);

        Assert.Single(orders);
        Assert.Equal(1, orders[0].UserId);
    }

    [Fact]
    public async Task GetAllOrdersAsync_OrdersByNewestFirst()
    {
        var service = CreateService(out _);

        await service.CreateOrderAsync(MakeOrder(1, 5m, 1));
        await Task.Delay(10);
        await service.CreateOrderAsync(MakeOrder(2, 7m, 1));

        var orders = await service.GetAllOrdersAsync();

        Assert.Equal(2, orders.Count);
        Assert.Equal(2, orders[0].UserId);
    }
}
