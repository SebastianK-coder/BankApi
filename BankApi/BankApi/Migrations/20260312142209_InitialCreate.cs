using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    NumerKonta = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Wlasciciel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Haslo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.NumerKonta);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kwota = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataTransakcji = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumerKontaNadawcy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumerKontaOdbiorcy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Typ = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KontoBankoweNumerKonta = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_KontoBankoweNumerKonta",
                        column: x => x.KontoBankoweNumerKonta,
                        principalTable: "Accounts",
                        principalColumn: "NumerKonta");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_KontoBankoweNumerKonta",
                table: "Transactions",
                column: "KontoBankoweNumerKonta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
