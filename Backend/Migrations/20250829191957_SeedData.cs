using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Lugares",
                columns: new[] { "Id", "IsDeleted", "Numero" },
                values: new object[,]
                {
                    { 1, false, 101 },
                    { 2, false, 102 }
                });

            migrationBuilder.InsertData(
                table: "Planes",
                columns: new[] { "Id", "Descripcion", "Duracion", "IsDeleted", "Nombre", "Precio" },
                values: new object[,]
                {
                    { 1, "Acceso ilimitado por un mes", 30, false, "Plan Mensual", 5000m },
                    { 2, "Pago por cada hora de uso", null, false, "Plan por Hora", 200m }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "IsDeleted", "Nombre", "Password", "TipoUsuario" },
                values: new object[,]
                {
                    { 1, "juan@example.com", false, "Juan Pérez", "hashed123", 0 },
                    { 2, "admin@example.com", false, "Admin Root", "hashed456", 1 }
                });

            migrationBuilder.InsertData(
                table: "Suscripciones",
                columns: new[] { "Id", "Estado", "FechaFin", "FechaInicio", "IsDeleted", "PlanId", "UsuarioId" },
                values: new object[,]
                {
                    { 1, 0, new DateTime(2025, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, 1 },
                    { 2, 1, new DateTime(2025, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Vehiculos",
                columns: new[] { "Id", "IsDeleted", "Patente", "TipoVehiculo", "UsuarioId" },
                values: new object[,]
                {
                    { 1, false, "ABC123", 0, 1 },
                    { 2, false, "XYZ789", 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "Pagos",
                columns: new[] { "Id", "Concepto", "Fecha", "IsDeleted", "Metodo", "Monto", "ReservaId", "SuscripcionId", "UsuarioId" },
                values: new object[] { 2, 0, new DateTime(2025, 8, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), false, 2, 5000.00m, null, 1, 1 });

            migrationBuilder.InsertData(
                table: "Reservas",
                columns: new[] { "Id", "EstadoReserva", "FechaFin", "FechaInicio", "IsDeleted", "LugarId", "UsuarioId", "VehiculoId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 8, 25, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 25, 9, 0, 0, 0, DateTimeKind.Unspecified), false, 1, 1, 1 },
                    { 2, 0, new DateTime(2025, 8, 29, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 29, 15, 0, 0, 0, DateTimeKind.Unspecified), false, 2, 1, 2 }
                });

            migrationBuilder.InsertData(
                table: "Pagos",
                columns: new[] { "Id", "Concepto", "Fecha", "IsDeleted", "Metodo", "Monto", "ReservaId", "SuscripcionId", "UsuarioId" },
                values: new object[] { 1, 1, new DateTime(2025, 8, 25, 12, 5, 0, 0, DateTimeKind.Unspecified), false, 1, 600.00m, 1, null, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Pagos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pagos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reservas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Suscripciones",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lugares",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Planes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reservas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Suscripciones",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vehiculos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lugares",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Planes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vehiculos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
