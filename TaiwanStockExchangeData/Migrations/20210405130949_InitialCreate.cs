using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TaiwanStockExchangeData.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Security",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CodeName = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    DividendYield = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    DividendYear = table.Column<long>(type: "INTEGER", nullable: false),
                    PriceToEarningRatio = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    PriceToBookRatio = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    FinancialStatements = table.Column<string>(type: "TEXT", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Security", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Security");
        }
    }
}
