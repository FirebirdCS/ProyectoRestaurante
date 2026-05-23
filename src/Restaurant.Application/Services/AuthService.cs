using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Abstractions;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Services;

public interface IAuthService
{
    /// <summary>Valida credenciales. Devuelve el usuario (con rol) o null.</summary>
    Task<Usuario?> ValidarCredencialesAsync(string username, string password);
}

public class AuthService : IAuthService
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _hasher;

    public AuthService(IAppDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task<Usuario?> ValidarCredencialesAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return null;

        var usuario = await _db.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Username == username);

        // Un usuario desactivado no debe poder iniciar sesion (HU-02).
        if (usuario is null || !usuario.Activo || !usuario.Rol.Activo)
            return null;

        return _hasher.Verify(usuario.PasswordHash, password) ? usuario : null;
    }
}
