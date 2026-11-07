using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AssetTracker.Web
{
    public static class IdentitySeed
    {
        public static async Task CrearRolesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // 1. Crear los roles si no existen
            foreach (var nombre in new[] { "Usuario", "Administrador" })
            {
                if (!await roleManager.RoleExistsAsync(nombre))
                {
                    var resultado = await roleManager.CreateAsync(new IdentityRole(nombre));
                    if (!resultado.Succeeded)
                    {
                        throw new InvalidOperationException($"No se pudo crear el rol {nombre}");
                    }
                }
            }

            // ★ NUEVO: Crear usuario administrador de prueba
            const string emailAdmin = "admin@assettracker.com";
            const string passwordAdmin = "Admin123!";

            if (await userManager.FindByEmailAsync(emailAdmin) is null)
            {
                var admin = new IdentityUser
                {
                    UserName = emailAdmin,
                    Email = emailAdmin,
                    EmailConfirmed = true
                };

                var resultado = await userManager.CreateAsync(admin, passwordAdmin);

                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Administrador");
                }
            }
        }
    }
}