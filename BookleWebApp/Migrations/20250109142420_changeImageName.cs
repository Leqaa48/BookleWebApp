using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookleWebApp.Migrations
{
    /// <inheritdoc />
    public partial class changeImageName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BookImage",
                table: "Categories",
                newName: "Image");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Categories",
                newName: "BookImage");
        }
    }
}
