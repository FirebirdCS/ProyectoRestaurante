using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Services;
using Restaurant.Domain.Enums;

namespace Restaurant.Web.Controllers;

[Authorize(Roles = $"{RolNombre.Cocina},{RolNombre.Administrador}")]
public class CocinaController : Controller
{
    private readonly ICocinaService _cocina;

    public CocinaController(ICocinaService cocina) => _cocina = cocina;

    // HU-10: visualizacion de pedidos en cocina.
    public async Task<IActionResult> Index() => View(await _cocina.ColaAsync());

    // Refresco automatico de la cola sin recargar toda la pantalla.
    public async Task<IActionResult> Cola() => PartialView("_Cola", await _cocina.ColaAsync());

    // HU-11: pendiente -> en preparacion.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Iniciar(int pedidoId)
    {
        var r = await _cocina.IniciarPreparacionAsync(pedidoId);
        TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
        return RedirectToAction(nameof(Index));
    }

    // HU-11: en preparacion -> listo.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Listo(int pedidoId)
    {
        var r = await _cocina.MarcarListoAsync(pedidoId);
        TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
        return RedirectToAction(nameof(Index));
    }
}
