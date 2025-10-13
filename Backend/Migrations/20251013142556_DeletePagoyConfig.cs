using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class DeletePagoyConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Configuraciones",
                table: "Configuraciones");

            migrationBuilder.RenameTable(
                name: "Configuraciones",
                newName: "Configuracion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Configuracion",
                table: "Configuracion",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Configuracion",
                table: "Configuracion");

            migrationBuilder.RenameTable(
                name: "Configuracion",
                newName: "Configuraciones");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Configuraciones",
                table: "Configuraciones",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReservaId = table.Column<int>(type: "int", nullable: true),
                    SuscripcionId = table.Column<int>(type: "int", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Concepto = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Metodo = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagos_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagos_Suscripciones_SuscripcionId",
                        column: x => x.SuscripcionId,
                        principalTable: "Suscripciones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Pagos",
                columns: new[] { "Id", "Concepto", "Fecha", "IsDeleted", "Metodo", "Monto", "ReservaId", "SuscripcionId", "UsuarioId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 8, 25, 12, 5, 0, 0, DateTimeKind.Unspecified), false, 1, 600.00m, 1, null, 1 },
                    { 2, 0, new DateTime(2025, 8, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), false, 2, 5000.00m, null, 1, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_ReservaId",
                table: "Pagos",
                column: "ReservaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_SuscripcionId",
                table: "Pagos",
                column: "SuscripcionId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_UsuarioId",
                table: "Pagos",
                column: "UsuarioId");
        }
    }
}
