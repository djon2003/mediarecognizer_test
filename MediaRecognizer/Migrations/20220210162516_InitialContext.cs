using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace com.cyberinternauts.all.MediaRecognizer.Migrations
{
    public partial class InitialContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MetaMovies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    StartYear = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndYear = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Genres = table.Column<string>(type: "TEXT", nullable: true),
                    MovieType = table.Column<string>(type: "TEXT", nullable: true),
                    MetaSource = table.Column<string>(type: "TEXT", nullable: true),
                    ExternalId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaMovies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    StartYear = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndYear = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Genres = table.Column<string>(type: "TEXT", nullable: true),
                    MovieType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaTitles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    Region = table.Column<string>(type: "TEXT", nullable: true),
                    MetaMovieId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaTitles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetaTitles_MetaMovies_MetaMovieId",
                        column: x => x.MetaMovieId,
                        principalTable: "MetaMovies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Titles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    Region = table.Column<string>(type: "TEXT", nullable: true),
                    MovieId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Titles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Titles_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetaMovies_ExternalId_MetaSource",
                table: "MetaMovies",
                columns: new[] { "ExternalId", "MetaSource" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetaMovies_MetaSource",
                table: "MetaMovies",
                column: "MetaSource");

            migrationBuilder.CreateIndex(
                name: "IX_MetaMovies_Title",
                table: "MetaMovies",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_MetaMovies_TotalMinutes",
                table: "MetaMovies",
                column: "TotalMinutes");

            migrationBuilder.CreateIndex(
                name: "IX_MetaTitles_MetaMovieId",
                table: "MetaTitles",
                column: "MetaMovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Title",
                table: "Movies",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Titles_MovieId",
                table: "Titles",
                column: "MovieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetaTitles");

            migrationBuilder.DropTable(
                name: "Titles");

            migrationBuilder.DropTable(
                name: "MetaMovies");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
