using CSE325_Team4_GroupProject.Models;
using CSE325_Team4_GroupProject.Services;

namespace CSE325_Team4_GroupProject.Tests;

public class UserServiceTests
{
    private static UserService CreateService(out CSE325_Team4_GroupProject.Data.ShopDbContext context)
    {
        context = TestDb.CreateContext();
        return new UserService(context);
    }

    [Fact]
    public async Task RegisterAsync_ValidUser_StoresHashedPassword()
    {
        var service = CreateService(out var _);

        var created = await service.RegisterAsync(new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "secret123"
        });

        Assert.NotNull(created);
        Assert.NotNull(created.Password);
        Assert.NotEqual("secret123", created.Password);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ReturnsNull()
    {
        var service = CreateService(out var _);

        await service.RegisterAsync(new User { FirstName = "A", LastName = "B", Email = "dup@example.com", Password = "secret123" });
        var duplicate = await service.RegisterAsync(new User { FirstName = "C", LastName = "D", Email = "DUP@example.com", Password = "other456" });

        Assert.Null(duplicate);
    }

    [Fact]
    public async Task RegisterAsync_NullOrEmptyEmail_ReturnsNull()
    {
        var service = CreateService(out var _);

        var result = await service.RegisterAsync(new User { Email = "", Password = "secret123" });

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsUser()
    {
        var service = CreateService(out var _);

        await service.RegisterAsync(new User { FirstName = "Jane", LastName = "Roe", Email = "jane@example.com", Password = "correct" });
        var user = await service.LoginAsync("jane@example.com", "correct");

        Assert.NotNull(user);
        Assert.Equal("Jane", user.FirstName);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsNull()
    {
        var service = CreateService(out var _);

        await service.RegisterAsync(new User { FirstName = "Jane", LastName = "Roe", Email = "jane@example.com", Password = "correct" });
        var user = await service.LoginAsync("jane@example.com", "wrong");

        Assert.Null(user);
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ReturnsNull()
    {
        var service = CreateService(out var _);

        var user = await service.LoginAsync("nobody@example.com", "whatever");

        Assert.Null(user);
    }

    [Fact]
    public async Task LoginAsync_LegacyPlaintextPassword_IsUpgradedToHash()
    {
        var service = CreateService(out var db);

        // Simulate a legacy row stored as plaintext before hashing was introduced.
        db.Users.Add(new User { FirstName = "Legacy", LastName = "User", Email = "legacy@example.com", Password = "plainpass" });
        await db.SaveChangesAsync();

        var user = await service.LoginAsync("legacy@example.com", "plainpass");

        Assert.NotNull(user);
        Assert.StartsWith("AQAAAA", user.Password);
    }
}

