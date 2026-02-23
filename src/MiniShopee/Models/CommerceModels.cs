namespace MiniShopee.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PriceVnd { get; set; }
    public int Stock { get; set; }
}

public class Voucher
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public bool VipOnly { get; set; }
    public string CreatedByRole { get; set; } = string.Empty;
    public DateTime ExpiredAt { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
    public string PaymentMethod { get; set; } = "COD";
    public decimal TotalVnd { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPriceVnd { get; set; }
}

public class UserReport
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
