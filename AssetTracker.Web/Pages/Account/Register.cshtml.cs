using AssetTracker.Web.Data;
using AssetTracker.Web.DTOs;
using AssetTracker.Web.Helpers;
using AssetTracker.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Web.Pages.Account;

/// <summary>
/// R4: alta de usuarios EXCLUSIVA del Administrador.
/// Evita el auto-registro público y la auto-escalada de privilegios.
/// Permite elegir rol, departamento y (opcional) crear la ficha de empleado en la API.
/// </summary>
[Authorize(Roles = Permisos.RolesSuperAdmin)]
public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAssetTrackerApiClient _api;

    public RegisterModel(UserManager<ApplicationUser> userManager, IAssetTrackerApiClient api)
    {
        _userManager = userManager;
        _api = api;
    }

    [BindProperty] public InputModel Input { get; set; } = new();
    public string? WarningMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmá la contraseña.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccioná un rol.")]
        public string Rol { get; set; } = Permisos.RolEmpleado;

        public string? Departamento { get; set; }

        public bool CrearFichaEmpleado { get; set; }

        [StringLength(100)] public string? NombreCompleto { get; set; }
        [StringLength(20)] public string? Documento { get; set; }
        [StringLength(50)] public string? Cargo { get; set; }
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        // Lista blanca de roles: nunca confiar en lo que viene del cliente
        string[] rolesValidos =
        {
            Permisos.RolEmpleado,
            Permisos.RolAdminInventario,
            Permisos.RolAdminDepartamento,
            Permisos.RolAdministrador
        };
        if (!rolesValidos.Contains(Input.Rol))
            ModelState.AddModelError(nameof(Input.Rol), "Rol no válido.");

        if (Input.Rol == Permisos.RolAdminDepartamento && string.IsNullOrWhiteSpace(Input.Departamento))
            ModelState.AddModelError(nameof(Input.Departamento),
                "El AdminDepartamento necesita un departamento (define su alcance).");

        if (Input.CrearFichaEmpleado && string.IsNullOrWhiteSpace(Input.NombreCompleto))
            ModelState.AddModelError(nameof(Input.NombreCompleto),
                "El nombre completo es obligatorio para crear la ficha de empleado.");

        if (!ModelState.IsValid) return Page();

        var usuario = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            EmailConfirmed = true,
            Departamento = Input.Departamento
        };

        var resultado = await _userManager.CreateAsync(usuario, Input.Password);
        if (!resultado.Succeeded)
        {
            foreach (var error in resultado.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        await _userManager.AddToRoleAsync(usuario, Input.Rol);

        // Orquestación opcional: ficha de empleado (RRHH) en la API
        if (Input.CrearFichaEmpleado)
        {
            var dto = new CrearEmpleadoDto
            {
                NombreCompleto = Input.NombreCompleto!,
                Departamento = Input.Departamento ?? "Sin departamento",
                Documento = Input.Documento,
                Cargo = Input.Cargo,
                Correo = Input.Email,
                FechaIngreso = DateTime.Today,
                Activo = true
            };

            var ficha = await _api.CrearEmpleadoAsync(dto);
            if (!ficha.Ok)
            {
                WarningMessage = $"Usuario creado, pero NO se pudo crear la ficha de empleado: {ficha.Error}";
                return Page();
            }
        }

        TempData["Exito"] = $"Usuario {Input.Email} creado con rol {Input.Rol}.";
        return RedirectToPage("Register");
    }
}