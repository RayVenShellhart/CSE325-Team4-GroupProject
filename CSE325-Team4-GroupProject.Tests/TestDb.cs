using CSE325_Team4_GroupProject.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CSE325_Team4_GroupProject.Tests;

/// <summary>
/// Creates an in-memory SQLite-backed ShopDbContext for unit tests.
/// The connection is kept open for the lifetime of the context so the
/// in-memory database persists across queries within the test.
/// </summary>
public static class TestDb
{
    public static ShopDbContext CreateContext()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ShopDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ShopDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
