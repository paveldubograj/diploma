using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsService.API.Migrations
{
    /// <inheritdoc />
    public partial class ver2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "News",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Visibility",
                table: "News",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "News");
        }
    }
}
