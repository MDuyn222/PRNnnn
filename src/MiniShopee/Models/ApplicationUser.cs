using Microsoft.AspNetCore.Identity;

namespace MiniShopee.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = "https://placehold.co/100x100";
    public decimal TotalSpentVnd { get; set; }
    public int ReputationPoints { get; set; } = 10;
    public bool IsBlocked { get; set; }
    public int FraudReports { get; set; }
}
