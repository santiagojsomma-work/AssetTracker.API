using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;
using System.Text.Json;
using AssetTracker.Web.DTOs;
using AssetTracker.Web.ViewModels;

namespace AssetTracker.Web.Pages.Asignaciones;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    [BindProperty]
    public CrearAsignacionDto Input { get; set; } = new();

    // Listas para los selects del formulario
    public List<SelectListItem> Empleados { get; set; } = new();
    public List<SelectListItem> Activos { get; set; } = new();

    public CreateModel(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task OnGetAsync()
    {
        await CargarListasAsync();
    }

    /// <summary>
    /// Carga las listas de Empleados y Activos desde la API.
    /// Los activos se filtran para mostrar solo los que están "Disponibles".
    /// </summary>
    private async Task CargarListasAsync()
    {
        var client = _factory.CreateClient("AssetTrackerApi");

        // 1. Cargar Empleados (la API devuelve lista directa)
        try
        {
            var empleados = await client.GetFromJsonAsync<List<EmpleadoViewModel>>("api/empleados")
                ?? new List<EmpleadoViewModel>();

            Empleados = empleados
                .OrderBy(e => e.NombreCompleto)
                .Select(e => new SelectListItem(e.NombreCompleto, e.Id.ToString()))
                .ToList();
        }
        catch (HttpRequestException)
        {
            Empleados = new List<SelectListItem>();
        }

        // 2. Cargar Activos (la API devuelve { datos: [...], totalValor: ... })
        try
        {
            var response = await client.GetAsync("api/activos");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // Deserializamos usando la clase auxiliar RespuestaActivos
                var resultado = JsonSerializer.Deserialize<RespuestaActivos>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // ← CLAVE: la API devuelve "datos" en minúscula
                    });

                var activos = resultado?.Datos ?? new List<ActivoViewModel>();

                // Filtramos solo los activos disponibles para asignar
                Activos = activos
                    .Where(a => a.Estado == "Disponible")
                    .OrderBy(a => a.Nombre)
                    .Select(a => new SelectListItem(a.Nombre, a.Id.ToString()))
                    .ToList();
            }
            else
            {
                Activos = new List<SelectListItem>();
            }
        }
        catch (HttpRequestException)
        {
            Activos = new List<SelectListItem>();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // Si la validación falla, recargamos las listas antes de devolver Page()
            await CargarListasAsync();
            return Page();
        }

        var client = _factory.CreateClient("AssetTrackerApi");

        try
        {
            var response = await client.PostAsJsonAsync("api/asignaciones", Input);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            // Leer el detalle del error de la API
            var detalle = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"La API respondió {(int)response.StatusCode}: {detalle}");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "No se pudo conectar con la API.");
        }

        // Recargar las listas para que los selects no queden vacíos
        await CargarListasAsync();
        return Page();
    }

    /// <summary>
    /// Clase auxiliar para deserializar la respuesta de GET /api/activos.
    /// La API devuelve: { "datos": [...], "totalValor": 1234567 }
    /// </summary>
    private class RespuestaActivos
    {
        public List<ActivoViewModel> Datos { get; set; } = new();
        public decimal TotalValor { get; set; }
    }
}