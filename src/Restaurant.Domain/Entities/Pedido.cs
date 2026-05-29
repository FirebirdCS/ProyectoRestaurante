using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Entities;

/// <summary>Pedido registrado por un mesero sobre una cuenta abierta.</summary>
public class Pedido
{
    public int IdPedido { get; set; }
    public int IdCuenta { get; set; }
    public int IdUsuarioMesero { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaEnvioCocina { get; set; }
    public DateTime? FechaListo { get; set; }
    public string Estado { get; set; } = EstadoPedido.Pendiente;
    public string? ObservacionGeneral { get; set; }

    public Cuenta Cuenta { get; set; } = null!;
    public Usuario UsuarioMesero { get; set; } = null!;
    public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();

    /// <summary>Indica si el pedido ya fue enviado al area de cocina.</summary>
    public bool EnviadoACocina => FechaEnvioCocina.HasValue;
}
