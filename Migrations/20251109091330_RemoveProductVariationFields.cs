using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYR.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductVariationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceAdjustment",
                table: "ProductVariations");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "ProductVariations");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "ProductVariations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceAdjustment",
                table: "ProductVariations",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "ProductVariations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "ProductVariations",
                type: "int",
                nullable: true);
        }
    }
}
