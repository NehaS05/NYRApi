using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYR.API.Migrations
{
    /// <inheritdoc />
    public partial class MoveLocationIdToRouteStops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key and index from Routes table
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Locations_LocationId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_LocationId",
                table: "Routes");

            // Remove LocationId column from Routes table
            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Routes");

            // Add LocationId column to RouteStops table
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "RouteStops",
                type: "int",
                nullable: false,
                defaultValue: 1); // Set a default value for existing rows

            // Create index on LocationId in RouteStops
            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_LocationId",
                table: "RouteStops",
                column: "LocationId");

            // Add foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_RouteStops_Locations_LocationId",
                table: "RouteStops",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key and index from RouteStops
            migrationBuilder.DropForeignKey(
                name: "FK_RouteStops_Locations_LocationId",
                table: "RouteStops");

            migrationBuilder.DropIndex(
                name: "IX_RouteStops_LocationId",
                table: "RouteStops");

            // Remove LocationId from RouteStops
            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "RouteStops");

            // Add LocationId back to Routes
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Routes",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Create index on Routes
            migrationBuilder.CreateIndex(
                name: "IX_Routes_LocationId",
                table: "Routes",
                column: "LocationId");

            // Add foreign key back to Routes
            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Locations_LocationId",
                table: "Routes",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
