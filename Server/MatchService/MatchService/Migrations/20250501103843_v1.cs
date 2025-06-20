using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchService.API.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Participant1Name",
                table: "Matches",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Participant2Name",
                table: "Matches",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TournamentName",
                table: "Matches",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Participant1Name",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Participant2Name",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "TournamentName",
                table: "Matches");
        }
    }
}
