using AssetTracker.API.Models;
using System.Linq.Expressions;

namespace AssetTracker.API.Dto;

/// <summary>
/// Proyecciones reutilizables (evita Duplicated Code).
/// La misma Expression sirve para SQL y para memoria.
/// </summary>
public static class Proyecciones
{
    public static readonly Expression<Func<Activo, ActivoResumenDto>> Resumen =
        a => new ActivoResumenDto
        {
            Id = a.Id,
            CodigoInterno = a.CodigoInterno,
            Nombre = a.Nombre,
            FechaAdquisicion = a.FechaAdquisicion,
            ValorOriginal = a.ValorOriginal,
            Estado = a.Estado,
            Discriminator = a is ActivoFisico ? "ActivoFisico" : "ActivoDigital"
        };
}