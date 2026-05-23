# Base de datos

Motor: **SQL Server LocalDB** (`(localdb)\MSSQLLocalDB`, base `RestauranteDb`).

| Carpeta / archivo | Contenido |
|---|---|
| `scripts/schema.sql` | Script idempotente del esquema completo, generado desde las migraciones de EF Core. |
| `seed/seed.sql` | Datos semilla de roles, mesas y menu. |
| `migrations/` | Las migraciones EF Core son la fuente de verdad y viven en `src/Restaurant.Infrastructure/Persistence/Migrations`. |

## Forma recomendada (automatica)

Al iniciar `Restaurant.Web`, `DbInitializer` aplica las migraciones pendientes
y carga los datos semilla (incluidos los usuarios, cuya contrasena se cifra
con PBKDF2-SHA256 segun RNF-03). No se requiere ningun paso manual.

## Forma manual (opcional)

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -i database/scripts/schema.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -d RestauranteDb -i database/seed/seed.sql
```

Los usuarios deben crearse desde la aplicacion para que la contrasena
quede cifrada (no se incluyen en `seed.sql` en texto plano).
