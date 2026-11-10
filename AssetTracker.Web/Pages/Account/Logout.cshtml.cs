using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AssetTracker.Web.Data;

namespace AssetTracker.Web.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(
        SignInManager<ApplicationUser> signInManager,
        ILogger<LogoutModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("Usuario cerró sesión");

        if (returnUrl != null)
        {
            return LocalRedirect(returnUrl);
        }

        // ✅ CORRECCIÓN: "/Index" en lugar de "~/Index"
        return RedirectToPage("/Index");
    }
}