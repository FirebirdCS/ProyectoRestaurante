using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Abstractions;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.Services;

public record OperationResult(bool Ok, string Mensaje, int? Id = null)
{
    public static OperationResult Fail(string m) => new(false, m);
    public static OperationResult Success(string m, int? id = null) => new(true, m, id);
}

public interface IMesaService
{
    Task<List<Mesa>> ObtenerTableroAsync();
    Task<Mesa?> ObtenerAsync(int idMesa);
    Task<int?> CuentaAbiertaDeMesaAsync(int idMesa);
    Task<OperationResult> AbrirMesaAsync(int idMesa, int idUsuario);
}

public class MesaService : IMesaService
{
    private readonly IAppDbContext _db;

    public MesaService(IAppDbContext db) => _db = db;

    public Task<List<Mesa>> ObtenerTableroAsync() =>
        _db.Mesas.Where(m => m.Activa)
            .OrderBy(m => m.NumeroMesa)
            .AsNoTracking()
            .ToListAsync();

    public Task<Mesa?> ObtenerAsync(int idMesa) =>
        _db.Mesas.AsNoTracking().FirstOrDefaultAsync(m => m.IdMesa == idMesa);

    public async Task<int?> CuentaAbiertaDeMesaAsync(int idMesa)
    {
        var cuenta = await _db.Cuentas.AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdMesa == idMesa && c.Estado == EstadoCuenta.Abierta);
        return cuenta?.IdCuenta;
    }

    public async Task<OperationResult> AbrirMesaAsync(int idMesa, int idUsuario)
    {
        var mesa = await _db.Mesas.FirstOrDefaultAsync(m => m.IdMesa == idMesa);
        if (mesa is null || !mesa.Activa)
            return OperationResult.Fail("La mesa no existe o esta inactiva.");

        // HU-04: solo debe poder abrirse una mesa que este disponible.
        if (mesa.Estado != EstadoMesa.Disponible)
            return OperationResult.Fail($"La mesa {mesa.NumeroMesa} no esta disponible.");

        var cuenta = new Cuenta
        {
            IdMesa = mesa.IdMesa,
            IdUsuarioApertura = idUsuario,
            FechaApertura = DateTime.Now,
            Estado = EstadoCuenta.Abierta
        };
        mesa.Estado = EstadoMesa.Ocupada;

        _db.Cuentas.Add(cuenta);
        await _db.SaveChangesAsync();

        return OperationResult.Success($"Mesa {mesa.NumeroMesa} abierta.", cuenta.IdCuenta);
    }
}
