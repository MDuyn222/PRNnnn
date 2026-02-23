using MiniShopee.Data;
using MiniShopee.Models;
using Microsoft.EntityFrameworkCore;

namespace MiniShopee.Services;

public class CommerceService(AppDbContext db, UserGovernanceService governance)
{
    public Task<List<Product>> GetProductsAsync() => db.Products.OrderBy(p => p.Id).ToListAsync();

    public Task<List<Voucher>> GetVouchersAsync(bool isVip) => db.Vouchers
        .Where(v => !v.VipOnly || isVip)
        .OrderBy(v => v.ExpiredAt)
        .ToListAsync();

    public async Task<Order> PlaceOrderAsync(ApplicationUser user, Dictionary<int, int> cart, string paymentMethod, string? voucherCode)
    {
        var products = await db.Products.Where(p => cart.Keys.Contains(p.Id)).ToListAsync();
        var order = new Order { UserId = user.Id, PaymentMethod = paymentMethod, Status = "Confirmed" };

        foreach (var product in products)
        {
            var qty = cart[product.Id];
            if (qty <= 0 || product.Stock < qty)
            {
                continue;
            }

            product.Stock -= qty;
            order.Items.Add(new OrderItem { ProductId = product.Id, Quantity = qty, UnitPriceVnd = product.PriceVnd });
            order.TotalVnd += qty * product.PriceVnd;
        }

        if (!string.IsNullOrWhiteSpace(voucherCode))
        {
            var voucher = await db.Vouchers.FirstOrDefaultAsync(v => v.Code == voucherCode && v.ExpiredAt > DateTime.UtcNow);
            if (voucher is not null && (!voucher.VipOnly || await governance.IsVipAsync(user)))
            {
                order.TotalVnd -= order.TotalVnd * voucher.DiscountPercent / 100;
            }
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync();
        await governance.ApplySpendingAndVipRuleAsync(user, order.TotalVnd);
        return order;
    }

    public Task<List<Order>> GetOrdersByUserAsync(string userId) => db.Orders
        .Include(o => o.Items)
        .ThenInclude(i => i.Product)
        .Where(o => o.UserId == userId)
        .OrderByDescending(o => o.CreatedAt)
        .ToListAsync();

    public Task<List<Order>> GetAllOrdersAsync() => db.Orders.Include(o => o.User).OrderByDescending(o => o.CreatedAt).ToListAsync();

    public async Task<Voucher> CreateVoucherAsync(string code, decimal discountPercent, bool vipOnly, string createdByRole)
    {
        var voucher = new Voucher
        {
            Code = code.ToUpperInvariant(),
            DiscountPercent = discountPercent,
            VipOnly = vipOnly,
            CreatedByRole = createdByRole,
            ExpiredAt = DateTime.UtcNow.AddMonths(2)
        };
        db.Vouchers.Add(voucher);
        await db.SaveChangesAsync();
        return voucher;
    }

    public async Task<object> BuildReportAsync()
    {
        var revenue = await db.Orders.SumAsync(o => o.TotalVnd);
        var orderCount = await db.Orders.CountAsync();
        var vipCount = await db.Users.CountAsync(u => u.TotalSpentVnd >= 1_000_000 && !u.IsBlocked);
        var blockedCount = await db.Users.CountAsync(u => u.IsBlocked);

        return new { revenue, orderCount, vipCount, blockedCount };
    }
}
