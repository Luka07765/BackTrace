using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    /// <inheritdoc />
    public partial class FIXEDTAG : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagAssignment_Files_FileId",
                table: "TagAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_TagAssignment_Tag_TagId",
                table: "TagAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagAssignment",
                table: "TagAssignment");

            migrationBuilder.RenameTable(
                name: "TagAssignment",
                newName: "TagAssignments");

            migrationBuilder.RenameIndex(
                name: "IX_TagAssignment_TagId",
                table: "TagAssignments",
                newName: "IX_TagAssignments_TagId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Tag",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagAssignments",
                table: "TagAssignments",
                columns: new[] { "FileId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tag_UserId",
                table: "Tag",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_AspNetUsers_UserId",
                table: "Tag",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagAssignments_Files_FileId",
                table: "TagAssignments",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagAssignments_Tag_TagId",
                table: "TagAssignments",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_AspNetUsers_UserId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_TagAssignments_Files_FileId",
                table: "TagAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TagAssignments_Tag_TagId",
                table: "TagAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Tag_UserId",
                table: "Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagAssignments",
                table: "TagAssignments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tag");

            migrationBuilder.RenameTable(
                name: "TagAssignments",
                newName: "TagAssignment");

            migrationBuilder.RenameIndex(
                name: "IX_TagAssignments_TagId",
                table: "TagAssignment",
                newName: "IX_TagAssignment_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagAssignment",
                table: "TagAssignment",
                columns: new[] { "FileId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TagAssignment_Files_FileId",
                table: "TagAssignment",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagAssignment_Tag_TagId",
                table: "TagAssignment",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
