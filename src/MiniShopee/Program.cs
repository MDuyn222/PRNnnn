using MiniShopee.Components;
using MiniShopee.Data;
using MiniShopee.Models;
using MiniShopee.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=minishopee.db"));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<CommerceService>();
builder.Services.AddScoped<UserGovernanceService>();
builder.Services.AddScoped<CartState>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

await SeedData.InitializeAsync(app.Services);

app.MapPost("/auth/login", async (LoginRequest request, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.FindByEmailAsync(request.Email);
    if (user is null || user.IsBlocked)
    {
        return Results.BadRequest("Tài khoản không tồn tại hoặc đã bị khóa.");
    }

    var result = await signInManager.PasswordSignInAsync(user, request.Password, true, lockoutOnFailure: false);
    return result.Succeeded ? Results.Ok() : Results.BadRequest("Sai thông tin đăng nhập.");
});

app.MapPost("/auth/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
});

app.MapPost("/auth/login-form", async (HttpContext httpContext, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) =>
{
    var form = await httpContext.Request.ReadFormAsync();
    var email = form["email"].ToString().Trim();
    var password = form["password"].ToString();

    var user = await userManager.FindByEmailAsync(email);
    if (user is null || user.IsBlocked)
    {
        return Results.Redirect("/login?error=blocked");
    }

    var result = await signInManager.PasswordSignInAsync(user, password, true, false);
    return result.Succeeded ? Results.Redirect("/") : Results.Redirect("/login?error=bad_credentials");
});

app.MapPost("/auth/register-form", async (HttpContext httpContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) =>
{
    var form = await httpContext.Request.ReadFormAsync();
    var fullName = form["fullName"].ToString().Trim();
    var email = form["email"].ToString().Trim();
    var password = form["password"].ToString();
    var confirmPassword = form["confirmPassword"].ToString();

    if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/register?error=missing_fields");
    }

    if (password != confirmPassword)
    {
        return Results.Redirect("/register?error=password_mismatch");
    }

    if (await userManager.FindByEmailAsync(email) is not null)
    {
        return Results.Redirect("/register?error=email_exists");
    }

    var user = new ApplicationUser
    {
        UserName = email,
        Email = email,
        FullName = fullName,
        EmailConfirmed = true,
        AvatarUrl = "https://placehold.co/100x100"
    };

    var createResult = await userManager.CreateAsync(user, password);
    if (!createResult.Succeeded)
    {
        return Results.Redirect("/register?error=invalid_password");
    }

    await userManager.AddToRoleAsync(user, "Customer");
    await signInManager.SignInAsync(user, isPersistent: true);
    return Results.Redirect("/");
});

app.MapPost("/auth/logout-form", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/login");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public record LoginRequest(string Email, string Password);
