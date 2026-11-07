using AssetTracker.Web.Helpers;
using AssetTracker.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace AssetTracker.Web.Pages.Activos;

[Authorize]
public class DepreciacionModel : PageModel
{
    private readonly IHttpClientFactory _factory;

    public List<FilaDepreciacion> Filas { get; set; } = new();
    public decimal TotalValorOriginal { get; set; }
    public decimal TotalAcumulada { get; set; }
    public decimal TotalValorEnLibros { get; set; }
    public string? Error { get; set; }

    // Datos serializados para el gráfico Chart.js
    public string EtiquetasJson => JsonSerializer.Serialize(Filas.Select(f => f.Codigo));
    public string ValoresLibrosJson => JsonSerializer.Serialize(Filas.Select(f => f.ValorEnLibros));
    public string AcumuladaJson => JsonSerializer.Serialize(Filas.Select(f => f.Acumulada));

    public DepreciacionModel(IHttpClientFactory factory) => _factory = factory;

    public async Task OnGetAsync()
    {
        var client = _factory.CreateClient("AssetTrackerApi");
        try
        {
            var response = await client.GetAsync("api/activos");
            if (!response.IsSuccessStatusCode)
            {
                Error = $"La API respondió {(int)response.StatusCode}";
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var datos = doc.RootElement.GetProperty("datos").GetRawText();

            var activos = JsonSerializer.Deserialize<List<ActivoViewModel>>(datos,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            Filas = activos
                .OrderBy(a => a.CodigoInterno)
                .Select(a =>
                {
                    var d = DepreciacionHelper.Calcular(a);
                    return new FilaDepreciacion
                    {
                        Codigo = a.CodigoInterno ?? $"ACT-{a.Id:D4}",
                        Nombre = a.Nombre,
                        Tipo = a.TipoActivo,
                        FechaAdquisicion = a.FechaAdquisicion,
                        ValorOriginal = a.ValorOriginal,
                        AniosTranscurridos = d.AniosTranscurridos,
                        VidaUtilAnios = d.VidaUtilAnios,
                        DepreciacionAnual = d.DepreciacionAnual,
                        Acumulada = d.DepreciacionAcumulada,
                        ValorEnLibros = d.ValorEnLibros,
                        Porcentaje = d.PorcentajeDepreciado
                    };
                })
                .ToList();

            TotalValorOriginal = Filas.Sum(f => f.ValorOriginal);
            TotalAcumulada = Filas.Sum(f => f.Acumulada);
            TotalValorEnLibros = Filas.Sum(f => f.ValorEnLibros);
        }
        catch (HttpRequestException) { Error = "No se pudo conectar con la API."; }
        catch (JsonException) { Error = "La API devolvió un formato inesperado."; }
    }

    public class FilaDepreciacion
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public DateTime FechaAdquisicion { get; set; }
        public decimal ValorOriginal { get; set; }
        public int AniosTranscurridos { get; set; }
        public int VidaUtilAnios { get; set; }
        public decimal DepreciacionAnual { get; set; }
        public decimal Acumulada { get; set; }
        public decimal ValorEnLibros { get; set; }
        public decimal Porcentaje { get; set; }
    }
}