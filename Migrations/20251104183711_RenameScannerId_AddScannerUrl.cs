using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYR.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameScannerId_AddScannerUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScannerId",
                table: "Scanners",
                newName: "SerialNo");

            migrationBuilder.RenameIndex(
                name: "IX_Scanners_ScannerId",
                table: "Scanners",
                newName: "IX_Scanners_SerialNo");

            migrationBuilder.AddColumn<string>(
                name: "ScannerUrl",
                table: "Scanners",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScannerUrl",
                table: "Scanners");

            migrationBuilder.RenameColumn(
                name: "SerialNo",
                table: "Scanners",
                newName: "ScannerId");

            migrationBuilder.RenameIndex(
                name: "IX_Scanners_SerialNo",
                table: "Scanners",
                newName: "IX_Scanners_ScannerId");
        }
    }
}
