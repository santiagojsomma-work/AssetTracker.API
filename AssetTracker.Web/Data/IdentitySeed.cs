using Microsoft.AspNetCore.Identity;

namespace AssetTracker.Web.Data;

/// <summary>
/// Siembra roles y usuarios de prueba al arrancar en Desarrollo.
/// </summary>
public static class IdentitySeed
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. Roles
        string[] roles = { "Empleado", "AdminInventario", "AdminDepartamento", "Administrador" };
        foreach (var rol in roles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }

        // 2. Usuarios de prueba (uno por rol)
        await CrearUsuarioAsync(userManager,
            "admin@assettracker.com", "Administrador", null);

        await CrearUsuarioAsync(userManager,
            "inventario@assettracker.com", "AdminInventario", "Inventario");

        await CrearUsuarioAsync(userManager,
            "depto@assettracker.com", "AdminDepartamento", "TI");

        await CrearUsuarioAsync(userManager,
            "empleado@assettracker.com", "Empleado", "Ventas");
    }

    private static async Task CrearUsuarioAsync(
        UserManager<ApplicationUser> userManager,
        string email, string rol, string? departamento)
    {
        var existente = await userManager.FindByEmailAsync(email);
        if (existente != null)
        {
            // Si ya existe, solo garantizamos el rol y el departamento
            if (!await userManager.IsInRoleAsync(existente, rol))
                await userManager.AddToRoleAsync(existente, rol);
            if (existente.Departamento != departamento)
            {
                existente.Departamento = departamento;
                await userManager.UpdateAsync(existente);
            }
            return;
        }

        var usuario = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            Departamento = departamento
        };

        var resultado = await userManager.CreateAsync(usuario, "Admin123!");
        if (resultado.Succeeded)
            await userManager.AddToRoleAsync(usuario, rol);
    }
}