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
                u.Email != null && u.Email.ToLower() == emailLower &&
                u.Password == password);
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

        user.CreatedAt = DateTime.Now;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
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

    public async Task<User?> UpdateProfileAsync(User user, CancellationToken cancellationToken = default)
    {
        if (user == null)
        {
            return null;
        }

        var existing = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

        if (existing == null)
        {
            return null;
        }

        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.PhoneNumber = user.PhoneNumber;
        existing.Address = user.Address;
        existing.ProfileImage = user.ProfileImage;

        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> ChangePasswordAsync(
        int userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
        {
            return false;
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null || user.Password != currentPassword)
        {
            return false;
        }

        user.Password = newPassword;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
