using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Abstractions;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.Services;

public record ConsumoLinea(string Producto, int Cantidad, decimal PrecioUnitario, decimal Subtotal);

public class ConsumoCuenta
{
    public int CuentaId { get; init; }
    public int MesaNumero { get; init; }
    public string EstadoCuenta { get; init; } = "";
    public List<ConsumoLinea> Lineas { get; init; } = new();
    public decimal Subtotal { get; init; }
    public decimal Impuesto { get; init; }
    public decimal Total { get; init; }
    public bool TieneConsumo => Lineas.Count > 0;
}

public interface ICuentaService
{
    /// <summary>Tasa de impuesto aplicada al subtotal (IVA Guatemala 12%).</summary>
    const decimal TasaImpuesto = 0.12m;

    Task<ConsumoCuenta?> ObtenerConsumoAsync(int cuentaId);
    Task<OperationResult> GenerarCuentaAsync(int cuentaId);
    Task<OperationResult> CerrarCuentaAsync(int cuentaId, int idUsuarioCierre);
}

public class CuentaService : ICuentaService
{
    private readonly IAppDbContext _db;

    public CuentaService(IAppDbContext db) => _db = db;

    // HU-13: consumo acumulado (excluye pedidos cancelados), agrupado por producto.
    public async Task<ConsumoCuenta?> ObtenerConsumoAsync(int cuentaId)
    {
        var cuenta = await _db.Cuentas
            .Include(c => c.Mesa)
            .Include(c => c.Pedidos.Where(p => p.Estado != EstadoPedido.Cancelado))
                .ThenInclude(p => p.Detalles).ThenInclude(d => d.Producto)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdCuenta == cuentaId);

        if (cuenta is null) return null;

        var lineas = cuenta.Pedidos
            .SelectMany(p => p.Detalles)
            .GroupBy(d => new { d.Producto.Nombre, d.PrecioUnitario })
            .Select(g => new ConsumoLinea(
                g.Key.Nombre,
                g.Sum(x => x.Cantidad),
                g.Key.PrecioUnitario,
                g.Sum(x => x.Subtotal)))
            .OrderBy(l => l.Producto)
            .ToList();

        var subtotal = lineas.Sum(l => l.Subtotal);
        var impuesto = Math.Round(subtotal * ICuentaService.TasaImpuesto, 2);

        return new ConsumoCuenta
        {
            CuentaId = cuenta.IdCuenta,
            MesaNumero = cuenta.Mesa.NumeroMesa,
            EstadoCuenta = cuenta.Estado,
            Lineas = lineas,
            Subtotal = subtotal,
            Impuesto = impuesto,
            Total = subtotal + impuesto
        };
    }

    // HU-14: generacion automatica de cuenta (calcula y guarda totales).
    public async Task<OperationResult> GenerarCuentaAsync(int cuentaId)
    {
        var cuenta = await _db.Cuentas
            .Include(c => c.Mesa)
            .Include(c => c.Pedidos.Where(p => p.Estado != EstadoPedido.Cancelado))
                .ThenInclude(p => p.Detalles)
            .FirstOrDefaultAsync(c => c.IdCuenta == cuentaId);

        if (cuenta is null) return OperationResult.Fail("La cuenta no existe.");
        if (cuenta.Estado == EstadoCuenta.Cerrada)
            return OperationResult.Fail("La cuenta ya esta cerrada.");

        var subtotal = cuenta.Pedidos.SelectMany(p => p.Detalles).Sum(d => d.Subtotal);
        if (subtotal <= 0)
            return OperationResult.Fail("La cuenta no tiene consumo registrado.");

        cuenta.Subtotal = subtotal;
        cuenta.Impuesto = Math.Round(subtotal * ICuentaService.TasaImpuesto, 2);
        cuenta.Total = cuenta.Subtotal + cuenta.Impuesto;
        cuenta.Mesa.Estado = EstadoMesa.EnCobro;

        await _db.SaveChangesAsync();
        return OperationResult.Success($"Cuenta generada. Total: {cuenta.Total:C}");
    }

    // HU-15: cierre de cuenta y liberacion de mesa.
    public async Task<OperationResult> CerrarCuentaAsync(int cuentaId, int idUsuarioCierre)
    {
        var cuenta = await _db.Cuentas
            .Include(c => c.Mesa)
            .Include(c => c.Pedidos.Where(p => p.Estado != EstadoPedido.Cancelado))
                .ThenInclude(p => p.Detalles)
            .FirstOrDefaultAsync(c => c.IdCuenta == cuentaId);

        if (cuenta is null) return OperationResult.Fail("La cuenta no existe.");
        if (cuenta.Estado == EstadoCuenta.Cerrada)
            return OperationResult.Fail("La cuenta ya fue cerrada.");

        var subtotal = cuenta.Pedidos.SelectMany(p => p.Detalles).Sum(d => d.Subtotal);
        // HU-15: solo puede cerrarse una cuenta con al menos un consumo.
        if (subtotal <= 0)
            return OperationResult.Fail("No se puede cerrar una cuenta sin consumo.");

        cuenta.Subtotal = subtotal;
        cuenta.Impuesto = Math.Round(subtotal * ICuentaService.TasaImpuesto, 2);
        cuenta.Total = cuenta.Subtotal + cuenta.Impuesto;
        cuenta.Estado = EstadoCuenta.Cerrada;
        cuenta.FechaCierre = DateTime.Now;
        cuenta.IdUsuarioCierre = idUsuarioCierre;
        cuenta.Mesa.Estado = EstadoMesa.Disponible;

        await _db.SaveChangesAsync();
        return OperationResult.Success($"Cuenta cerrada. Total cobrado: {cuenta.Total:C}");
    }
}
