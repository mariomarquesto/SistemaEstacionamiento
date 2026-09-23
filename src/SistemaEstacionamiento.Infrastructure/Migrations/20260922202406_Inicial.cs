using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaEstacionamiento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Sectores = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lugares",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Sector = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Fila = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Posicion = table.Column<int>(type: "INTEGER", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Patente = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    HoraIngreso = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MetodoPago = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Monto = table.Column<decimal>(type: "TEXT", precision: 12, scale: 2, nullable: true),
                    EventoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lugares", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TipoVehiculo = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    LugarCodigo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Patente = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    Monto = table.Column<decimal>(type: "TEXT", precision: 12, scale: 2, nullable: false),
                    MetodoPago = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Operador = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lugares_Sector_Estado",
                table: "Lugares",
                columns: new[] { "Sector", "Estado" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropTable(
                name: "Lugares");

            migrationBuilder.DropTable(
                name: "Tickets");
        }
    }
}
