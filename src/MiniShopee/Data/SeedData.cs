using MiniShopee.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MiniShopee.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        var context = provider.GetRequiredService<AppDbContext>();
        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

        await context.Database.EnsureCreatedAsync();

        foreach (var role in new[] { "Admin", "Staff", "Customer", "VipCustomer" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await CreateUserIfMissing(userManager, "admin@local", "Admin123$", "System Admin", "Admin");
        await CreateUserIfMissing(userManager, "staff@local", "Staff123$", "Staff One", "Staff");
        await CreateUserIfMissing(userManager, "customer@local", "Customer123$", "Nguyen Van A", "Customer");
        await CreateUserIfMissing(userManager, "vip@local", "Vip123$", "VIP User", "VipCustomer", spent: 1_500_000M);

        if (!await context.Products.AnyAsync())
        {
            context.Products.AddRange(
                new Product { Name = "Áo thun basic", Description = "Cotton 100%, form regular", PriceVnd = 150_000, Stock = 80 },
                new Product { Name = "Áo polo premium", Description = "Vải cá sấu mềm, thoáng khí", PriceVnd = 320_000, Stock = 60 },
                new Product { Name = "Áo sơ mi công sở", Description = "Chống nhăn, dễ phối đồ", PriceVnd = 420_000, Stock = 45 },
                new Product { Name = "Áo hoodie unisex", Description = "Nỉ bông dày, giữ ấm tốt", PriceVnd = 490_000, Stock = 40 },
                new Product { Name = "Áo khoác bomber", Description = "Phong cách streetwear, nhẹ", PriceVnd = 650_000, Stock = 30 }
            );
        }

        if (!await context.Vouchers.AnyAsync())
        {
            context.Vouchers.AddRange(
                new Voucher { Code = "WELCOME10", DiscountPercent = 10, VipOnly = false, CreatedByRole = "Admin", ExpiredAt = DateTime.UtcNow.AddMonths(6) },
                new Voucher { Code = "VIP20", DiscountPercent = 20, VipOnly = true, CreatedByRole = "Staff", ExpiredAt = DateTime.UtcNow.AddMonths(3) }
            );
        }

        await context.SaveChangesAsync();
    }

    private static async Task CreateUserIfMissing(UserManager<ApplicationUser> userManager, string email, string password, string fullName, string role, decimal spent = 0)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            return;
        }

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            TotalSpentVnd = spent,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
