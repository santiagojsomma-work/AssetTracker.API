namespace AssetTracker.API.Services;

public class NotificadorEmailSimulado : INotificador
{
    public Task NotificarAsync(string mensaje)
    {
        // Simulación: en un entorno real, aquí se usaría un cliente SMTP o una API externa.
        Console.WriteLine($"[EMAIL SIMULADO] Para: admin@assettracker.com | Mensaje: {mensaje}");
        return Task.CompletedTask;
    }
}