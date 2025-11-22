using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYR.API.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceCustomerNameWithCustomerIdInRouteStops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the CustomerName column
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "RouteStops");

            // Add CustomerId column
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "RouteStops",
                type: "int",
                nullable: true);

            // Create index on CustomerId
            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_CustomerId",
                table: "RouteStops",
                column: "CustomerId");

            // Add foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_RouteStops_Customers_CustomerId",
                table: "RouteStops",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key and index
            migrationBuilder.DropForeignKey(
                name: "FK_RouteStops_Customers_CustomerId",
                table: "RouteStops");

            migrationBuilder.DropIndex(
                name: "IX_RouteStops_CustomerId",
                table: "RouteStops");

            // Remove CustomerId column
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "RouteStops");

            // Add CustomerName column back
            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "RouteStops",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
