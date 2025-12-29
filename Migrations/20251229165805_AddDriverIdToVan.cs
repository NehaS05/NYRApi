using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverIdToVan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DriverId",
                table: "Vans",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vans_DriverId",
                table: "Vans",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vans_Users_DriverId",
                table: "Vans",
                column: "DriverId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vans_Users_DriverId",
                table: "Vans");

            migrationBuilder.DropIndex(
                name: "IX_Vans_DriverId",
                table: "Vans");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Vans");
        }
    }
}
