namespace AssetTracker.API.Services;

// Interfaz que define el contrato para cualquier sistema de notificación.
// El programa trabajará contra esta abstracción (R4).
public interface INotificador
{
    Task NotificarAsync(string mensaje);
}