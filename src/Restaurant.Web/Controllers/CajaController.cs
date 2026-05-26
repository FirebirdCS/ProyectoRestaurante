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

    // HU-13: visualiza el consumo acumulado de la cuenta.
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

    // HU-14: genera la cuenta (subtotal, IVA, total) y deja la mesa en cobro.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generar(int cuentaId)
    {
        var r = await _cuentas.GenerarCuentaAsync(cuentaId);
        TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
        return RedirectToAction(nameof(Cuenta), new { cuentaId });
    }

    // HU-15: cierra la cuenta, libera la mesa y conserva el historico.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cerrar(int cuentaId)
    {
        var r = await _cuentas.CerrarCuentaAsync(cuentaId, User.UserId());
        TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
        if (!r.Ok)
            return RedirectToAction(nameof(Cuenta), new { cuentaId });
        return RedirectToAction("Index", "Mesas");
    }
}
