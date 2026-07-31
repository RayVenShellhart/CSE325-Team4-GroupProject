using CSE325_Team4_GroupProject.Models;

namespace CSE325_Team4_GroupProject.Services;

public class CartService
{
    private readonly List<CartItem> _items = new();

    public event Action? Changed;

    public IReadOnlyList<CartItem> Items => _items;

    public int ItemCount => _items.Sum(i => i.Quantity);

    public decimal TotalPrice => _items.Sum(i => i.Price * i.Quantity);

    public void Add(Product product)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing is null)
        {
            _items.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name ?? "Unnamed product",
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Quantity = 1
            });
        }
        else
        {
            existing.Quantity++;
        }

        Changed?.Invoke();
    }

    public void Remove(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null) return;

        _items.Remove(item);
        Changed?.Invoke();
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null) return;

        if (quantity <= 0)
        {
            _items.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        Changed?.Invoke();
    }

    public void Clear()
    {
        _items.Clear();
        Changed?.Invoke();
    }
}
