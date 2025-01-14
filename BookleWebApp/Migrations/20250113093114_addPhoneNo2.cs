using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookleWebApp.Migrations
{
    /// <inheritdoc />
    public partial class addPhoneNo2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserPhoneNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPhoneNumber",
                table: "Orders");
        }
    }
}
