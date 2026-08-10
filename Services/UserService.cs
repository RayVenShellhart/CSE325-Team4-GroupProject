using CSE325_Team4_GroupProject.Data;
using CSE325_Team4_GroupProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CSE325_Team4_GroupProject.Services;

/// <summary>
/// Handles user registration and login. Passwords are stored as hashes
/// (PBKDF2 via PasswordHasher) rather than plaintext.
/// </summary>
public class UserService
{
    private readonly ShopDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher = new();

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

        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Email != null && u.Email.ToLower() == emailLower);

        if (user == null)
        {
            return null;
        }

        // Verify the stored hash. New registrations store a hash; if a legacy
        // plaintext row is found (old dev data), fall back to a direct match
        // so existing accounts keep working during the transition.
        if (user.Password != null && user.Password.StartsWith("AQAAAA"))
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        if (user.Password == password)
        {
            user.Password = _passwordHasher.HashPassword(user, password);
            await _context.SaveChangesAsync();
            return user;
        }

        return null;
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
        if (!string.IsNullOrEmpty(user.Password))
        {
            user.Password = _passwordHasher.HashPassword(user, user.Password);
        }
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
}