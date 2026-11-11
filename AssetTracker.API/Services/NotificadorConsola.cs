namespace AssetTracker.API.Services;

public class NotificadorConsola : INotificador
{
    public Task NotificarAsync(string mensaje)
    {
        Console.WriteLine($"[NOTIFICACIÓN CONSOLA] {mensaje}");
        return Task.CompletedTask;
    }
}