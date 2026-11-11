using AssetTracker.API.Models;

namespace AssetTracker.API.Repositories;

/// <summary>
/// Interfaz que define las operaciones de persistencia para empleados.
/// </summary>
public interface IEmpleadoRepository
{
    Task<List<Empleado>> ObtenerTodosAsync();
    Task<Empleado?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Empleado empleado);
    Task ActualizarAsync(Empleado empleado);
    Task EliminarAsync(Empleado empleado);
}