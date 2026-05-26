using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Services;
using Restaurant.Domain.Enums;
using Restaurant.Web.Common;
using Restaurant.Web.Models;

namespace Restaurant.Web.Controllers;

[Authorize(Roles = $"{RolNombre.Mesero},{RolNombre.Cajero},{RolNombre.Administrador}")]
public class PedidosController : Controller
{
    private readonly IPedidoService _pedidos;

    public PedidosController(IPedidoService pedidos) => _pedidos = pedidos;

    // Detalle de mesa / pedido: cuenta + pedidos + formulario de nuevo pedido.
    public async Task<IActionResult> Detalle(int cuentaId)
    {
        var cuenta = await _pedidos.ObtenerCuentaDetalleAsync(cuentaId);
        if (cuenta is null)
        {
            TempData["Error"] = "La cuenta no existe.";
            return RedirectToAction("Index", "Mesas");
        }

        var menu = await _pedidos.MenuDisponibleAsync();
        var vm = new CuentaDetalleVM
        {
            Cuenta = cuenta,
            Nuevo = new NuevoPedidoVM
            {
                CuentaId = cuentaId,
                Lineas = menu.Select(p => new LineaMenuVM
                {
                    ProductoId = p.IdProducto,
                    Nombre = p.Nombre,
                    Categoria = p.Categoria.Nombre,
                    Precio = p.Precio
                }).ToList()
            }
        };
        return View(vm);
    }

    // HU-06: registro de pedido por mesa.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{RolNombre.Mesero},{RolNombre.Administrador}")]
    public async Task<IActionResult> Crear(NuevoPedidoVM model)
    {
        var lineas = model.Lineas
            .Where(l => l.Cantidad > 0)
            .Select(l => new LineaPedidoInput(l.ProductoId, l.Cantidad, l.Observaciones));

        var result = await _pedidos.RegistrarPedidoAsync(
            model.CuentaId, User.UserId(), lineas, model.ObservacionGeneral);

        TempData[result.Ok ? "Ok" : "Error"] = result.Mensaje;
        return RedirectToAction(nameof(Detalle), new { cuentaId = model.CuentaId });
    }

    // HU-08: envio de pedido a cocina.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = $"{RolNombre.Mesero},{RolNombre.Administrador}")]
    public async Task<IActionResult> Enviar(int pedidoId, int cuentaId)
    {
        var result = await _pedidos.EnviarACocinaAsync(pedidoId, User.UserId());
        TempData[result.Ok ? "Ok" : "Error"] = result.Mensaje;
        return RedirectToAction(nameof(Detalle), new { cuentaId });
    }
}
