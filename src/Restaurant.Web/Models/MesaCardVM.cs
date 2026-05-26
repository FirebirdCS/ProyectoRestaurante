using Restaurant.Domain.Entities;

namespace Restaurant.Web.Models;

public class MesaCardVM
{
    public Mesa Mesa { get; set; } = null!;
    public int? CuentaId { get; set; }
}
