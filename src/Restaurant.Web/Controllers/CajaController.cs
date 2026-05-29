using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Services;
using Restaurant.Domain.Enums;
using Restaurant.Web.Common;

namespace Restaurant.Web.Controllers;

[Authorize(Roles = $"{RolNombre.Cajero},{RolNombre.Administrador}")]
public class CajaController : Controller
{
    private readonly ICuentaService _cuentas;

    public CajaController(ICuentaService cuentas) => _cuentas = cuentas;

    // HU-13 / HU-14: consulta del consumo acumulado y cuenta a pagar.
    public async Task<IActionResult> Cuenta(int cuentaId)
    {
        var consumo = await _cuentas.ObtenerConsumoAsync(cuentaId);
        if (consumo is null)
        {
            TempData["Error"] = "La cuenta no existe.";
            return RedirectToAction("Index", "Mesas");
        }
        return View(consumo);
    }

    // HU-14: generacion automatica de cuenta (calcula y guarda totales).
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generar(int cuentaId)
    {
        var r = await _cuentas.GenerarCuentaAsync(cuentaId);
        TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
        return RedirectToAction(nameof(Cuenta), new { cuentaId });
    }

    // HU-15: cierre de cuenta y liberacion de mesa.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cerrar(int cuentaId)
    {
        var r = await _cuentas.CerrarCuentaAsync(cuentaId, User.UserId());
        if (!r.Ok)
        {
            TempData["Error"] = r.Mensaje;
            return RedirectToAction(nameof(Cuenta), new { cuentaId });
        }
        TempData["Ok"] = r.Mensaje;
        return RedirectToAction("Index", "Mesas");
    }
}
