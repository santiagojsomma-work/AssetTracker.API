using Microsoft.AspNetCore.Identity;

namespace AssetTracker.Web.Data;

/// <summary>
/// Usuario de Identity extendido con el departamento al que pertenece.
/// Permite aplicar permisos con alcance por departamento (AdminDepartamento).
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? Departamento { get; set; }
}