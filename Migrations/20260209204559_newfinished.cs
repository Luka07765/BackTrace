using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    /// <inheritdoc />
    public partial class newfinished : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Domains_DomainId",
                table: "Folders");

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_Folders_DomainId",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "Folders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DomainId",
                table: "Folders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Domains_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Folders_DomainId",
                table: "Folders",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_UserId_Title",
                table: "Domains",
                columns: new[] { "UserId", "Title" });

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Domains_DomainId",
                table: "Folders",
                column: "DomainId",
                principalTable: "Domains",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
