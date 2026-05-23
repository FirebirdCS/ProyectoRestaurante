/* ============================================================
   Datos semilla - Sistema de gestion de restaurante (Fase 1)
   ------------------------------------------------------------
   Roles, mesas y menu. Los USUARIOS se cargan desde la
   aplicacion (DbInitializer) porque la contrasena se almacena
   cifrada con PBKDF2-SHA256 (RNF-03) y no en texto plano.
   Ejecutar despues de database/scripts/schema.sql
   ============================================================ */

SET NOCOUNT ON;

/* ---- Roles ---- */
IF NOT EXISTS (SELECT 1 FROM Rol)
INSERT INTO Rol (nombre, descripcion, activo) VALUES
 ('administrador', 'Acceso total y administracion', 1),
 ('mesero',        'Atiende mesas y registra pedidos', 1),
 ('cocina',        'Prepara y actualiza pedidos', 1),
 ('cajero',        'Genera y cierra cuentas', 1);

/* ---- Mesas (18 mesas) ---- */
IF NOT EXISTS (SELECT 1 FROM Mesa)
BEGIN
    DECLARE @i INT = 1;
    WHILE @i <= 18
    BEGIN
        INSERT INTO Mesa (numero_mesa, capacidad, estado, ubicacion, activa)
        VALUES (@i,
                CASE WHEN @i % 3 = 0 THEN 6 ELSE 4 END,
                'DISPONIBLE',
                CASE WHEN @i <= 12 THEN 'Salon principal' ELSE 'Terraza' END,
                1);
        SET @i += 1;
    END
END

/* ---- Categorias ---- */
IF NOT EXISTS (SELECT 1 FROM CategoriaProducto)
INSERT INTO CategoriaProducto (nombre, descripcion, activa) VALUES
 ('Entradas',       'Para iniciar', 1),
 ('Platos fuertes', 'Platillos principales', 1),
 ('Bebidas',        'Frias y calientes', 1),
 ('Postres',        'Para finalizar', 1);

/* ---- Productos ---- */
IF NOT EXISTS (SELECT 1 FROM Producto)
INSERT INTO Producto (id_categoria, nombre, descripcion, precio, disponible, tiempo_preparacion_min)
SELECT c.id_categoria, p.nombre, NULL, p.precio, 1, p.tiempo
FROM (VALUES
 ('Entradas',       'Sopa de tortilla',         35.00, 10),
 ('Entradas',       'Guacamole con totopos',    45.00,  8),
 ('Entradas',       'Tostadas de pollo',        40.00, 10),
 ('Platos fuertes', 'Carne asada',              95.00, 20),
 ('Platos fuertes', 'Pollo a la plancha',       80.00, 18),
 ('Platos fuertes', 'Enchiladas suizas',        75.00, 15),
 ('Platos fuertes', 'Pescado al mojo de ajo',  110.00, 22),
 ('Bebidas',        'Agua de horchata',         20.00,  3),
 ('Bebidas',        'Refresco',                 18.00,  1),
 ('Bebidas',        'Cafe americano',           22.00,  4),
 ('Bebidas',        'Limonada',                 20.00,  3),
 ('Postres',        'Flan napolitano',          38.00,  5),
 ('Postres',        'Pastel de chocolate',      42.00,  5),
 ('Postres',        'Helado de vainilla',       30.00,  2)
) AS p(categoria, nombre, precio, tiempo)
JOIN CategoriaProducto c ON c.nombre = p.categoria;
GO
