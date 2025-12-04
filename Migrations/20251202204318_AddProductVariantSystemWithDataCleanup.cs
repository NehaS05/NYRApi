using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVariantSystemWithDataCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clean up existing data - delete items with ProductVariationId since we're migrating to new system
            migrationBuilder.Sql("DELETE FROM RestockRequestItems WHERE ProductVariationId IS NOT NULL");
            migrationBuilder.Sql("DELETE FROM TransferInventoryItems WHERE ProductVariationId IS NOT NULL");
            migrationBuilder.Sql("DELETE FROM VanInventoryItems WHERE ProductVariationId IS NOT NULL");
            migrationBuilder.Sql("DELETE FROM WarehouseInventories WHERE ProductVariationId IS NOT NULL");

            migrationBuilder.DropForeignKey(
                name: "FK_RestockRequestItems_ProductVariations_ProductVariationId",
                table: "RestockRequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferInventoryItems_ProductVariations_ProductVariationId",
                table: "TransferInventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_VanInventoryItems_ProductVariations_ProductVariationId",
                table: "VanInventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseInventories_ProductVariations_ProductVariationId",
                table: "WarehouseInventories");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseInventories_ProductVariationId",
                table: "WarehouseInventories");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseInventories_WarehouseId_ProductVariationId",
                table: "WarehouseInventories");

            migrationBuilder.DropIndex(
                name: "IX_VanInventoryItems_ProductVariationId",
                table: "VanInventoryItems");

            migrationBuilder.DropColumn(
                name: "ProductVariationId",
                table: "WarehouseInventories");

            migrationBuilder.DropColumn(
                name: "ProductVariationId",
                table: "VanInventoryItems");

            migrationBuilder.RenameColumn(
                name: "ProductVariationId",
                table: "TransferInventoryItems",
                newName: "ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferInventoryItems_ProductVariationId",
                table: "TransferInventoryItems",
                newName: "IX_TransferInventoryItems_ProductVariantId");

            migrationBuilder.RenameColumn(
                name: "ProductVariationId",
                table: "RestockRequestItems",
                newName: "ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_RestockRequestItems_ProductVariationId",
                table: "RestockRequestItems",
                newName: "IX_RestockRequestItems_ProductVariantId");

            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                table: "WarehouseInventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                table: "VanInventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                table: "LocationInventoryData",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    VariantName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariantAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    VariationId = table.Column<int>(type: "int", nullable: false),
                    VariationOptionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariantAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariantAttributes_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductVariantAttributes_VariationOptions_VariationOptionId",
                        column: x => x.VariationOptionId,
                        principalTable: "VariationOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariantAttributes_Variations_VariationId",
                        column: x => x.VariationId,
                        principalTable: "Variations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseInventories_ProductVariantId",
                table: "WarehouseInventories",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseInventories_WarehouseId_ProductId_ProductVariantId",
                table: "WarehouseInventories",
                columns: new[] { "WarehouseId", "ProductId", "ProductVariantId" },
                unique: true,
                filter: "[ProductVariantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VanInventoryItems_ProductVariantId",
                table: "VanInventoryItems",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationInventoryData_ProductVariantId",
                table: "LocationInventoryData",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantAttributes_ProductVariantId",
                table: "ProductVariantAttributes",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantAttributes_VariationId",
                table: "ProductVariantAttributes",
                column: "VariationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantAttributes_VariationOptionId",
                table: "ProductVariantAttributes",
                column: "VariationOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationInventoryData_ProductVariants_ProductVariantId",
                table: "LocationInventoryData",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RestockRequestItems_ProductVariants_ProductVariantId",
                table: "RestockRequestItems",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferInventoryItems_ProductVariants_ProductVariantId",
                table: "TransferInventoryItems",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VanInventoryItems_ProductVariants_ProductVariantId",
                table: "VanInventoryItems",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseInventories_ProductVariants_ProductVariantId",
                table: "WarehouseInventories",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationInventoryData_ProductVariants_ProductVariantId",
                table: "LocationInventoryData");

            migrationBuilder.DropForeignKey(
                name: "FK_RestockRequestItems_ProductVariants_ProductVariantId",
                table: "RestockRequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferInventoryItems_ProductVariants_ProductVariantId",
                table: "TransferInventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_VanInventoryItems_ProductVariants_ProductVariantId",
                table: "VanInventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseInventories_ProductVariants_ProductVariantId",
                table: "WarehouseInventories");

            migrationBuilder.DropTable(
                name: "ProductVariantAttributes");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseInventories_ProductVariantId",
                table: "WarehouseInventories");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseInventories_WarehouseId_ProductId_ProductVariantId",
                table: "WarehouseInventories");

            migrationBuilder.DropIndex(
                name: "IX_VanInventoryItems_ProductVariantId",
                table: "VanInventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_LocationInventoryData_ProductVariantId",
                table: "LocationInventoryData");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "WarehouseInventories");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "VanInventoryItems");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "LocationInventoryData");

            migrationBuilder.RenameColumn(
                name: "ProductVariantId",
                table: "TransferInventoryItems",
                newName: "ProductVariationId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferInventoryItems_ProductVariantId",
                table: "TransferInventoryItems",
                newName: "IX_TransferInventoryItems_ProductVariationId");

            migrationBuilder.RenameColumn(
                name: "ProductVariantId",
                table: "RestockRequestItems",
                newName: "ProductVariationId");

            migrationBuilder.RenameIndex(
                name: "IX_RestockRequestItems_ProductVariantId",
                table: "RestockRequestItems",
                newName: "IX_RestockRequestItems_ProductVariationId");

            migrationBuilder.AddColumn<int>(
                name: "ProductVariationId",
                table: "WarehouseInventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductVariationId",
                table: "VanInventoryItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseInventories_ProductVariationId",
                table: "WarehouseInventories",
                column: "ProductVariationId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseInventories_WarehouseId_ProductVariationId",
                table: "WarehouseInventories",
                columns: new[] { "WarehouseId", "ProductVariationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VanInventoryItems_ProductVariationId",
                table: "VanInventoryItems",
                column: "ProductVariationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RestockRequestItems_ProductVariations_ProductVariationId",
                table: "RestockRequestItems",
                column: "ProductVariationId",
                principalTable: "ProductVariations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferInventoryItems_ProductVariations_ProductVariationId",
                table: "TransferInventoryItems",
                column: "ProductVariationId",
                principalTable: "ProductVariations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VanInventoryItems_ProductVariations_ProductVariationId",
                table: "VanInventoryItems",
                column: "ProductVariationId",
                principalTable: "ProductVariations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseInventories_ProductVariations_ProductVariationId",
                table: "WarehouseInventories",
                column: "ProductVariationId",
                principalTable: "ProductVariations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
