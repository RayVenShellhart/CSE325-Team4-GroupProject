using System.Text.Json;
using Microsoft.JSInterop;
using CSE325_Team4_GroupProject.Models;

namespace CSE325_Team4_GroupProject.Services;

public class CartService
{
    private readonly IJSRuntime _js;

    private const string StorageKey = "shopping_cart";

    private readonly List<CartItem> _items = new();

    public event Action? Changed;

    public CartService(IJSRuntime js)
    {
        _js = js;
    }

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    public int ItemCount => _items.Sum(i => i.Quantity);

    public decimal TotalPrice => _items.Sum(i => i.Price * i.Quantity);

    public async Task LoadCartAsync()
    {
        try
        {
            var json = await _js.InvokeAsync<string?>(
                "localStorage.getItem",
                StorageKey);

            _items.Clear();

            if (!string.IsNullOrWhiteSpace(json))
            {
                var items = JsonSerializer.Deserialize<List<CartItem>>(json);

                if (items != null)
                {
                    _items.AddRange(items);
                }
            }

            Changed?.Invoke();
        }
        catch
        {
            // Ignore storage errors
        }
    }

    public async Task Add(Product product)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);

        if (existing == null)
        {
            _items.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name ?? string.Empty,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Quantity = 1
            });
        }
        else
        {
            existing.Quantity++;
        }

        await SaveAsync();

        Changed?.Invoke();
    }

    public async Task UpdateQuantity(int productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);

        if (item == null)
            return;

        if (quantity <= 0)
        {
            _items.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        await SaveAsync();

        Changed?.Invoke();
    }

    public async Task Remove(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);

        if (item == null)
            return;

        _items.Remove(item);

        await SaveAsync();

        Changed?.Invoke();
    }

    public async Task Clear()
    {
        _items.Clear();

        await SaveAsync();

        Changed?.Invoke();
    }

    public bool Contains(int productId)
    {
        return _items.Any(i => i.ProductId == productId);
    }

    public int GetQuantity(int productId)
    {
        return _items
            .FirstOrDefault(i => i.ProductId == productId)?
            .Quantity ?? 0;
    }

    private async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(_items);

        await _js.InvokeVoidAsync(
            "localStorage.setItem",
            StorageKey,
            json);
    }
}