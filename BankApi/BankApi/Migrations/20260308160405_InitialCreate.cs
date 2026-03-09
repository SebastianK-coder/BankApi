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
                    NumerKonta = table.Column<string>(type: "TEXT", nullable: false),
                    Wlasciciel = table.Column<string>(type: "TEXT", nullable: false),
                    Saldo = table.Column<decimal>(type: "TEXT", nullable: false),
                    Haslo = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.NumerKonta);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    DataTransakcji = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Kwota = table.Column<decimal>(type: "TEXT", nullable: false),
                    NumerKontaNadawcy = table.Column<string>(type: "TEXT", nullable: false),
                    NumerKontaOdbiorcy = table.Column<string>(type: "TEXT", nullable: false),
                    KontoBankoweNumerKonta = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.DataTransakcji);
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
