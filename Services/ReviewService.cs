using CSE325_Team4_GroupProject.Data;
using CSE325_Team4_GroupProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CSE325_Team4_GroupProject.Services;

public class ReviewService
{
    private readonly ShopDbContext _context;

    public ReviewService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<List<Review>> GetReviewsByProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Review?> GetUserReviewForProductAsync(int productId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId, cancellationToken);
    }

    public async Task<(bool Success, string Message, Review? Review)> AddOrUpdateReviewAsync(
        int productId,
        int userId,
        int rating,
        string? comment,
        CancellationToken cancellationToken = default)
    {
        if (rating < 1 || rating > 5)
            return (false, "Rating must be between 1 and 5.", null);

        var product = await _context.Products.FindAsync(new object[] { productId }, cancellationToken);
        if (product == null)
            return (false, "Product not found.", null);

        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null || !user.IsActive)
            return (false, "User not found or inactive.", null);

        var existing = await _context.Reviews
            .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId, cancellationToken);

        Review review;
        if (existing != null)
        {
            existing.Rating = rating;
            existing.Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
            existing.CreatedAt = DateTime.Now;
            review = existing;
        }
        else
        {
            review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
                CreatedAt = DateTime.Now
            };
            _context.Reviews.Add(review);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await RecalculateProductRatingAsync(productId, cancellationToken);

        return (true, existing != null ? "Review updated." : "Review submitted.", review);
    }

    public async Task<(bool Success, string Message)> DeleteReviewAsync(
        int reviewId,
        int userId,
        bool isAdmin = false,
        CancellationToken cancellationToken = default)
    {
        var review = await _context.Reviews.FindAsync(new object[] { reviewId }, cancellationToken);
        if (review == null)
            return (false, "Review not found.");

        if (!isAdmin && review.UserId != userId)
            return (false, "You can only delete your own review.");

        var productId = review.ProductId;
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync(cancellationToken);
        await RecalculateProductRatingAsync(productId, cancellationToken);

        return (true, "Review deleted.");
    }

    public async Task RecalculateProductRatingAsync(int productId, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FindAsync(new object[] { productId }, cancellationToken);
        if (product == null)
            return;

        var ratings = await _context.Reviews
            .Where(r => r.ProductId == productId)
            .Select(r => r.Rating)
            .ToListAsync(cancellationToken);

        product.Rating = ratings.Count == 0
            ? 0
            : Math.Round(ratings.Average(), 1);

        product.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
