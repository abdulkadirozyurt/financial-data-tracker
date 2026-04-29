using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialDataTracker.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig05_AddEntityAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Watchlists",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Watchlists",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Watchlists",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Watchlists",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Stocks",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Stocks",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Stocks",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Stocks",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "StockQuoteSnapshots",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "StockQuoteSnapshots",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StockQuoteSnapshots",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "StockQuoteSnapshots",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Watchlists");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Watchlists");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Watchlists");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Watchlists");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StockQuoteSnapshots");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StockQuoteSnapshots");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StockQuoteSnapshots");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "StockQuoteSnapshots");
        }
    }
}
