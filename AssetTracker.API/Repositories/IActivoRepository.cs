using AssetTracker.API.Models;

namespace AssetTracker.API.Repositories;

/// <summary>
/// Interfaz que define las operaciones de persistencia para activos.
/// </summary>
public interface IActivoRepository
{
    Task<List<Activo>> ObtenerTodosAsync();
    Task<Activo?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Activo activo);
    Task ActualizarAsync(Activo activo);
    Task EliminarAsync(Activo activo);
    Task<bool> TieneAsignacionesAsync(int activoId);
}