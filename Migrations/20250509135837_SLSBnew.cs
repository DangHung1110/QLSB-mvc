using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_OngPhuong.Migrations
{
    /// <inheritdoc />
    public partial class SLSBnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Bookings",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Bookings");
        }
    }
}
