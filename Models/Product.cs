namespace CSE325_Team4_GroupProject.Models;

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    /// <summary>
    /// Average rating (1.0–5.0). Can be recalculated from Reviews.
    /// </summary>
    public double Rating { get; set; }
    public int? SellerId { get; set; }
    /// <summary>
    /// Denormalized display name for convenience / backward compatibility.
    /// Prefer resolving via Seller navigation when possible.
    /// </summary>
    public string? SellerName { get; set; }
    public string? Category { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User? Seller { get; set; }
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
