using System.Security.Claims;

namespace AssetTracker.Web.Helpers;

/// <summary>
/// Fuente única de verdad de roles y verificaciones de permiso (R3).
/// Evita strings mágicos dispersos en PageModels y vistas.
/// </summary>
public static class Permisos
{
    public const string RolEmpleado = "Empleado";
    public const string RolAdminInventario = "AdminInventario";
    public const string RolAdminDepartamento = "AdminDepartamento";
    public const string RolAdministrador = "Administrador";

    // Combinaciones para atributos [Authorize] (deben ser constantes de compilación)
    public const string RolesInventario = RolAdminInventario + "," + RolAdministrador;
    public const string RolesEmpleados = RolAdminDepartamento + "," + RolAdministrador;
    public const string RolesSuperAdmin = RolAdministrador;

    /// <summary>Gestiona activos, asignaciones y mantenimientos.</summary>
    public static bool GestionaInventario(ClaimsPrincipal user)
        => user.IsInRole(RolAdminInventario) || user.IsInRole(RolAdministrador);

    /// <summary>Gestiona empleados (AdminDepartamento con alcance por departamento).</summary>
    public static bool GestionaEmpleados(ClaimsPrincipal user)
        => user.IsInRole(RolAdminDepartamento) || user.IsInRole(RolAdministrador);

    /// <summary>Permiso global: único que elimina activos y empleados de cualquier departamento.</summary>
    public static bool EsSuperAdmin(ClaimsPrincipal user)
        => user.IsInRole(RolAdministrador);
}