using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_OngPhuong.Migrations
{
    /// <inheritdoc />
    public partial class QLSB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Fields",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Fields");
        }
    }
}
