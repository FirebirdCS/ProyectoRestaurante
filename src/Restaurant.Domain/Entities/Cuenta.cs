using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Entities;

/// <summary>
/// Cuenta de consumo asociada a una mesa. Agrupa los pedidos
/// realizados desde su apertura hasta su cierre o anulacion.
/// </summary>
public class Cuenta
{
  public int IdCuenta { get; set; }
  public int IdMesa { get; set; }
  public int IdUsuarioApertura { get; set; }
  public int? IdUsuarioCierre { get; set; }
  public DateTime FechaApertura { get; set; }
  public DateTime? FechaCierre { get; set; }
  public string Estado { get; set; } = EstadoCuenta.Abierta;
  public decimal Subtotal { get; set; }
  public decimal Impuesto { get; set; }
  public decimal Total { get; set; }
  public string? Observaciones { get; set; }

  public Mesa Mesa { get; set; } = null!;
  public Usuario UsuarioApertura { get; set; } = null!;
  public Usuario? UsuarioCierre { get; set; }
  public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
