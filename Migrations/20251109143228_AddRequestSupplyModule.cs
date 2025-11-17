using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestSupplyModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestSupplies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EmailTemplate = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestSupplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestSupplies_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestSupplies_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestSupplyItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestSupplyId = table.Column<int>(type: "int", nullable: false),
                    VariationId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestSupplyItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestSupplyItems_RequestSupplies_RequestSupplyId",
                        column: x => x.RequestSupplyId,
                        principalTable: "RequestSupplies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestSupplyItems_VariationOptions_VariationOptionId",
                        column: x => x.VariationId,
                        principalTable: "VariationOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_RequestSupplyItems_ProductId",
            //    table: "RequestSupplyItems",
            //    column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_RequestSupplyItems_SupplierId",
            //    table: "RequestSupplyItems",
            //    column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestSupplyItems_RequestSupplyId",
                table: "RequestSupplyItems",
                column: "RequestSupplyId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_RequestSupplyItems_VariationOptionId",
            //    table: "RequestSupplyItems",
            //    column: "VariationOptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestSupplyItems");

            migrationBuilder.DropTable(
                name: "RequestSupplies");
        }
    }
}
