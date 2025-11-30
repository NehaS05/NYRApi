using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRestockAndFollowupRequestToRouteStop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FollowupRequestId",
                table: "RouteStops",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RestockRequestId",
                table: "RouteStops",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_FollowupRequestId",
                table: "RouteStops",
                column: "FollowupRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_RestockRequestId",
                table: "RouteStops",
                column: "RestockRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_RouteStops_FollowupRequests_FollowupRequestId",
                table: "RouteStops",
                column: "FollowupRequestId",
                principalTable: "FollowupRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteStops_RestockRequests_RestockRequestId",
                table: "RouteStops",
                column: "RestockRequestId",
                principalTable: "RestockRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteStops_FollowupRequests_FollowupRequestId",
                table: "RouteStops");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteStops_RestockRequests_RestockRequestId",
                table: "RouteStops");

            migrationBuilder.DropIndex(
                name: "IX_RouteStops_FollowupRequestId",
                table: "RouteStops");

            migrationBuilder.DropIndex(
                name: "IX_RouteStops_RestockRequestId",
                table: "RouteStops");

            migrationBuilder.DropColumn(
                name: "FollowupRequestId",
                table: "RouteStops");

            migrationBuilder.DropColumn(
                name: "RestockRequestId",
                table: "RouteStops");
        }
    }
}
