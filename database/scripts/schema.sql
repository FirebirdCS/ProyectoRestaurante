IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE TABLE [CategoriaProducto] (
        [id_categoria] int NOT NULL IDENTITY,
        [nombre] nvarchar(50) NOT NULL,
        [descripcion] nvarchar(150) NULL,
        [activa] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_CategoriaProducto] PRIMARY KEY ([id_categoria])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE TABLE [Mesa] (
        [id_mesa] int NOT NULL IDENTITY,
        [numero_mesa] int NOT NULL,
        [capacidad] tinyint NOT NULL,
        [estado] nvarchar(20) NOT NULL DEFAULT N'DISPONIBLE',
        [ubicacion] nvarchar(50) NULL,
        [activa] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_Mesa] PRIMARY KEY ([id_mesa]),
        CONSTRAINT [CK_Mesa_capacidad] CHECK ([capacidad] BETWEEN 1 AND 20),
        CONSTRAINT [CK_Mesa_estado] CHECK ([estado] IN ('DISPONIBLE','OCUPADA','EN_COBRO')),
        CONSTRAINT [CK_Mesa_numero] CHECK ([numero_mesa] > 0)
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE TABLE [Rol] (
        [id_rol] int NOT NULL IDENTITY,
        [nombre] nvarchar(30) NOT NULL,
        [descripcion] nvarchar(120) NULL,
        [activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_Rol] PRIMARY KEY ([id_rol])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE TABLE [Producto] (
        [id_producto] int NOT NULL IDENTITY,
        [id_categoria] int NOT NULL,
        [nombre] nvarchar(80) NOT NULL,
        [descripcion] nvarchar(200) NULL,
        [precio] decimal(10,2) NOT NULL,
        [disponible] bit NOT NULL DEFAULT CAST(1 AS bit),
        [tiempo_preparacion_min] smallint NULL,
        CONSTRAINT [PK_Producto] PRIMARY KEY ([id_producto]),
        CONSTRAINT [CK_Producto_precio] CHECK ([precio] >= 0),
        CONSTRAINT [CK_Producto_tiempo] CHECK ([tiempo_preparacion_min] >= 0),
        CONSTRAINT [FK_Producto_CategoriaProducto_id_categoria] FOREIGN KEY ([id_categoria]) REFERENCES [CategoriaProducto] ([id_categoria]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE TABLE [Usuario] (
        [id_usuario] int NOT NULL IDENTITY,
        [id_rol] int NOT NULL,
        [nombres] nvarchar(60) NOT NULL,
        [apellidos] nvarchar(60) NOT NULL,
        [username] nvarchar(40) NOT NULL,
        [password_hash] nvarchar(255) NOT NULL,
        [activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [fecha_creacion] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        CONSTRAINT [PK_Usuario] PRIMARY KEY ([id_usuario]),
        CONSTRAINT [FK_Usuario_Rol_id_rol] FOREIGN KEY ([id_rol]) REFERENCES [Rol] ([id_rol]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE TABLE [Cuenta] (
        [id_cuenta] int NOT NULL IDENTITY,
        [id_mesa] int NOT NULL,
        [id_usuario_apertura] int NOT NULL,
        [id_usuario_cierre] int NULL,
        [fecha_apertura] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [fecha_cierre] datetime2 NULL,
        [estado] nvarchar(15) NOT NULL DEFAULT N'ABIERTA',
        [subtotal] decimal(10,2) NOT NULL DEFAULT 0.0,
        [impuesto] decimal(10,2) NOT NULL DEFAULT 0.0,
        [total] decimal(10,2) NOT NULL DEFAULT 0.0,
        [observaciones] nvarchar(200) NULL,
        CONSTRAINT [PK_Cuenta] PRIMARY KEY ([id_cuenta]),
        CONSTRAINT [CK_Cuenta_estado] CHECK ([estado] IN ('ABIERTA','CERRADA','ANULADA')),
        CONSTRAINT [CK_Cuenta_impuesto] CHECK ([impuesto] >= 0),
        CONSTRAINT [CK_Cuenta_subtotal] CHECK ([subtotal] >= 0),
        CONSTRAINT [CK_Cuenta_total] CHECK ([total] >= 0),
        CONSTRAINT [FK_Cuenta_Mesa_id_mesa] FOREIGN KEY ([id_mesa]) REFERENCES [Mesa] ([id_mesa]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Cuenta_Usuario_id_usuario_apertura] FOREIGN KEY ([id_usuario_apertura]) REFERENCES [Usuario] ([id_usuario]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Cuenta_Usuario_id_usuario_cierre] FOREIGN KEY ([id_usuario_cierre]) REFERENCES [Usuario] ([id_usuario]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE TABLE [Pedido] (
        [id_pedido] int NOT NULL IDENTITY,
        [id_cuenta] int NOT NULL,
        [id_usuario_mesero] int NOT NULL,
        [fecha_creacion] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
        [fecha_envio_cocina] datetime2 NULL,
        [fecha_listo] datetime2 NULL,
        [estado] nvarchar(20) NOT NULL DEFAULT N'PENDIENTE',
        [observacion_general] nvarchar(200) NULL,
        CONSTRAINT [PK_Pedido] PRIMARY KEY ([id_pedido]),
        CONSTRAINT [CK_Pedido_estado] CHECK ([estado] IN ('PENDIENTE','EN_PREPARACION','LISTO','CANCELADO')),
        CONSTRAINT [FK_Pedido_Cuenta_id_cuenta] FOREIGN KEY ([id_cuenta]) REFERENCES [Cuenta] ([id_cuenta]) ON DELETE CASCADE,
        CONSTRAINT [FK_Pedido_Usuario_id_usuario_mesero] FOREIGN KEY ([id_usuario_mesero]) REFERENCES [Usuario] ([id_usuario]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE TABLE [PedidoDetalle] (
        [id_pedido_detalle] int NOT NULL IDENTITY,
        [id_pedido] int NOT NULL,
        [id_producto] int NOT NULL,
        [cantidad] smallint NOT NULL,
        [precio_unitario] decimal(10,2) NOT NULL,
        [subtotal] decimal(10,2) NOT NULL,
        [observaciones] nvarchar(200) NULL,
        CONSTRAINT [PK_PedidoDetalle] PRIMARY KEY ([id_pedido_detalle]),
        CONSTRAINT [CK_PedidoDetalle_cantidad] CHECK ([cantidad] > 0),
        CONSTRAINT [CK_PedidoDetalle_precio] CHECK ([precio_unitario] >= 0),
        CONSTRAINT [CK_PedidoDetalle_subtotal] CHECK ([subtotal] >= 0),
        CONSTRAINT [FK_PedidoDetalle_Pedido_id_pedido] FOREIGN KEY ([id_pedido]) REFERENCES [Pedido] ([id_pedido]) ON DELETE CASCADE,
        CONSTRAINT [FK_PedidoDetalle_Producto_id_producto] FOREIGN KEY ([id_producto]) REFERENCES [Producto] ([id_producto]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CategoriaProducto_nombre] ON [CategoriaProducto] ([nombre]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_Cuenta_Estado] ON [Cuenta] ([estado]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_Cuenta_FechaCierre] ON [Cuenta] ([fecha_cierre]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_Cuenta_id_mesa] ON [Cuenta] ([id_mesa]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_Cuenta_id_usuario_apertura] ON [Cuenta] ([id_usuario_apertura]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_Cuenta_id_usuario_cierre] ON [Cuenta] ([id_usuario_cierre]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_Mesa_Estado] ON [Mesa] ([estado]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_Mesa_numero_mesa] ON [Mesa] ([numero_mesa]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_Pedido_Cuenta_Fecha] ON [Pedido] ([id_cuenta], [fecha_creacion]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_Pedido_Estado] ON [Pedido] ([estado]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_Pedido_FechaEnvioCocina] ON [Pedido] ([fecha_envio_cocina]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_Pedido_id_usuario_mesero] ON [Pedido] ([id_usuario_mesero]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_PedidoDetalle_id_producto] ON [PedidoDetalle] ([id_producto]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_PedidoDetalle_Pedido] ON [PedidoDetalle] ([id_pedido]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_Producto_Disponible] ON [Producto] ([disponible]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_Producto_id_categoria] ON [Producto] ([id_categoria]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_Producto_nombre] ON [Producto] ([nombre]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_Rol_nombre] ON [Rol] ([nombre]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE INDEX [IX_Usuario_id_rol] ON [Usuario] ([id_rol]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_Usuario_username] ON [Usuario] ([username]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260518181017_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260518181017_InitialCreate', N'6.0.36');
END;
GO

COMMIT;
GO

