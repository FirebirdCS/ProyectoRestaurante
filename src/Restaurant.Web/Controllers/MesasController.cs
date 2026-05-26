using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Services;
using Restaurant.Domain.Enums;
using Restaurant.Web.Common;
using Restaurant.Web.Models;

namespace Restaurant.Web.Controllers;

[Authorize(Roles = $"{RolNombre.Mesero},{RolNombre.Cajero},{RolNombre.Administrador}")]
public class MesasController : Controller
{
    private readonly IMesaService _mesas;

    public MesasController(IMesaService mesas) => _mesas = mesas;

    // HU-03: visualizacion del estado de las mesas.
    public async Task<IActionResult> Index()
    {
        return View(await BuildTableroAsync());
    }

    // Fragmento para refresco automatico sin recargar toda la pantalla (HU-03).
    public async Task<IActionResult> Tablero()
    {
        return PartialView("_Tablero", await BuildTableroAsync());
    }

    // HU-04: apertura de mesa.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{RolNombre.Mesero},{RolNombre.Administrador}")]
    public async Task<IActionResult> Abrir(int id)
    {
        var result = await _mesas.AbrirMesaAsync(id, User.UserId());
        if (!result.Ok)
        {
            TempData["Error"] = result.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        TempData["Ok"] = result.Mensaje;
        return RedirectToAction("Detalle", "Pedidos", new { cuentaId = result.Id });
    }

    private async Task<List<MesaCardVM>> BuildTableroAsync()
    {
        var mesas = await _mesas.ObtenerTableroAsync();
        var cards = new List<MesaCardVM>(mesas.Count);
        foreach (var mesa in mesas)
        {
            int? cuentaId = mesa.Estado == EstadoMesa.Disponible
                ? null
                : await _mesas.CuentaAbiertaDeMesaAsync(mesa.IdMesa);
            cards.Add(new MesaCardVM { Mesa = mesa, CuentaId = cuentaId });
        }
        return cards;
    }
}
