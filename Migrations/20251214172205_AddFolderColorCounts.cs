using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    /// <inheritdoc />
    public partial class AddFolderColorCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Files_FileId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_FileId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Tag");

            migrationBuilder.AddColumn<int>(
                name: "RedCount",
                table: "Folders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "YellowCount",
                table: "Folders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ColorCountDto",
                columns: table => new
                {
                    Color = table.Column<string>(type: "text", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorCountDto");

            migrationBuilder.DropColumn(
                name: "RedCount",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "YellowCount",
                table: "Folders");

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "Tag",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tag_FileId",
                table: "Tag",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Files_FileId",
                table: "Tag",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id");
        }
    }
}
