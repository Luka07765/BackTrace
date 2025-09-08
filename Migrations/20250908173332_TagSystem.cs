using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    /// <inheritdoc />
    public partial class TagSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpecialGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false, defaultValue: "#808080"),
                    IconId = table.Column<int>(type: "integer", nullable: false, defaultValue: 3)

                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false, defaultValue: "#808080"),
                    IconId = table.Column<int>(type: "integer", nullable: false, defaultValue: 2)

                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileTags",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileTags", x => new { x.FileId, x.TagId });
                    table.ForeignKey(
                        name: "FK_FileTags_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagSpecialGroups",
                columns: table => new
                {
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecialGroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagSpecialGroups", x => new { x.TagId, x.SpecialGroupId });
                    table.ForeignKey(
                        name: "FK_TagSpecialGroups_SpecialGroups_SpecialGroupId",
                        column: x => x.SpecialGroupId,
                        principalTable: "SpecialGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagSpecialGroups_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileTags_TagId",
                table: "FileTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagSpecialGroups_SpecialGroupId",
                table: "TagSpecialGroups",
                column: "SpecialGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileTags");

            migrationBuilder.DropTable(
                name: "TagSpecialGroups");

            migrationBuilder.DropTable(
                name: "SpecialGroups");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
