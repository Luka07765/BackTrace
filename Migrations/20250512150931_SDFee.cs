using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trace.Migrations
{
    /// <inheritdoc />
    public partial class SDFee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "folderPosition",
                table: "Folders",
                newName: "FolderPosition");

            migrationBuilder.RenameColumn(
                name: "filePosition",
                table: "Files",
                newName: "FilePosition");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FolderPosition",
                table: "Folders",
                newName: "folderPosition");

            migrationBuilder.RenameColumn(
                name: "FilePosition",
                table: "Files",
                newName: "filePosition");
        }
    }
}
