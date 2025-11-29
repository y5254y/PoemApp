using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoemApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Dynasty = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Biography = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WeChatId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    QQId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthorRelationships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FromAuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    ToAuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    RelationshipType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorRelationships_Authors_FromAuthorId",
                        column: x => x.FromAuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuthorRelationships_Authors_ToAuthorId",
                        column: x => x.ToAuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Poems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Background = table.Column<string>(type: "TEXT", nullable: true),
                    Translation = table.Column<string>(type: "TEXT", nullable: true),
                    Annotation = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Poems_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Annotations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PoemId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    HighlightText = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 600, nullable: false),
                    StartIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    EndIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Annotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Annotations_Poems_PoemId",
                        column: x => x.PoemId,
                        principalTable: "Poems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Annotations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Audios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PoemId = table.Column<int>(type: "INTEGER", nullable: false),
                    FileUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    UploaderId = table.Column<int>(type: "INTEGER", nullable: true),
                    UploadTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AverageRating = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audios_Poems_PoemId",
                        column: x => x.PoemId,
                        principalTable: "Poems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Audios_Users_UploaderId",
                        column: x => x.UploaderId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PoemCategories",
                columns: table => new
                {
                    PoemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoemCategories", x => new { x.PoemId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_PoemCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PoemCategories_Poems_PoemId",
                        column: x => x.PoemId,
                        principalTable: "Poems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    PoemId = table.Column<int>(type: "INTEGER", nullable: false),
                    FavoriteTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavorites", x => new { x.UserId, x.PoemId });
                    table.ForeignKey(
                        name: "FK_UserFavorites_Poems_PoemId",
                        column: x => x.PoemId,
                        principalTable: "Poems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AudioRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AudioId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    RatingTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudioRatings_Audios_AudioId",
                        column: x => x.AudioId,
                        principalTable: "Audios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AudioRatings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Annotations_PoemId",
                table: "Annotations",
                column: "PoemId");

            migrationBuilder.CreateIndex(
                name: "IX_Annotations_UserId",
                table: "Annotations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioRatings_AudioId",
                table: "AudioRatings",
                column: "AudioId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioRatings_UserId",
                table: "AudioRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Audios_PoemId",
                table: "Audios",
                column: "PoemId");

            migrationBuilder.CreateIndex(
                name: "IX_Audios_UploaderId",
                table: "Audios",
                column: "UploaderId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorRelationships_FromAuthorId",
                table: "AuthorRelationships",
                column: "FromAuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorRelationships_ToAuthorId",
                table: "AuthorRelationships",
                column: "ToAuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_PoemCategories_CategoryId",
                table: "PoemCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Poems_AuthorId",
                table: "Poems",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_PoemId",
                table: "UserFavorites",
                column: "PoemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Annotations");

            migrationBuilder.DropTable(
                name: "AudioRatings");

            migrationBuilder.DropTable(
                name: "AuthorRelationships");

            migrationBuilder.DropTable(
                name: "PoemCategories");

            migrationBuilder.DropTable(
                name: "UserFavorites");

            migrationBuilder.DropTable(
                name: "Audios");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Poems");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
