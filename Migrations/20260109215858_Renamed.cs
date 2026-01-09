using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    public partial class Renamed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_ProjectDomain_ProjectDomainId",
                table: "Folders");

            migrationBuilder.RenameTable(
                name: "ProjectDomain",
                newName: "Domains");

            migrationBuilder.RenameColumn(
                name: "ProjectDomainId",
                table: "Folders",
                newName: "DomainId");

            migrationBuilder.RenameIndex(
                name: "IX_Folders_ProjectDomainId",
                table: "Folders",
                newName: "IX_Folders_DomainId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectDomain_UserId",
                table: "Domains",
                newName: "IX_Domains_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Domains_DomainId",
                table: "Folders",
                column: "DomainId",
                principalTable: "Domains",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Domains_DomainId",
                table: "Folders");

            migrationBuilder.RenameTable(
                name: "Domains",
                newName: "ProjectDomain");

            migrationBuilder.RenameColumn(
                name: "DomainId",
                table: "Folders",
                newName: "ProjectDomainId");

            migrationBuilder.RenameIndex(
                name: "IX_Folders_DomainId",
                table: "Folders",
                newName: "IX_Folders_ProjectDomainId");

            migrationBuilder.RenameIndex(
                name: "IX_Domains_UserId",
                table: "ProjectDomain",
                newName: "IX_ProjectDomain_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_ProjectDomain_ProjectDomainId",
                table: "Folders",
                column: "ProjectDomainId",
                principalTable: "ProjectDomain",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
