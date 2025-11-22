using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Routes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pending");

            // Create index on Status for better query performance
            migrationBuilder.CreateIndex(
                name: "IX_Routes_Status",
                table: "Routes",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Routes_Status",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Routes");
        }
    }
}
