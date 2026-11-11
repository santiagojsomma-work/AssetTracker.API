using AssetTracker.API.Models;

namespace AssetTracker.API.Repositories;

/// <summary>
/// Contrato de persistencia de activos.
/// </summary>
public interface IActivoRepository
{
    /// <summary>Listado simple sin filtros (lo consume GetActivos sin estado).</summary>
    Task<List<Activo>> ObtenerTodosAsync();

    /// <summary>Fuente consultable para filtros/orden/agregaciones (se traduce a SQL).</summary>
    IQueryable<Activo> Consultar();

    Task<Activo?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Activo activo);
    Task ActualizarAsync(Activo activo);
    Task EliminarAsync(Activo activo);

    /// <summary>Cantidad REAL de asignaciones del activo (0 = no tiene).</summary>
    Task<int> ContarAsignacionesAsync(int activoId);
}