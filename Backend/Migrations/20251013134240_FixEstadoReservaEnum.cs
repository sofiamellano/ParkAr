using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixEstadoReservaEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Suscripciones",
                keyColumn: "Id",
                keyValue: 1,
                column: "Estado",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Suscripciones",
                keyColumn: "Id",
                keyValue: 2,
                column: "Estado",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Suscripciones",
                keyColumn: "Id",
                keyValue: 1,
                column: "Estado",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Suscripciones",
                keyColumn: "Id",
                keyValue: 2,
                column: "Estado",
                value: 1);
        }
    }
}
