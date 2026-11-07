using AssetTracker.Web;
using AssetTracker.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de la base de datos para Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configuración de Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// ★ NUEVO: Redirigir a NUESTRAS páginas de login/logout en vez de las de Identity UI
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 3. Configuración de Razor Pages
builder.Services.AddRazorPages();

// 4. Configuración de HttpClient para consumir la API
builder.Services.AddHttpClient("AssetTrackerApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7087/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

// 5. Crear roles al iniciar (solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    await IdentitySeed.CrearRolesAsync(app.Services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 6. Middleware de autenticación y autorización (ORDEN IMPORTANTE)
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();