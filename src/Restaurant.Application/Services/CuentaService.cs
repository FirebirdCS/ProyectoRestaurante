using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Abstractions;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.Services;

/// <summary>Linea de consumo agrupada por producto.</summary>
public record ConsumoLinea(string Producto, int Cantidad, decimal PrecioUnitario, decimal Subtotal);

/// <summary>Resumen de consumo acumulado de una cuenta.</summary>
public record ConsumoCuenta(
    Cuenta Cuenta,
    IReadOnlyList<ConsumoLinea> Lineas,
    decimal Subtotal,
    decimal Impuesto,
    decimal Total);

public interface ICuentaService
{
    Task<ConsumoCuenta?> ObtenerConsumoAsync(int cuentaId);
    Task<OperationResult> GenerarCuentaAsync(int cuentaId);
    Task<OperationResult> CerrarCuentaAsync(int cuentaId, int idUsuarioCierre);
}

public class CuentaService : ICuentaService
{
    // RNF / HU-14: tasa de impuesto aplicada al consumo (IVA 12%).
    private const decimal IvaRate = 0.12m;

    private readonly IAppDbContext _db;

    public CuentaService(IAppDbContext db) => _db = db;

    // HU-13: consumo agrupado por producto, excluye pedidos cancelados.
    public async Task<ConsumoCuenta?> ObtenerConsumoAsync(int cuentaId)
    {
        var cuenta = await _db.Cuentas
            .Include(c => c.Mesa)
            .Include(c => c.Pedidos).ThenInclude(p => p.Detalles).ThenInclude(d => d.Producto)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdCuenta == cuentaId);

        if (cuenta is null) return null;

        var lineas = cuenta.Pedidos
            .Where(p => p.Estado != EstadoPedido.Cancelado)
            .SelectMany(p => p.Detalles)
            .GroupBy(d => new { d.IdProducto, d.Producto.Nombre, d.PrecioUnitario })
            .Select(g => new ConsumoLinea(
                g.Key.Nombre,
                g.Sum(d => (int)d.Cantidad),
                g.Key.PrecioUnitario,
                g.Sum(d => d.Subtotal)))
            .OrderBy(l => l.Producto)
            .ToList();

        var subtotal = lineas.Sum(l => l.Subtotal);
        var impuesto = Math.Round(subtotal * IvaRate, 2);
        var total = subtotal + impuesto;

        return new ConsumoCuenta(cuenta, lineas, subtotal, impuesto, total);
    }

    // HU-14: calcula subtotal, impuesto (IVA 12%) y total; deja la mesa en cobro.
    public async Task<OperationResult> GenerarCuentaAsync(int cuentaId)
    {
        var cuenta = await _db.Cuentas
            .Include(c => c.Mesa)
            .Include(c => c.Pedidos).ThenInclude(p => p.Detalles)
            .FirstOrDefaultAsync(c => c.IdCuenta == cuentaId);

        if (cuenta is null)
            return OperationResult.Fail("La cuenta no existe.");

        if (cuenta.Estado != EstadoCuenta.Abierta)
            return OperationResult.Fail("Solo se puede generar el cobro de una cuenta abierta.");

        var subtotal = cuenta.Pedidos
            .Where(p => p.Estado != EstadoPedido.Cancelado)
            .SelectMany(p => p.Detalles)
            .Sum(d => d.Subtotal);

        if (subtotal <= 0)
            return OperationResult.Fail("La cuenta no tiene consumo facturable.");

        cuenta.Subtotal = subtotal;
        cuenta.Impuesto = Math.Round(subtotal * IvaRate, 2);
        cuenta.Total = cuenta.Subtotal + cuenta.Impuesto;
        cuenta.Mesa.Estado = EstadoMesa.EnCobro;

        await _db.SaveChangesAsync();
        return OperationResult.Success("Cuenta generada. La mesa quedo en cobro.", cuenta.IdCuenta);
    }

    // HU-15: cierre solo con consumo; registra fecha, total y usuario;
    // libera la mesa (DISPONIBLE) y conserva la cuenta cerrada.
    public async Task<OperationResult> CerrarCuentaAsync(int cuentaId, int idUsuarioCierre)
    {
        var cuenta = await _db.Cuentas
            .Include(c => c.Mesa)
            .Include(c => c.Pedidos).ThenInclude(p => p.Detalles)
            .FirstOrDefaultAsync(c => c.IdCuenta == cuentaId);

        if (cuenta is null)
            return OperationResult.Fail("La cuenta no existe.");

        if (cuenta.Estado == EstadoCuenta.Cerrada)
            return OperationResult.Fail("La cuenta ya esta cerrada.");

        if (cuenta.Estado != EstadoCuenta.Abierta)
            return OperationResult.Fail("Solo se puede cerrar una cuenta abierta o en cobro.");

        var subtotal = cuenta.Pedidos
            .Where(p => p.Estado != EstadoPedido.Cancelado)
            .SelectMany(p => p.Detalles)
            .Sum(d => d.Subtotal);

        if (subtotal <= 0)
            return OperationResult.Fail("No se puede cerrar una cuenta sin consumo.");

        cuenta.Subtotal = subtotal;
        cuenta.Impuesto = Math.Round(subtotal * IvaRate, 2);
        cuenta.Total = cuenta.Subtotal + cuenta.Impuesto;
        cuenta.Estado = EstadoCuenta.Cerrada;
        cuenta.FechaCierre = DateTime.Now;
        cuenta.IdUsuarioCierre = idUsuarioCierre;
        cuenta.Mesa.Estado = EstadoMesa.Disponible;

        await _db.SaveChangesAsync();
        return OperationResult.Success("Cuenta cerrada. La mesa quedo disponible.", cuenta.IdCuenta);
    }
}
