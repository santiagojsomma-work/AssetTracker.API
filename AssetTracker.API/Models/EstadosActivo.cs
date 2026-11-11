namespace AssetTracker.API.Models;

/// <summary>
/// Catálogo de los 10 estados posibles de un activo
/// (equivalente al catálogo de estados del sistema de referencia).
/// </summary>
public static class EstadosActivo
{
    public const string Disponible = "Disponible";
    public const string Asignado = "Asignado";
    public const string EnMantenimiento = "En Mantenimiento";
    public const string EnReparacion = "En Reparación";
    public const string EnTransito = "En Tránsito";
    public const string EnPrestamo = "En Préstamo";
    public const string Obsoleto = "Obsoleto";
    public const string DadoDeBaja = "Dado de Baja";
    public const string Perdido = "Perdido";
    public const string Robado = "Robado";

    public static readonly string[] Todos =
    {
        Disponible, Asignado, EnMantenimiento, EnReparacion,
        EnTransito, EnPrestamo, Obsoleto, DadoDeBaja, Perdido, Robado
    };

    public static bool EsValido(string estado) => Todos.Contains(estado);
}