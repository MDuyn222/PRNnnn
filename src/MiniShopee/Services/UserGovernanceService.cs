using MiniShopee.Data;
using MiniShopee.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MiniShopee.Services;

public class UserGovernanceService(AppDbContext db, UserManager<ApplicationUser> userManager)
{
    public async Task ApplySpendingAndVipRuleAsync(ApplicationUser user, decimal orderTotal)
    {
        user.TotalSpentVnd += orderTotal;
        if (user.TotalSpentVnd >= 1_000_000)
        {
            if (!await userManager.IsInRoleAsync(user, "VipCustomer"))
            {
                await userManager.AddToRoleAsync(user, "VipCustomer");
            }
        }

        await userManager.UpdateAsync(user);
    }

    public async Task<bool> IsVipAsync(ApplicationUser user) =>
        await userManager.IsInRoleAsync(user, "VipCustomer") || user.TotalSpentVnd >= 1_000_000;

    public async Task ReportUserAsync(string userId, string reason)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return;
        }

        user.FraudReports += 1;
        user.ReputationPoints = Math.Max(0, user.ReputationPoints - 1);
        db.UserReports.Add(new UserReport { UserId = userId, Reason = reason });

        if (user.FraudReports >= 3 || user.ReputationPoints == 0)
        {
            user.IsBlocked = true;
        }

        await db.SaveChangesAsync();
    }

    public async Task AssignStaffAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return;
        }

        if (!await userManager.IsInRoleAsync(user, "Staff"))
        {
            await userManager.AddToRoleAsync(user, "Staff");
        }
    }
}
