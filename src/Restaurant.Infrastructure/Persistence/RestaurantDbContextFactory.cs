using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Restaurant.Infrastructure.Persistence;

/// <summary>
/// Fabrica usada por las herramientas de EF Core en tiempo de diseno
/// (dotnet ef migrations / database update). Apunta a SQL Server LocalDB.
/// </summary>
public class RestaurantDbContextFactory : IDesignTimeDbContextFactory<RestaurantDbContext>
{
    public const string LocalDbConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=RestauranteDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    public RestaurantDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<RestaurantDbContext>()
            .UseSqlServer(LocalDbConnectionString)
            .Options;
        return new RestaurantDbContext(options);
    }
}
