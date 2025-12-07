using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryOTPToRouteStop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryOTP",
                table: "RouteStops",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryOTP",
                table: "RouteStops");
        }
    }
}
