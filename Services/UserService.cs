using CSE325_Team4_GroupProject.Data;
using CSE325_Team4_GroupProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CSE325_Team4_GroupProject.Services;

public class UserService
{
    private readonly ShopDbContext _context;

    public UserService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        var emailLower = email.ToLower();

        return await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Email != null &&
                u.Email.ToLower() == emailLower &&
                u.Password == password &&
                u.IsActive);
    }

    public async Task<User?> RegisterAsync(User user)
    {
        if (user == null || string.IsNullOrEmpty(user.Email))
        {
            return null;
        }

        var emailLower = user.Email.ToLower();

        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email != null && u.Email.ToLower() == emailLower))
        {
            return null; // Email already exists
        }

        // Prevent self-registration as Admin
        if (string.Equals(user.UserType, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            user.UserType = "Buyer";
        }

        user.IsActive = true;
        user.CreatedAt = DateTime.Now;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.OrderBy(u => u.CreatedAt).ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return null;
        }

        var emailLower = email.ToLower();

        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == emailLower);
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        var existing = await _context.Users.FindAsync(user.Id);
        if (existing == null)
            return false;

        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.Email = user.Email;
        existing.PhoneNumber = user.PhoneNumber;
        existing.Address = user.Address;
        existing.ProfileImage = user.ProfileImage;
        existing.UserType = user.UserType;
        existing.IsActive = user.IsActive;

        if (!string.IsNullOrEmpty(user.Password))
        {
            existing.Password = user.Password;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetUserActiveStatusAsync(int userId, bool isActive)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        user.IsActive = isActive;
        await _context.SaveChangesAsync();
        return true;
    }
}
