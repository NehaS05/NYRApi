using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationInventoryDataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationInventoryData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    VariationType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VariationValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationInventoryData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationInventoryData_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocationInventoryData_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocationInventoryData_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocationInventoryData_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationInventoryData_CreatedBy",
                table: "LocationInventoryData",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LocationInventoryData_LocationId",
                table: "LocationInventoryData",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationInventoryData_ProductId",
                table: "LocationInventoryData",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationInventoryData_UpdatedBy",
                table: "LocationInventoryData",
                column: "UpdatedBy");

            // Create a unique index for Location, Product, VariationType, and VariationValue combination
            migrationBuilder.CreateIndex(
                name: "IX_LocationInventoryData_Location_Product_Variation",
                table: "LocationInventoryData",
                columns: new[] { "LocationId", "ProductId", "VariationType", "VariationValue" },
                unique: true,
                filter: "[VariationType] IS NOT NULL AND [VariationValue] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationInventoryData");
        }
    }
}
