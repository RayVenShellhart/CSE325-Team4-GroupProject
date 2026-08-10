namespace CSE325_Team4_GroupProject.Models;

public class Order
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? CustomerName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Phone { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<OrderItem> Items { get; set; } = new();
}
