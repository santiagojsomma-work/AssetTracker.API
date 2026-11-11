using AssetTracker.API.Data;
using AssetTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetTracker.API.Repositories;

public class MantenimientoRepository : IMantenimientoRepository
{
    private readonly AssetTrackerContext _context;

    public MantenimientoRepository(AssetTrackerContext context) => _context = context;

    public async Task<List<Mantenimiento>> ObtenerTodosAsync()
    {
        return await _context.Mantenimientos
            .Include(m => m.Activo)
            .OrderByDescending(m => m.FechaProgramada)
            .ToListAsync();
    }

    public async Task<Mantenimiento?> ObtenerPorIdAsync(int id)
    {
        return await _context.Mantenimientos
            .Include(m => m.Activo)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task AgregarAsync(Mantenimiento mantenimiento)
    {
        _context.Mantenimientos.Add(mantenimiento);
        await _context.SaveChangesAsync();

        mantenimiento.AsignarCodigoInterno($"MNT-{mantenimiento.Id:D4}");
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Mantenimiento mantenimiento)
    {
        _context.Mantenimientos.Update(mantenimiento);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Mantenimiento mantenimiento)
    {
        _context.Mantenimientos.Remove(mantenimiento);
        await _context.SaveChangesAsync();
    }
}