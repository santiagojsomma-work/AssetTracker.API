namespace AssetTracker.API.Exceptions;

/// <summary>
/// Excepción del dominio que se lanza cuando se intenta eliminar
/// un activo que todavía tiene asignaciones asociadas.
/// </summary>
public class ActivoConDependenciasException : Exception
{
    public int ActivoId { get; }
    public int CantidadAsignaciones { get; }

    public ActivoConDependenciasException(int activoId, int cantidadAsignaciones)
        : base($"No se puede eliminar el activo {activoId} porque tiene {cantidadAsignaciones} asignación(es) asociada(s).")
    {
        ActivoId = activoId;
        CantidadAsignaciones = cantidadAsignaciones;
    }
}