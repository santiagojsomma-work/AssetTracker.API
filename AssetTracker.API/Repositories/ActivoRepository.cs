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

    public async Task<Activo?> ObtenerPorIdAsync(int id)
    {
        return await _context.Activos.FindAsync(id);
    }

    /// <summary>
    /// Agrega un activo y le asigna el código interno autogenerado (ACT-0001).
    /// Primer guardado: obtiene el Id de la base.
    /// Segundo guardado: persiste el código derivado del Id (único y sin huecos visibles).
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

    public async Task<bool> TieneAsignacionesAsync(int activoId)
    {
        return await _context.Asignaciones.AnyAsync(a => a.ActivoId == activoId);
    }
}