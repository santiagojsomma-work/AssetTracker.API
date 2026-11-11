using AssetTracker.API.Models;

namespace AssetTracker.API.Repositories;

public interface IMantenimientoRepository
{
    Task<List<Mantenimiento>> ObtenerTodosAsync();
    Task<Mantenimiento?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Mantenimiento mantenimiento);
    Task ActualizarAsync(Mantenimiento mantenimiento);
    Task EliminarAsync(Mantenimiento mantenimiento);
}