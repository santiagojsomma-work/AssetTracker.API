using AssetTracker.Web.Data;
using AssetTracker.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ====== EF Core: base de Identity (usuarios/roles) ======
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=assettracker-web.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// ====== Identity con ApplicationUser y roles (R1) ======
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ====== Razor Pages + autorización ======
builder.Services.AddRazorPages();
builder.Services.AddAuthorization();

// ====== HttpClient nombrado hacia la API ======
builder.Services.AddHttpClient("AssetTrackerApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7087/");
});

// ====== Cliente centralizado (Tanda 5) ======
builder.Services.AddScoped<IAssetTrackerApiClient, AssetTrackerApiClient>();

var app = builder.Build();

// ====== Seed de roles y usuarios al arrancar en Desarrollo ======
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await IdentitySeed.SeedAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();