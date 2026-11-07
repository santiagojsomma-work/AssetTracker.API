using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using AssetTracker.Web.ViewModels;

namespace AssetTracker.Web.Pages.Asignaciones;

public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _factory;
    public AsignacionViewModel? Asignacion { get; set; }

    public DeleteModel(IHttpClientFactory factory) => _factory = factory;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var client = _factory.CreateClient("AssetTrackerApi");
        Asignacion = await client.GetFromJsonAsync<AsignacionViewModel>($"api/asignaciones/{id}");
        return Asignacion == null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var client = _factory.CreateClient("AssetTrackerApi");
        var response = await client.DeleteAsync($"api/asignaciones/{id}");
        return response.IsSuccessStatusCode ? RedirectToPage("Index") : Page();
    }
}