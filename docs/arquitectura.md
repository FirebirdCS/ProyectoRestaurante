# Notas de arquitectura y trazabilidad

Documento breve que relaciona la implementación con la especificación de la
Fase 1 y deja constancia de las decisiones tomadas para mantener el alcance
**mínimo y local**.

## Patrón

Arquitectura **en capas como monolito modular** (sección 4.1 de la Fase 1):

| Capa | Proyecto | Responsabilidad |
|------|----------|-----------------|
| Presentación | `Restaurant.Web` | MVC, autenticación por cookie, vistas Bootstrap |
| Aplicación | `Restaurant.Application` | servicios de caso de uso, abstracciones (`IAppDbContext`, `IPasswordHasher`) |
| Dominio | `Restaurant.Domain` | entidades y estados del negocio |
| Infraestructura | `Restaurant.Infrastructure` | EF Core, `RestaurantDbContext`, seguridad, seed |

La dirección de dependencias se mantiene hacia el dominio: la aplicación
define `IAppDbContext` y la infraestructura lo implementa (RNF-14,
mantenibilidad modular).

## Modelo de datos

Las 8 entidades y sus restricciones replican el **diccionario de datos**
(sección 4.3): nombres de tabla/columna, longitudes `NVARCHAR`, `CHECK`,
defaults, índices únicos, FK e índices operativos
(`IX_Mesa_Estado`, `IX_Pedido_Estado`, `IX_Cuenta_FechaCierre`, etc.).
El esquema generado se exporta en `database/scripts/schema.sql`.

## Trazabilidad de requisitos

- **RNF-03**: contraseñas con PBKDF2-SHA256 (`Pbkdf2PasswordHasher`).
- **RNF-04/05**: autorización por rol y expiración de sesión a 15 min.
- **RNF-13**: validaciones antes de guardar (cuenta abierta, producto
  disponible, consumo > 0 para cerrar).
- **HU-01…HU-15 (Must)**: implementadas como se describe en el README.

## Decisiones y desviaciones conscientes

1. **Autenticación**: la Fase 1 permite *"ASP.NET Identity o autenticación
   por roles"*. Se eligió autenticación por roles propia para **no introducir
   las tablas de Identity** y respetar el modelo `Usuario`/`Rol` del
   diccionario de datos.
2. **HU-11 — usuario que cambia el estado en cocina**: el diccionario de
   datos de `Pedido` no define una columna para el usuario de cocina, solo
   `fecha_envio_cocina` y `fecha_listo`. Se respeta el esquema documentado y
   se registran las marcas de tiempo; añadir esa columna queda para una
   evolución del modelo.
3. **Impuesto**: la Fase 1 indica subtotal/impuesto/total sin fijar tasa. Se
   aplica **IVA 12 %** (Guatemala) como constante en `CuentaService`.
4. **DevOps**: la Fase 1 propone Git Flow + Azure Pipelines para la Fase II.
   Para una entrega local mínima se usa un **historial lineal** en `main`
   con commits por incremento funcional, alineados al calendario de sprints.
5. **Pruebas**: en lugar de dos proyectos (unit + integration) se incluye un
   único proyecto de pruebas enfocado en las reglas críticas (totales de
   cuenta y transiciones de estado), suficiente para la Definition of Done.
6. **Alcance**: solo historias *Must* con UI; usuarios, menú e historial se
   cubren con datos semilla. El modelo soporta su incorporación posterior.

## Calendario de sprints (referencia)

| Sprint | Fechas | Enfoque |
|--------|--------|---------|
| 1 | 13/04 – 27/04 | login, mesas, pedidos, cocina |
| 2 | 28/04 – 12/05 | flujo operativo completo, cuenta y cierre |
| 3 | 13/05 – 27/05 | documentación y estabilización |
