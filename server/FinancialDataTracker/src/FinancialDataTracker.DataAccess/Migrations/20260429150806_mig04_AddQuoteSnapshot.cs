using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialDataTracker.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig04_AddQuoteSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockQuoteSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quote_CurrentPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Quote_OpenPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Quote_HighPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Quote_LowPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Quote_PreviousClosePrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Quote_Change = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Quote_PercentChange = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Quote_FinnhubTimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FetchedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockQuoteSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockQuoteSnapshots_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockQuoteSnapshots_StockId_FetchedAtUtc",
                table: "StockQuoteSnapshots",
                columns: new[] { "StockId", "FetchedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_StockQuoteSnapshots_Symbol_FetchedAtUtc",
                table: "StockQuoteSnapshots",
                columns: new[] { "Symbol", "FetchedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockQuoteSnapshots");
        }
    }
}
