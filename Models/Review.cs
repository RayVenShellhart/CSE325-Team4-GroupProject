namespace CSE325_Team4_GroupProject.Models;

public class Review
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    /// <summary>
    /// Rating value from 1 to 5.
    /// </summary>
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public Product? Product { get; set; }
    public User? User { get; set; }
}
