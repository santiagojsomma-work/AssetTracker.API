using AssetTracker.API.Data;
using AssetTracker.API.Services;
using AssetTracker.API.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no se encontró.");

// DbContext con SQLite
builder.Services.AddDbContext<AssetTrackerContext>(options =>
    options.UseSqlite(connectionString));

// Interfaz de notificación
var proveedorNotificacion = builder.Configuration["Notificacion:Proveedor"] ?? "Consola";

builder.Services.AddScoped<IActivoRepository, ActivoRepository>();
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IAsignacionRepository, AsignacionRepository>();
builder.Services.AddScoped<IMantenimientoRepository, MantenimientoRepository>();

// Controladores + Swagger + Configuración para evitar ciclos en JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Manejo global de errores con ProblemDetails
builder.Services.AddProblemDetails();

// --- NUEVO: Configuración de CORS ---
// Permite que un frontend (Razor Pages, SPA, etc.) consuma la API
// desde un origen diferente (otro puerto o dominio).
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowRazorPages", policy =>
    {
        policy
            // Orígenes permitidos: proyecto Razor Pages (ajustar según los puertos reales)
            .WithOrigins(
                "https://localhost:7002",
                "http://localhost:5002",
                "https://localhost:5001",  // Puerto por defecto de Razor Pages HTTPS
                "http://localhost:5000"    // Puerto por defecto de Razor Pages HTTP
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
builder.Services.AddSingleton<INotificador>(proveedorNotificacion.Equals("EmailSimulado", StringComparison.OrdinalIgnoreCase)
    ? new NotificadorEmailSimulado()
    : new NotificadorConsola());
// ------------------------------------

var app = builder.Build();

// Pipeline de manejo de errores
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler();
}

app.UseHttpsRedirection();

// --- NUEVO: Habilitar CORS ---
// Debe ir antes de UseAuthorization y MapControllers
app.UseCors("AllowRazorPages");
// -----------------------------

app.UseAuthorization();
app.MapControllers();
app.Logger.LogInformation("Notificador activo: {Proveedor}", proveedorNotificacion);
app.Run();