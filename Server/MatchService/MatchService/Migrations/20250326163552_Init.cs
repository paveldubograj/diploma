using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchService.API.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Round = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    MatchOrder = table.Column<int>(type: "integer", nullable: false),
                    WinScore = table.Column<int>(type: "integer", nullable: true),
                    LooseScore = table.Column<int>(type: "integer", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CategoryId = table.Column<string>(type: "text", nullable: false),
                    WinnerId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Participant1Id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Participant2Id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    TournamentId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    NextMatchId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Matches_NextMatchId",
                        column: x => x.NextMatchId,
                        principalTable: "Matches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_NextMatchId",
                table: "Matches",
                column: "NextMatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches");
        }
    }
}
