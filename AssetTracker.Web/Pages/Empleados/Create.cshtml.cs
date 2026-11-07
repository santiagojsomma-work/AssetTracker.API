using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using AssetTracker.Web.DTOs;

namespace AssetTracker.Web.Pages.Empleados;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _factory;
    [BindProperty] public CrearEmpleadoDto Input { get; set; } = new();

    public CreateModel(IHttpClientFactory factory) => _factory = factory;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var client = _factory.CreateClient("AssetTrackerApi");
        try
        {
            var response = await client.PostAsJsonAsync("api/empleados", Input);
            if (response.IsSuccessStatusCode) return RedirectToPage("Index");
            ModelState.AddModelError("", $"Error API: {(int)response.StatusCode}");
        }
        catch (HttpRequestException) { ModelState.AddModelError("", "No se pudo conectar con la API."); }
        return Page();
    }
}