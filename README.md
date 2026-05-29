# Sistema de gestión de restaurante — Fase 1 (MVP)

Aplicación web para administrar de forma integrada **mesas, pedidos, cocina y
cuenta** del restaurante ficticio *Sabor Tradicional, S.A.*, según la
especificación del Proyecto de Ingeniería de Software – Fase 1.

Esta es una implementación **mínima y local** centrada en el flujo operativo
esencial (historias de usuario *Must*); las funciones de menor prioridad se
resuelven con datos semilla en lugar de pantallas de administración.

## Stack

- **.NET 6** · ASP.NET Core MVC + Bootstrap 5
- **SQL Server LocalDB** (`(localdb)\MSSQLLocalDB`, base `RestauranteDb`)
- EF Core 6 (code-first, migración + seed automáticos al iniciar)
- Arquitectura en capas (monolito modular): `Domain` · `Application` ·
  `Infrastructure` · `Web`

## Requisitos

- SDK de **.NET 6**
- **SQL Server LocalDB** (incluido con Visual Studio o SQL Server Express)

## Cómo ejecutar

```powershell
cd src/Restaurant.Web
dotnet run
```

Al iniciar se aplican las migraciones y se cargan los datos semilla
automáticamente. Abrir la URL que muestra la consola (p. ej.
`https://localhost:7017`).

### Usuarios semilla

| Usuario   | Contraseña   | Rol           |
|-----------|--------------|---------------|
| `admin`   | `Admin123!`  | administrador |
| `mesero1` | `Mesero123!` | mesero        |
| `cocina1` | `Cocina123!` | cocina        |
| `cajero1` | `Cajero123!` | cajero        |

Las contraseñas se almacenan cifradas con **PBKDF2-SHA256** (RNF-03).

## Flujo soportado (historias *Must*)

1. **Inicio de sesión** por rol con cierre de sesión automático a los 15 min
   de inactividad (HU-01, RNF-04/05).
2. **Mesas**: tablero por estado con refresco automático y apertura de mesa
   (HU-03, HU-04).
3. **Pedidos**: registro por mesa con productos/observaciones y envío a
   cocina sin duplicar (HU-06, HU-08).
4. **Cocina**: cola por orden de llegada y transiciones
   pendiente → en preparación → listo (HU-10, HU-11, HU-12).
5. **Caja**: consumo acumulado, generación de cuenta (IVA 12 %) y cierre con
   liberación de la mesa (HU-13, HU-14, HU-15).

## Fuera de alcance en este MVP

Gestión de usuarios/menú por UI (HU-02, HU-16), modificación/cancelación de
pedidos (HU-07), consulta de estado por mesero (HU-09) e historial de cuentas
cerradas (HU-17). El modelo de datos ya soporta su incorporación posterior.

## Pruebas

```powershell
dotnet test
```

## Estructura

```
src/
  Restaurant.Domain          entidades y reglas del dominio
  Restaurant.Application      servicios de caso de uso + abstracciones
  Restaurant.Infrastructure   EF Core, persistencia, seguridad, seed
  Restaurant.Web              controladores y vistas MVC
tests/Restaurant.UnitTests    pruebas de negocio (EF Core InMemory)
database/                     schema.sql y seed.sql + README
docs/arquitectura.md          decisiones y trazabilidad con la Fase 1
```