namespace MiniShopee.Services;

public class CartState
{
    public Dictionary<int, int> Items { get; } = new();

    public void Add(int productId)
    {
        if (!Items.TryAdd(productId, 1))
        {
            Items[productId]++;
        }
    }

    public void Clear() => Items.Clear();
}
