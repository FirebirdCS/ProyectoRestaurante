using Restaurant.Domain.Entities;

namespace Restaurant.Web.Models;

public class LineaMenuVM
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public short Cantidad { get; set; }
    public string? Observaciones { get; set; }
}

public class NuevoPedidoVM
{
    public int CuentaId { get; set; }
    public string? ObservacionGeneral { get; set; }
    public List<LineaMenuVM> Lineas { get; set; } = new();
}

public class CuentaDetalleVM
{
    public Cuenta Cuenta { get; set; } = null!;
    public NuevoPedidoVM Nuevo { get; set; } = new();
}
