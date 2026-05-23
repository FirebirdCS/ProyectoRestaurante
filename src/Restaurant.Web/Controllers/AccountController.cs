using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Services;
using Restaurant.Web.Common;
using Restaurant.Web.Models;

namespace Restaurant.Web.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IAuthService _auth;

    public AccountController(IAuthService auth) => _auth = auth;

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToHome();
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario = await _auth.ValidarCredencialesAsync(model.Username, model.Password);
        if (usuario is null)
        {
            // HU-01: credenciales incorrectas -> mensaje de error.
            ModelState.AddModelError(string.Empty, "Usuario o contrasena incorrectos.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new(ClaimTypes.Name, usuario.Username),
            new("FullName", usuario.NombreCompleto),
            new(ClaimTypes.Role, usuario.Rol.Nombre)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return RedirectToHome(usuario.Rol.Nombre);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();

    private IActionResult RedirectToHome(string? rol = null)
    {
        rol ??= User.FindFirstValue(ClaimTypes.Role);
        var (controller, action) = RoleHome.For(rol);
        return RedirectToAction(action, controller);
    }
}
