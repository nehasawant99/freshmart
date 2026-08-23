using GroceryShopping.Data;
using GroceryShopping.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// SQL SERVER DATABASE
// =====================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


// =====================================
// ASP.NET CORE IDENTITY
// =====================================

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Password security
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;

    // Account lockout
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();


// =====================================
// RAZOR PAGES
// =====================================

builder.Services.AddRazorPages();


// =====================================
// SESSION
// =====================================

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();


// =====================================
// CART SERVICE
// =====================================

builder.Services.AddScoped<CartService>();


var app = builder.Build();

// =====================================
// SEED IDENTITY ROLES
// =====================================

using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

    await IdentitySeeder.SeedRolesAsync(roleManager);
}

// =====================================
// HTTP REQUEST PIPELINE
// =====================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();


// IMPORTANT:
// Authentication must come BEFORE Authorization
app.UseAuthentication();

app.UseAuthorization();


// Session
app.UseSession();


// Static files
app.MapStaticAssets();


// Razor Pages
app.MapRazorPages()
   .WithStaticAssets();

app.Run();