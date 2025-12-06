using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYR.API.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceVariationFieldsWithVariationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VariationType",
                table: "LocationInventoryData");

            migrationBuilder.DropColumn(
                name: "VariationValue",
                table: "LocationInventoryData");

            migrationBuilder.AddColumn<string>(
                name: "VariationName",
                table: "LocationInventoryData",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VariationName",
                table: "LocationInventoryData");

            migrationBuilder.AddColumn<string>(
                name: "VariationType",
                table: "LocationInventoryData",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VariationValue",
                table: "LocationInventoryData",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
