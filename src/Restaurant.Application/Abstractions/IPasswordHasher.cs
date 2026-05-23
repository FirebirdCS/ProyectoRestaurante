namespace Restaurant.Application.Abstractions;

/// <summary>
/// Servicio de cifrado de contrasenas (RNF-03: las contrasenas deben
/// almacenarse cifradas mediante un algoritmo seguro de hash).
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string hash, string password);
}
