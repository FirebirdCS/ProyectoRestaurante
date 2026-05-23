using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Infrastructure.Persistence.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriaProducto",
                columns: table => new
                {
                    id_categoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaProducto", x => x.id_categoria);
                });

            migrationBuilder.CreateTable(
                name: "Mesa",
                columns: table => new
                {
                    id_mesa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numero_mesa = table.Column<int>(type: "int", nullable: false),
                    capacidad = table.Column<byte>(type: "tinyint", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "DISPONIBLE"),
                    ubicacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesa", x => x.id_mesa);
                    table.CheckConstraint("CK_Mesa_capacidad", "[capacidad] BETWEEN 1 AND 20");
                    table.CheckConstraint("CK_Mesa_estado", "[estado] IN ('DISPONIBLE','OCUPADA','EN_COBRO')");
                    table.CheckConstraint("CK_Mesa_numero", "[numero_mesa] > 0");
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    id_producto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_categoria = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    disponible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    tiempo_preparacion_min = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.id_producto);
                    table.CheckConstraint("CK_Producto_precio", "[precio] >= 0");
                    table.CheckConstraint("CK_Producto_tiempo", "[tiempo_preparacion_min] >= 0");
                    table.ForeignKey(
                        name: "FK_Producto_CategoriaProducto_id_categoria",
                        column: x => x.id_categoria,
                        principalTable: "CategoriaProducto",
                        principalColumn: "id_categoria",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_rol = table.Column<int>(type: "int", nullable: false),
                    nombres = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    apellidos = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    username = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol_id_rol",
                        column: x => x.id_rol,
                        principalTable: "Rol",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cuenta",
                columns: table => new
                {
                    id_cuenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_mesa = table.Column<int>(type: "int", nullable: false),
                    id_usuario_apertura = table.Column<int>(type: "int", nullable: false),
                    id_usuario_cierre = table.Column<int>(type: "int", nullable: true),
                    fecha_apertura = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    fecha_cierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false, defaultValue: "ABIERTA"),
                    subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    impuesto = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    total = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    observaciones = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuenta", x => x.id_cuenta);
                    table.CheckConstraint("CK_Cuenta_estado", "[estado] IN ('ABIERTA','CERRADA','ANULADA')");
                    table.CheckConstraint("CK_Cuenta_impuesto", "[impuesto] >= 0");
                    table.CheckConstraint("CK_Cuenta_subtotal", "[subtotal] >= 0");
                    table.CheckConstraint("CK_Cuenta_total", "[total] >= 0");
                    table.ForeignKey(
                        name: "FK_Cuenta_Mesa_id_mesa",
                        column: x => x.id_mesa,
                        principalTable: "Mesa",
                        principalColumn: "id_mesa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cuenta_Usuario_id_usuario_apertura",
                        column: x => x.id_usuario_apertura,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cuenta_Usuario_id_usuario_cierre",
                        column: x => x.id_usuario_cierre,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    id_pedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_cuenta = table.Column<int>(type: "int", nullable: false),
                    id_usuario_mesero = table.Column<int>(type: "int", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    fecha_envio_cocina = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fecha_listo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "PENDIENTE"),
                    observacion_general = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.id_pedido);
                    table.CheckConstraint("CK_Pedido_estado", "[estado] IN ('PENDIENTE','EN_PREPARACION','LISTO','CANCELADO')");
                    table.ForeignKey(
                        name: "FK_Pedido_Cuenta_id_cuenta",
                        column: x => x.id_cuenta,
                        principalTable: "Cuenta",
                        principalColumn: "id_cuenta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pedido_Usuario_id_usuario_mesero",
                        column: x => x.id_usuario_mesero,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PedidoDetalle",
                columns: table => new
                {
                    id_pedido_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_pedido = table.Column<int>(type: "int", nullable: false),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<short>(type: "smallint", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoDetalle", x => x.id_pedido_detalle);
                    table.CheckConstraint("CK_PedidoDetalle_cantidad", "[cantidad] > 0");
                    table.CheckConstraint("CK_PedidoDetalle_precio", "[precio_unitario] >= 0");
                    table.CheckConstraint("CK_PedidoDetalle_subtotal", "[subtotal] >= 0");
                    table.ForeignKey(
                        name: "FK_PedidoDetalle_Pedido_id_pedido",
                        column: x => x.id_pedido,
                        principalTable: "Pedido",
                        principalColumn: "id_pedido",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoDetalle_Producto_id_producto",
                        column: x => x.id_producto,
                        principalTable: "Producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaProducto_nombre",
                table: "CategoriaProducto",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_Estado",
                table: "Cuenta",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_FechaCierre",
                table: "Cuenta",
                column: "fecha_cierre");

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_id_mesa",
                table: "Cuenta",
                column: "id_mesa");

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_id_usuario_apertura",
                table: "Cuenta",
                column: "id_usuario_apertura");

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_id_usuario_cierre",
                table: "Cuenta",
                column: "id_usuario_cierre");

            migrationBuilder.CreateIndex(
                name: "IX_Mesa_Estado",
                table: "Mesa",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_Mesa_numero_mesa",
                table: "Mesa",
                column: "numero_mesa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_Cuenta_Fecha",
                table: "Pedido",
                columns: new[] { "id_cuenta", "fecha_creacion" });

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_Estado",
                table: "Pedido",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_FechaEnvioCocina",
                table: "Pedido",
                column: "fecha_envio_cocina");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_id_usuario_mesero",
                table: "Pedido",
                column: "id_usuario_mesero");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalle_id_producto",
                table: "PedidoDetalle",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalle_Pedido",
                table: "PedidoDetalle",
                column: "id_pedido");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Disponible",
                table: "Producto",
                column: "disponible");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_id_categoria",
                table: "Producto",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_nombre",
                table: "Producto",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rol_nombre",
                table: "Rol",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_id_rol",
                table: "Usuario",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_username",
                table: "Usuario",
                column: "username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidoDetalle");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Cuenta");

            migrationBuilder.DropTable(
                name: "CategoriaProducto");

            migrationBuilder.DropTable(
                name: "Mesa");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
