using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddVanInventoryTablesOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VanInventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VanId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VanInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VanInventories_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VanInventories_Vans_VanId",
                        column: x => x.VanId,
                        principalTable: "Vans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VanInventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VanInventoryId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductVariationId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VanInventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VanInventoryItems_ProductVariations_ProductVariationId",
                        column: x => x.ProductVariationId,
                        principalTable: "ProductVariations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VanInventoryItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VanInventoryItems_VanInventories_VanInventoryId",
                        column: x => x.VanInventoryId,
                        principalTable: "VanInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VanInventories_LocationId",
                table: "VanInventories",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_VanInventories_VanId",
                table: "VanInventories",
                column: "VanId");

            migrationBuilder.CreateIndex(
                name: "IX_VanInventoryItems_ProductId",
                table: "VanInventoryItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_VanInventoryItems_ProductVariationId",
                table: "VanInventoryItems",
                column: "ProductVariationId");

            migrationBuilder.CreateIndex(
                name: "IX_VanInventoryItems_VanInventoryId",
                table: "VanInventoryItems",
                column: "VanInventoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VanInventoryItems");

            migrationBuilder.DropTable(
                name: "VanInventories");
        }
    }
}
