using CSE325_Team4_GroupProject.Components;
using CSE325_Team4_GroupProject.Services;
using CSE325_Team4_GroupProject.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register database context
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlite("Data Source=shop.db"));

// Register services - Use Singleton for AuthStateService
builder.Services.AddSingleton<AuthStateService>();  // Changed from Scoped to Singleton
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var productService = scope.ServiceProvider.GetRequiredService<ProductService>();
    await productService.SeedProductsAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();