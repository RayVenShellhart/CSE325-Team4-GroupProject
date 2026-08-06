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

    public async Task<List<Product>> GetProductsByCategoryAsync(
        string category,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return await GetAllProductsAsync(cancellationToken);
        }

        return await _context.Products
            .Where(p => p.Category != null && p.Category.ToLower() == category.ToLower())
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.Category != null)
            .Select(p => p.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetProductsBySellerAsync(
        string sellerName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sellerName))
        {
            return new List<Product>();
        }

        return await _context.Products
            .Where(p => p.SellerName != null && p.SellerName.ToLower() == sellerName.ToLower())
            .OrderByDescending(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetRelatedProductsAsync(
        Product product,
        int count = 4,
        CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.Id != product.Id &&
                        p.Category != null &&
                        product.Category != null &&
                        p.Category.ToLower() == product.Category.ToLower())
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product> AddProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        product.Id = 0; // Let the database assign the id
        product.Rating = product.Rating <= 0 ? 4.0 : product.Rating;
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == product.Id, cancellationToken);

        if (existing == null)
        {
            return false;
        }

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Description = product.Description;
        existing.ImageUrl = product.ImageUrl;
        existing.Rating = product.Rating;
        existing.SellerName = product.SellerName;
        existing.Category = product.Category;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (existing == null)
        {
            return false;
        }

        _context.Products.Remove(existing);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task SeedProductsAsync(CancellationToken cancellationToken = default)
    {
        var seedProducts = new List<Product>
        {
            // ---------- CLOTHING ----------
            new Product { Name = "Classic T-Shirt", Price = 20.00M, Description = "Comfortable 100% cotton t-shirt, available in all sizes", ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=300&h=300&fit=crop", Rating = 4.5, SellerName = "Fashion Store", Category = "Clothing" },
            new Product { Name = "Leather Jacket", Price = 85.00M, Description = "Premium genuine leather jacket with a stylish fit", ImageUrl = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=300&h=300&fit=crop", Rating = 4.7, SellerName = "Fashion Store", Category = "Clothing" },
            new Product { Name = "Slim Fit Jeans", Price = 45.00M, Description = "Classic slim fit denim jeans for everyday wear", ImageUrl = "https://images.unsplash.com/photo-1542272604-787c3835535d?w=300&h=300&fit=crop", Rating = 4.3, SellerName = "Fashion Store", Category = "Clothing" },
            new Product { Name = "Cotton Hoodie", Price = 55.00M, Description = "Warm, cozy hoodie made from soft brushed cotton", ImageUrl = "https://images.unsplash.com/photo-1556821840-3a63f95609a7?w=300&h=300&fit=crop", Rating = 4.6, SellerName = "Urban Wear", Category = "Clothing" },
            new Product { Name = "Summer Dress", Price = 40.00M, Description = "Light and breezy floral dress perfect for summer days", ImageUrl = "https://images.unsplash.com/photo-1595777457583-95e059d581b8?w=300&h=300&fit=crop", Rating = 4.4, SellerName = "Urban Wear", Category = "Clothing" },
            new Product { Name = "Classic Sneakers", Price = 65.00M, Description = "Versatile white sneakers that go with any outfit", ImageUrl = "https://images.unsplash.com/photo-1560769629-975ec94e6a86?w=300&h=300&fit=crop", Rating = 4.5, SellerName = "Kicks Hub", Category = "Clothing" },

            // ---------- FOOD ----------
            new Product { Name = "Gourmet Burger Kit", Price = 15.00M, Description = "Build your own gourmet burgers at home with fresh buns and patties", ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=300&h=300&fit=crop", Rating = 4.7, SellerName = "FreshBite Foods", Category = "Food" },
            new Product { Name = "Wood-Fired Pizza", Price = 12.00M, Description = "Authentic wood-fired margherita pizza with fresh mozzarella", ImageUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=300&h=300&fit=crop", Rating = 4.8, SellerName = "FreshBite Foods", Category = "Food" },
            new Product { Name = "Sushi Platter", Price = 28.00M, Description = "Freshly prepared assorted sushi platter for two", ImageUrl = "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?w=300&h=300&fit=crop", Rating = 4.9, SellerName = "Ocean Delights", Category = "Food" },
            new Product { Name = "Specialty Coffee Beans", Price = 18.00M, Description = "Single-origin roasted coffee beans, 500g bag", ImageUrl = "https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=300&h=300&fit=crop", Rating = 4.6, SellerName = "Brew & Co", Category = "Food" },
            new Product { Name = "Organic Fruit Basket", Price = 22.00M, Description = "Hand-picked seasonal organic fruits delivered fresh", ImageUrl = "https://images.unsplash.com/photo-1610832958506-aa56368176cf?w=300&h=300&fit=crop", Rating = 4.5, SellerName = "Green Acres", Category = "Food" },
            new Product { Name = "Chocolate Cake", Price = 25.00M, Description = "Rich, moist chocolate layer cake — perfect for celebrations", ImageUrl = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=300&h=300&fit=crop", Rating = 4.8, SellerName = "Sweet Treats", Category = "Food" },

            // ---------- FURNITURE ----------
            new Product { Name = "Wooden Chair", Price = 120.00M, Description = "Solid wood dining chair with ergonomic design", ImageUrl = "https://images.unsplash.com/photo-1503602642458-232111445657?w=300&h=300&fit=crop", Rating = 4.2, SellerName = "Furniture Store", Category = "Furniture" },
            new Product { Name = "Coffee Table", Price = 150.00M, Description = "Modern coffee table with tempered glass top", ImageUrl = "https://images.unsplash.com/photo-1499933374294-4584851497cc?w=300&h=300&fit=crop", Rating = 4.0, SellerName = "Furniture Store", Category = "Furniture" },
            new Product { Name = "Comfort Sofa", Price = 450.00M, Description = "Three-seater fabric sofa with plush cushions", ImageUrl = "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=300&h=300&fit=crop", Rating = 4.6, SellerName = "Homely Living", Category = "Furniture" },
            new Product { Name = "Floor Lamp", Price = 55.00M, Description = "Modern floor lamp with warm LED lighting", ImageUrl = "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?w=300&h=300&fit=crop", Rating = 4.3, SellerName = "Homely Living", Category = "Furniture" },
            new Product { Name = "Oak Bookshelf", Price = 180.00M, Description = "Five-shelf oak bookshelf, sturdy and stylish", ImageUrl = "https://images.unsplash.com/photo-1594620302200-9a762244a156?w=300&h=300&fit=crop", Rating = 4.4, SellerName = "WoodCraft", Category = "Furniture" },
            new Product { Name = "King Size Bed Frame", Price = 520.00M, Description = "Solid oak king size bed frame with upholstered headboard", ImageUrl = "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?w=300&h=300&fit=crop", Rating = 4.7, SellerName = "WoodCraft", Category = "Furniture" },

            // ---------- ELECTRONICS ----------
            new Product { Name = "Wireless Headphones", Price = 45.00M, Description = "High quality over-ear wireless headphones with noise cancellation", ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=300&h=300&fit=crop", Rating = 4.8, SellerName = "Tech Store", Category = "Electronics" },
            new Product { Name = "Smartphone", Price = 300.00M, Description = "Latest model smartphone with 128GB storage", ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=300&h=300&fit=crop", Rating = 4.9, SellerName = "Tech Store", Category = "Electronics" },
            new Product { Name = "Gaming Mouse", Price = 35.00M, Description = "RGB gaming mouse with programmable buttons", ImageUrl = "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=300&h=300&fit=crop", Rating = 4.4, SellerName = "Tech Store", Category = "Electronics" },
            new Product { Name = "Ultrabook Laptop", Price = 750.00M, Description = "Lightweight ultrabook with 16GB RAM and SSD storage", ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=300&h=300&fit=crop", Rating = 4.8, SellerName = "Tech Store", Category = "Electronics" },
            new Product { Name = "Smart Watch", Price = 120.00M, Description = "Smartwatch with heart-rate monitor and GPS", ImageUrl = "https://images.unsplash.com/photo-1546868871-7041f2a55e12?w=300&h=300&fit=crop", Rating = 4.5, SellerName = "Tech Store", Category = "Electronics" },
            new Product { Name = "Mechanical Keyboard", Price = 60.00M, Description = "RGB mechanical keyboard with tactile switches", ImageUrl = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=300&h=300&fit=crop", Rating = 4.6, SellerName = "Tech Store", Category = "Electronics" },
            new Product { Name = "DSLR Camera", Price = 650.00M, Description = "Professional DSLR camera with 24MP sensor and kit lens", ImageUrl = "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=300&h=300&fit=crop", Rating = 4.7, SellerName = "Tech Store", Category = "Electronics" },
            new Product { Name = "Portable Speaker", Price = 40.00M, Description = "Waterproof Bluetooth speaker with 12-hour battery", ImageUrl = "https://images.unsplash.com/photo-1545454675-3531b543be5d?w=300&h=300&fit=crop", Rating = 4.3, SellerName = "Tech Store", Category = "Electronics" },

            // ---------- BOOKS ----------
            new Product { Name = "Bestseller Novel", Price = 15.00M, Description = "The year's most talked-about fiction novel", ImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=300&h=300&fit=crop", Rating = 4.7, SellerName = "ReadMore Books", Category = "Books" },
            new Product { Name = "Cookbook Collection", Price = 25.00M, Description = "500 easy recipes from around the world", ImageUrl = "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=300&h=300&fit=crop", Rating = 4.5, SellerName = "ReadMore Books", Category = "Books" },
            new Product { Name = "Self-Development Guide", Price = 20.00M, Description = "Practical guide to building better habits", ImageUrl = "https://images.unsplash.com/photo-1506880018603-83d5b814b5a6?w=300&h=300&fit=crop", Rating = 4.6, SellerName = "ReadMore Books", Category = "Books" },
            new Product { Name = "History of Art", Price = 35.00M, Description = "Beautifully illustrated journey through art history", ImageUrl = "https://images.unsplash.com/photo-1512820790803-83ca734da794?w=300&h=300&fit=crop", Rating = 4.8, SellerName = "ReadMore Books", Category = "Books" },
            new Product { Name = "Science Fiction Trilogy", Price = 30.00M, Description = "Complete trilogy box set of a sci-fi epic", ImageUrl = "https://images.unsplash.com/photo-1516979187457-637abb4f9353?w=300&h=300&fit=crop", Rating = 4.9, SellerName = "ReadMore Books", Category = "Books" },

            // ---------- GAMES ----------
            new Product { Name = "Gaming Console", Price = 400.00M, Description = "Next-gen gaming console with 1TB storage", ImageUrl = "https://images.unsplash.com/photo-1606813907291-d86efa9b94db?w=300&h=300&fit=crop", Rating = 4.9, SellerName = "GameZone", Category = "Games" },
            new Product { Name = "Wireless Game Controller", Price = 55.00M, Description = "Ergonomic wireless controller with rumble support", ImageUrl = "https://images.unsplash.com/photo-1606144042614-b2417e99c4e3?w=300&h=300&fit=crop", Rating = 4.6, SellerName = "GameZone", Category = "Games" },
            new Product { Name = "Strategy Board Game", Price = 32.00M, Description = "Award-winning strategy board game for 2-5 players", ImageUrl = "https://images.unsplash.com/photo-1610890716171-6b1bb98ffd09?w=300&h=300&fit=crop", Rating = 4.7, SellerName = "GameZone", Category = "Games" },
            new Product { Name = "Gaming Headset", Price = 70.00M, Description = "Surround sound headset with noise-isolating mic", ImageUrl = "https://images.unsplash.com/photo-1598550476439-6847785fcea6?w=300&h=300&fit=crop", Rating = 4.5, SellerName = "GameZone", Category = "Games" },
            new Product { Name = "Gaming Desk Setup", Price = 320.00M, Description = "Complete RGB gaming setup with LED desk", ImageUrl = "https://images.unsplash.com/photo-1552820728-8b83bb6b773f?w=300&h=300&fit=crop", Rating = 4.4, SellerName = "GameZone", Category = "Games" },

            // ---------- SPORTS ----------
            new Product { Name = "Running Shoes", Price = 80.00M, Description = "Lightweight running shoes with responsive cushioning", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=300&h=300&fit=crop", Rating = 4.3, SellerName = "Sport Store", Category = "Sports" },
            new Product { Name = "Soccer Ball", Price = 25.00M, Description = "Size 5 match-quality soccer ball", ImageUrl = "https://images.unsplash.com/photo-1614632537190-23e4146777db?w=300&h=300&fit=crop", Rating = 4.4, SellerName = "Sport Store", Category = "Sports" },
            new Product { Name = "Adjustable Dumbbells", Price = 95.00M, Description = "Set of adjustable dumbbells from 2.5kg to 25kg", ImageUrl = "https://images.unsplash.com/photo-1517836357463-d25dfeac3438?w=300&h=300&fit=crop", Rating = 4.7, SellerName = "FitLife", Category = "Sports" },
            new Product { Name = "Basketball", Price = 28.00M, Description = "Official size indoor/outdoor basketball", ImageUrl = "https://images.unsplash.com/photo-1519861531473-9200262188bf?w=300&h=300&fit=crop", Rating = 4.5, SellerName = "Sport Store", Category = "Sports" },
            new Product { Name = "Yoga Mat", Price = 30.00M, Description = "Non-slip eco-friendly yoga mat with carry strap", ImageUrl = "https://images.unsplash.com/photo-1592432678016-e910b452f9a2?w=300&h=300&fit=crop", Rating = 4.6, SellerName = "FitLife", Category = "Sports" },
            new Product { Name = "Mountain Bike", Price = 380.00M, Description = "21-speed mountain bike with front suspension", ImageUrl = "https://images.unsplash.com/photo-1485965120184-e220f721d03e?w=300&h=300&fit=crop", Rating = 4.4, SellerName = "Sport Store", Category = "Sports" },

            // ---------- BEAUTY ----------
            new Product { Name = "Makeup Kit", Price = 45.00M, Description = "Complete 20-piece professional makeup kit", ImageUrl = "https://images.unsplash.com/photo-1596462502278-27bfdc403348?w=300&h=300&fit=crop", Rating = 4.6, SellerName = "Glow Beauty", Category = "Beauty" },
            new Product { Name = "Skincare Set", Price = 35.00M, Description = "Gentle cleanser, toner and moisturizer set", ImageUrl = "https://images.unsplash.com/photo-1570172619644-dfd03ed5d881?w=300&h=300&fit=crop", Rating = 4.7, SellerName = "Glow Beauty", Category = "Beauty" },
            new Product { Name = "Designer Perfume", Price = 85.00M, Description = "Long-lasting floral fragrance, 100ml", ImageUrl = "https://images.unsplash.com/photo-1541643600914-78b084683601?w=300&h=300&fit=crop", Rating = 4.8, SellerName = "Glow Beauty", Category = "Beauty" },
            new Product { Name = "Matte Lipstick Set", Price = 22.00M, Description = "Set of 6 long-wear matte lipsticks", ImageUrl = "https://images.unsplash.com/photo-1586495777744-4413f21062fa?w=300&h=300&fit=crop", Rating = 4.4, SellerName = "Glow Beauty", Category = "Beauty" },
            new Product { Name = "Hair Styling Set", Price = 48.00M, Description = "Professional hair dryer with styling tools", ImageUrl = "https://images.unsplash.com/photo-1522338242992-e1a54906a8da?w=300&h=300&fit=crop", Rating = 4.3, SellerName = "Glow Beauty", Category = "Beauty" },

            // ---------- AUTOMOTIVE ----------
            new Product { Name = "Premium Car Wax", Price = 30.00M, Description = "Carnauba car wax for a deep, glossy shine", ImageUrl = "https://images.unsplash.com/photo-1607860108855-64acf2078ed9?w=300&h=300&fit=crop", Rating = 4.5, SellerName = "AutoPro", Category = "Automotive" },
            new Product { Name = "All-Season Tire Set", Price = 320.00M, Description = "Set of 4 all-season tires for 17-inch rims", ImageUrl = "https://images.unsplash.com/photo-1486262715619-67b85e0b08d3?w=300&h=300&fit=crop", Rating = 4.6, SellerName = "AutoPro", Category = "Automotive" },
            new Product { Name = "Car Dash Camera", Price = 55.00M, Description = "HD front dash cam with night vision and loop recording", ImageUrl = "https://images.unsplash.com/photo-1503376780353-7e6692767b70?w=300&h=300&fit=crop", Rating = 4.4, SellerName = "AutoPro", Category = "Automotive" },
            new Product { Name = "Leather Seat Covers", Price = 110.00M, Description = "Custom-fit leather seat covers for front and back", ImageUrl = "https://images.unsplash.com/photo-1532968961962-8a0cb3a2d4f5?w=300&h=300&fit=crop", Rating = 4.3, SellerName = "AutoPro", Category = "Automotive" },
            new Product { Name = "Motorcycle Helmet", Price = 95.00M, Description = "Snell-certified full-face motorcycle helmet", ImageUrl = "https://images.unsplash.com/photo-1558981403-c5f9899a28bc?w=300&h=300&fit=crop", Rating = 4.7, SellerName = "AutoPro", Category = "Automotive" },

            // ---------- HOME & GARDEN ----------
            new Product { Name = "Indoor Plant Bundle", Price = 40.00M, Description = "Three low-maintenance indoor plants with pots", ImageUrl = "https://images.unsplash.com/photo-1463320726281-696a485928c7?w=300&h=300&fit=crop", Rating = 4.7, SellerName = "Green Thumb", Category = "Home & Garden" },
            new Product { Name = "Ceramic Flower Pots", Price = 25.00M, Description = "Set of 4 stylish ceramic planters", ImageUrl = "https://images.unsplash.com/photo-1485955900006-10f4d324d411?w=300&h=300&fit=crop", Rating = 4.4, SellerName = "Green Thumb", Category = "Home & Garden" },
            new Product { Name = "Garden Tool Set", Price = 35.00M, Description = "10-piece stainless steel gardening tool kit", ImageUrl = "https://images.unsplash.com/photo-1416879595882-3373a0480b5b?w=300&h=300&fit=crop", Rating = 4.5, SellerName = "Green Thumb", Category = "Home & Garden" },
            new Product { Name = "Decorative Vase", Price = 28.00M, Description = "Handcrafted ceramic vase for fresh or dried flowers", ImageUrl = "https://images.unsplash.com/photo-1578500494198-246f612d3b3d?w=300&h=300&fit=crop", Rating = 4.3, SellerName = "Green Thumb", Category = "Home & Garden" },
            new Product { Name = "Scented Candle Set", Price = 20.00M, Description = "Set of 3 natural soy wax candles in glass jars", ImageUrl = "https://images.unsplash.com/photo-1602874801007-bd458bb1b8b6?w=300&h=300&fit=crop", Rating = 4.6, SellerName = "Cozy Home", Category = "Home & Garden" },
            new Product { Name = "Wall Art Canvas", Price = 45.00M, Description = "Modern abstract canvas print, 60x90cm", ImageUrl = "https://images.unsplash.com/photo-1616486338812-3dadae4b4ace?w=300&h=300&fit=crop", Rating = 4.5, SellerName = "Cozy Home", Category = "Home & Garden" },
        };

        foreach (var product in seedProducts)
        {
            var exists = await _context.Products
                .AnyAsync(p => p.Name == product.Name, cancellationToken);

            if (!exists)
            {
                _context.Products.Add(product);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
