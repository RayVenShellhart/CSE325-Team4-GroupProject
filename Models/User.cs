namespace CSE325_Team4_GroupProject.Models;

public class User
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? UserType { get; set; } // "Buyer" or "Seller"
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? ProfileImage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}