using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    /// <inheritdoc />
    public partial class projectdomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectDomainId",
                table: "Folders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectDomain",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDomain", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectDomain_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Folders_ProjectDomainId",
                table: "Folders",
                column: "ProjectDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDomain_UserId",
                table: "ProjectDomain",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_ProjectDomain_ProjectDomainId",
                table: "Folders",
                column: "ProjectDomainId",
                principalTable: "ProjectDomain",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_ProjectDomain_ProjectDomainId",
                table: "Folders");

            migrationBuilder.DropTable(
                name: "ProjectDomain");

            migrationBuilder.DropIndex(
                name: "IX_Folders_ProjectDomainId",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "ProjectDomainId",
                table: "Folders");
        }
    }
}
