using AssetTracker.API.Models;

namespace AssetTracker.API.Repositories;

/// <summary>
/// Interfaz que define las operaciones de persistencia para asignaciones.
/// </summary>
public interface IAsignacionRepository
{
    Task<List<Asignacion>> ObtenerTodasAsync();
    Task<Asignacion?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Asignacion asignacion);
    Task EliminarAsync(Asignacion asignacion);
}