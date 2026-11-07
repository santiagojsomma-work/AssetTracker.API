using AssetTracker.API.Data;
using AssetTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetTracker.API.Repositories;

/// <summary>
/// Implementación de IEmpleadoRepository usando Entity Framework Core.
/// </summary>
public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly AssetTrackerContext _context;

    public EmpleadoRepository(AssetTrackerContext context)
    {
        _context = context;
    }

    public async Task<List<Empleado>> ObtenerTodosAsync()
    {
        return await _context.Empleados.ToListAsync();
    }

    public async Task<Empleado?> ObtenerPorIdAsync(int id)
    {
        return await _context.Empleados.FindAsync(id);
    }

    public async Task AgregarAsync(Empleado empleado)
    {
        _context.Empleados.Add(empleado);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Empleado empleado)
    {
        _context.Empleados.Update(empleado);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Empleado empleado)
    {
        _context.Empleados.Remove(empleado);
        await _context.SaveChangesAsync();
    }
}