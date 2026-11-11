namespace AssetTracker.Web.Helpers;

/// <summary>
/// Catálogos fijos de Cargo y Departamento (paridad con el escritorio).
/// Fuente única de verdad para los combos de Empleados y de Registro de usuarios.
/// </summary>
public static class CatalogosEmpleados
{
    public static readonly string[] Cargos =
    {
        "Gerente",
        "Administrativo Contable",
        "Técnico en Sistemas",
        "Vendedor",
        "Cadete",
        "Despachante",
        "Administrador de Inventario",
        "Administrador de Departamento"
    };

    public static readonly string[] Departamentos =
    {
        "TI",
        "Ventas",
        "Contabilidad",
        "Logística",
        "Inventario"
    };
}