using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookleWebApp.Migrations
{
    /// <inheritdoc />
    public partial class aeditEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserPhoneEmail",
                table: "Orders",
                newName: "UserEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "Orders",
                newName: "UserPhoneEmail");
        }
    }
}
