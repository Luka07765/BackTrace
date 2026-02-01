using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    /// <inheritdoc />
    public partial class Role2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "Files",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_RoleId",
                table: "Files",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Roles_RoleId",
                table: "Files",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Roles_RoleId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_RoleId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Files");
        }
    }
}
