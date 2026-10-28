namespace AssetTracker.API.Models;

public abstract class Activo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaAdquisicion { get; set; }
    public decimal ValorOriginal { get; set; }
    public string Estado { get; private set; } = "Disponible";

    public void CambiarEstado(string nuevoEstado)
    {
        if (string.IsNullOrWhiteSpace(nuevoEstado))
            throw new ArgumentException("El estado no puede estar vacío.");

        if (nuevoEstado != "Disponible" && nuevoEstado != "Asignado" && nuevoEstado != "En Mantenimiento")
            throw new ArgumentException("Estado no válido. Debe ser: Disponible, Asignado o En Mantenimiento.");

        Estado = nuevoEstado;
    }

    public virtual string ObtenerDetallesEspecificos()
    {
        return $"Activo: {Nombre} | Valor: {ValorOriginal:C} | Estado: {Estado}";
    }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
            throw new ArgumentException("El nombre del activo es obligatorio.");
        if (ValorOriginal <= 0)
            throw new ArgumentException("El valor del activo debe ser mayor a cero.");
        if (FechaAdquisicion > DateTime.Now)
            throw new ArgumentException("La fecha de adquisición no puede ser futura.");
    }
}