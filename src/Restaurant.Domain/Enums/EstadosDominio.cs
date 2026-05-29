namespace Restaurant.Domain.Enums;

/// <summary>
/// Valores permitidos para los estados del dominio.
/// Se modelan como constantes de cadena para coincidir con las
/// restricciones CHECK definidas en el diccionario de datos.
/// </summary>
public static class EstadoMesa
{
    public const string Disponible = "DISPONIBLE";
    public const string Ocupada = "OCUPADA";
    public const string EnCobro = "EN_COBRO";

    public static readonly string[] Todos = { Disponible, Ocupada, EnCobro };
}

public static class EstadoCuenta
{
    public const string Abierta = "ABIERTA";
    public const string Cerrada = "CERRADA";
    public const string Anulada = "ANULADA";

    public static readonly string[] Todos = { Abierta, Cerrada, Anulada };
}

public static class EstadoPedido
{
    public const string Pendiente = "PENDIENTE";
    public const string EnPreparacion = "EN_PREPARACION";
    public const string Listo = "LISTO";
    public const string Cancelado = "CANCELADO";

    public static readonly string[] Todos = { Pendiente, EnPreparacion, Listo, Cancelado };
}

public static class RolNombre
{
    public const string Administrador = "administrador";
    public const string Mesero = "mesero";
    public const string Cocina = "cocina";
    public const string Cajero = "cajero";
}
