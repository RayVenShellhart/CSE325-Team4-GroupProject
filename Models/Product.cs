namespace CSE325_Team4_GroupProject.Models;

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public double Rating { get; set; }
    public string? SellerName { get; set; }
    public string? Category { get; set; }
}