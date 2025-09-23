using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    /// <inheritdoc />
    public partial class tagstesting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
