using AssetTracker.API.Data;
using AssetTracker.API.Models;
using Microsoft.EntityFrameworkCore; // <-- Asegurate de tener este using

namespace AssetTracker.API.Repositories;

/// <summary>
/// Implementación de IAsignacionRepository usando Entity Framework Core.
/// </summary>
public class AsignacionRepository : IAsignacionRepository
{
    private readonly AssetTrackerContext _context;

    public AsignacionRepository(AssetTrackerContext context)
    {
        _context = context;
    }

    public async Task<List<Asignacion>> ObtenerTodasAsync()
    {
        // USAMOS INCLUDE PARA CARGAR LAS RELACIONES (Eager Loading)
        return await _context.Asignaciones
            .Include(a => a.Activo)
            .Include(a => a.Empleado)
            .ToListAsync();
    }

    public async Task<Asignacion?> ObtenerPorIdAsync(int id)
    {
        return await _context.Asignaciones
            .Include(a => a.Activo)
            .Include(a => a.Empleado)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AgregarAsync(Asignacion asignacion)
    {
        _context.Asignaciones.Add(asignacion);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Asignacion asignacion)
    {
        _context.Asignaciones.Remove(asignacion);
        await _context.SaveChangesAsync();
    }
}