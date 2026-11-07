namespace AssetTracker.Web.Helpers;

/// <summary>
/// Catálogo visual de los 10 estados de un activo para la UI:
/// centraliza el icono (Bootstrap Icons) y la clase CSS de color de cada estado.
/// </summary>
public static class EstadoActivoHelper
{
    public static readonly string[] Todos =
    {
        "Disponible", "Asignado", "En Mantenimiento", "En Reparación",
        "En Tránsito", "En Préstamo", "Obsoleto", "Dado de Baja",
        "Perdido", "Robado"
    };

    public static string Icono(string? estado) => estado switch
    {
        "Disponible" => "bi-check-circle-fill",
        "Asignado" => "bi-person-check-fill",
        "En Mantenimiento" => "bi-wrench-adjustable",
        "En Reparación" => "bi-tools",
        "En Tránsito" => "bi-truck",
        "En Préstamo" => "bi-box-arrow-right",
        "Obsoleto" => "bi-archive-fill",
        "Dado de Baja" => "bi-slash-circle-fill",
        "Perdido" => "bi-question-circle-fill",
        "Robado" => "bi-shield-fill-x",
        _ => "bi-tag-fill"
    };

    public static string Badge(string? estado) => estado switch
    {
        "Disponible" => "estado-disponible",
        "Asignado" => "estado-asignado",
        "En Mantenimiento" => "estado-mantenimiento",
        "En Reparación" => "estado-reparacion",
        "En Tránsito" => "estado-transito",
        "En Préstamo" => "estado-prestamo",
        "Obsoleto" => "estado-obsoleto",
        "Dado de Baja" => "estado-baja",
        "Perdido" => "estado-perdido",
        "Robado" => "estado-robado",
        _ => "estado-otro"
    };
}