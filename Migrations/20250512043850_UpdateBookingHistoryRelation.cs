using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_OngPhuong.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingHistoryRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingHistories_Bookings_BookingId",
                table: "BookingHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingHistories_Bookings_BookingId",
                table: "BookingHistories",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingHistories_Bookings_BookingId",
                table: "BookingHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingHistories_Bookings_BookingId",
                table: "BookingHistories",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
