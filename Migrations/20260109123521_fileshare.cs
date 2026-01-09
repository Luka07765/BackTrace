using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    /// <inheritdoc />
    public partial class fileshare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "Files",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ShareToken",
                table: "Files",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ShareToken",
                table: "Files");
        }
    }
}
