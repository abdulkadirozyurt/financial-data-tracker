using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialDataTracker.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig02_IndexAddedToStockEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "Stocks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_Symbol",
                table: "Stocks",
                column: "Symbol",
                unique: true,
                filter: "[Symbol] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stocks_Symbol",
                table: "Stocks");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "Stocks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
