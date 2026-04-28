using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialDataTracker.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig03_TypeColumnAddedToStockEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Stocks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Stocks");
        }
    }
}
