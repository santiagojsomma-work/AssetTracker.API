using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace AssetTracker.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    public EstadisticasDto Estadisticas { get; set; } = new();
    public string? Error { get; set; }

    public IndexModel(IHttpClientFactory factory) => _factory = factory;

    public async Task OnGetAsync()
    {
        var client = _factory.CreateClient("AssetTrackerApi");
        try
        {
            // Consumimos el endpoint que ya existe (5B)
            var response = await client.GetAsync("api/activos/estadisticas");
            if (response.IsSuccessStatusCode)
            {
                Estadisticas = await response.Content.ReadFromJsonAsync<EstadisticasDto>() ?? new();
            }
            else
            {
                Error = $"La API respondió {(int)response.StatusCode}";
            }
        }
        catch (HttpRequestException)
        {
            Error = "No se pudo conectar con la API.";
        }
    }

    public class EstadisticasDto
    {
        public int TotalActivos { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorPromedio { get; set; }
        public decimal ValorMaximo { get; set; }
        public decimal ValorMinimo { get; set; }
        public int ActivosDisponibles { get; set; }
        public int ActivosAsignados { get; set; }
        public bool HayActivosDisponibles { get; set; }
        public bool HayActivosAsignados { get; set; }
        public int EstadosUnicos { get; set; }
    }
}