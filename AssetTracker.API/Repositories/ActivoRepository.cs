using AssetTracker.API.Data;
using AssetTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetTracker.API.Repositories;

/// <summary>
/// Implementación de IActivoRepository usando Entity Framework Core.
/// </summary>
public class ActivoRepository : IActivoRepository
{
    private readonly AssetTrackerContext _context;

    public ActivoRepository(AssetTrackerContext context)
    {
        _context = context;
    }

    public async Task<List<Activo>> ObtenerTodosAsync()
    {
        return await _context.Activos.ToListAsync();
    }

    /// <summary>
    /// Fuente consultable para filtros/orden/agregaciones (se traduce a SQL).
    /// </summary>
    public IQueryable<Activo> Consultar()
    {
        return _context.Activos.AsQueryable();
    }

    public async Task<Activo?> ObtenerPorIdAsync(int id)
    {
        return await _context.Activos.FindAsync(id);
    }

    /// <summary>
    /// Doble guardado: el 1.º obtiene el Id de la base;
    /// el 2.º persiste el código interno ACT-000X (Fase A).
    /// </summary>
    public async Task AgregarAsync(Activo activo)
    {
        _context.Activos.Add(activo);
        await _context.SaveChangesAsync();

        activo.AsignarCodigoInterno($"ACT-{activo.Id:D4}");
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Activo activo)
    {
        _context.Activos.Update(activo);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Activo activo)
    {
        _context.Activos.Remove(activo);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Cantidad REAL de asignaciones del activo (0 = no tiene).
    /// Reemplaza a TieneAsignacionesAsync (fix del magic number en el 409).
    /// </summary>
    public async Task<int> ContarAsignacionesAsync(int activoId)
    {
        return await _context.Asignaciones.CountAsync(a => a.ActivoId == activoId);
    }
}