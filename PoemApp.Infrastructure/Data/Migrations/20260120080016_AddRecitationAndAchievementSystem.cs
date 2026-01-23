using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PoemApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecitationAndAchievementSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Annotations_Poems_PoemId",
                table: "Annotations");

            migrationBuilder.DropForeignKey(
                name: "FK_Annotations_Users_UserId",
                table: "Annotations");

            migrationBuilder.DropForeignKey(
                name: "FK_AudioRatings_Audios_AudioId",
                table: "AudioRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_AudioRatings_Users_UserId",
                table: "AudioRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_Audios_Poems_PoemId",
                table: "Audios");

            migrationBuilder.DropForeignKey(
                name: "FK_Audios_Users_UploaderId",
                table: "Audios");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorRelationships_Authors_FromAuthorId",
                table: "AuthorRelationships");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorRelationships_Authors_ToAuthorId",
                table: "AuthorRelationships");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_PoemCategories_Categories_CategoryId",
                table: "PoemCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PoemCategories_Poems_PoemId",
                table: "PoemCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Poems_Authors_AuthorId",
                table: "Poems");

            migrationBuilder.DropForeignKey(
                name: "FK_PointsRecords_Users_UserId",
                table: "PointsRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Authors_AuthorId",
                table: "Quotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Poems_PoemId",
                table: "Quotes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFavorites_Poems_PoemId",
                table: "UserFavorites");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFavorites_Users_UserId",
                table: "UserFavorites");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuoteFavorites_Quotes_QuoteId",
                table: "UserQuoteFavorites");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuoteFavorites_Users_UserId",
                table: "UserQuoteFavorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserQuoteFavorites",
                table: "UserQuoteFavorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFavorites",
                table: "UserFavorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quotes",
                table: "Quotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PointsRecords",
                table: "PointsRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Poems",
                table: "Poems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PoemCategories",
                table: "PoemCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authors",
                table: "Authors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorRelationships",
                table: "AuthorRelationships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Audios",
                table: "Audios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AudioRatings",
                table: "AudioRatings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Annotations",
                table: "Annotations");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "UserQuoteFavorites",
                newName: "userquotefavorites");

            migrationBuilder.RenameTable(
                name: "UserFavorites",
                newName: "userfavorites");

            migrationBuilder.RenameTable(
                name: "Quotes",
                newName: "quotes");

            migrationBuilder.RenameTable(
                name: "PointsRecords",
                newName: "pointsrecords");

            migrationBuilder.RenameTable(
                name: "Poems",
                newName: "poems");

            migrationBuilder.RenameTable(
                name: "PoemCategories",
                newName: "poemcategories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "categories");

            migrationBuilder.RenameTable(
                name: "Authors",
                newName: "authors");

            migrationBuilder.RenameTable(
                name: "AuthorRelationships",
                newName: "authorrelationships");

            migrationBuilder.RenameTable(
                name: "Audios",
                newName: "audios");

            migrationBuilder.RenameTable(
                name: "AudioRatings",
                newName: "audioratings");

            migrationBuilder.RenameTable(
                name: "Annotations",
                newName: "annotations");

            migrationBuilder.RenameIndex(
                name: "IX_UserQuoteFavorites_QuoteId",
                table: "userquotefavorites",
                newName: "IX_userquotefavorites_QuoteId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFavorites_PoemId",
                table: "userfavorites",
                newName: "IX_userfavorites_PoemId");

            migrationBuilder.RenameIndex(
                name: "IX_Quotes_PoemId",
                table: "quotes",
                newName: "IX_quotes_PoemId");

            migrationBuilder.RenameIndex(
                name: "IX_Quotes_AuthorId",
                table: "quotes",
                newName: "IX_quotes_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_PointsRecords_UserId",
                table: "pointsrecords",
                newName: "IX_pointsrecords_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Poems_AuthorId",
                table: "poems",
                newName: "IX_poems_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_PoemCategories_CategoryId",
                table: "poemcategories",
                newName: "IX_poemcategories_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_ParentId",
                table: "categories",
                newName: "IX_categories_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorRelationships_ToAuthorId",
                table: "authorrelationships",
                newName: "IX_authorrelationships_ToAuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorRelationships_FromAuthorId",
                table: "authorrelationships",
                newName: "IX_authorrelationships_FromAuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Audios_UploaderId",
                table: "audios",
                newName: "IX_audios_UploaderId");

            migrationBuilder.RenameIndex(
                name: "IX_Audios_PoemId",
                table: "audios",
                newName: "IX_audios_PoemId");

            migrationBuilder.RenameIndex(
                name: "IX_AudioRatings_UserId",
                table: "audioratings",
                newName: "IX_audioratings_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AudioRatings_AudioId",
                table: "audioratings",
                newName: "IX_audioratings_AudioId");

            migrationBuilder.RenameIndex(
                name: "IX_Annotations_UserId",
                table: "annotations",
                newName: "IX_annotations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Annotations_PoemId",
                table: "annotations",
                newName: "IX_annotations_PoemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userquotefavorites",
                table: "userquotefavorites",
                columns: new[] { "UserId", "QuoteId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_userfavorites",
                table: "userfavorites",
                columns: new[] { "UserId", "PoemId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_quotes",
                table: "quotes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pointsrecords",
                table: "pointsrecords",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_poems",
                table: "poems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_poemcategories",
                table: "poemcategories",
                columns: new[] { "PoemId", "CategoryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_categories",
                table: "categories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_authors",
                table: "authors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_authorrelationships",
                table: "authorrelationships",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_audios",
                table: "audios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_audioratings",
                table: "audioratings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_annotations",
                table: "annotations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "achievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IconUrl = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    TargetValue = table.Column<int>(type: "int", nullable: false),
                    RewardPoints = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsHidden = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievements", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "userrecitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PoemId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FirstRecitationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastReviewTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    Proficiency = table.Column<int>(type: "int", nullable: false),
                    NextReviewTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userrecitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userrecitations_poems_PoemId",
                        column: x => x.PoemId,
                        principalTable: "poems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userrecitations_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "userachievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AchievementId = table.Column<int>(type: "int", nullable: false),
                    AchievedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CurrentValue = table.Column<int>(type: "int", nullable: false),
                    RewardClaimed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RewardClaimedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userachievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userachievements_achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userachievements_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "recitationreviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserRecitationId = table.Column<int>(type: "int", nullable: false),
                    ScheduledTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ActualReviewTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewRound = table.Column<int>(type: "int", nullable: false),
                    QualityRating = table.Column<int>(type: "int", nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    ReminderSent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReminderSentTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recitationreviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_recitationreviews_userrecitations_UserRecitationId",
                        column: x => x.UserRecitationId,
                        principalTable: "userrecitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_recitationreviews_UserRecitationId",
                table: "recitationreviews",
                column: "UserRecitationId");

            migrationBuilder.CreateIndex(
                name: "IX_userachievements_AchievementId",
                table: "userachievements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "UX_UserAchievement_User_Achievement",
                table: "userachievements",
                columns: new[] { "UserId", "AchievementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_userrecitations_PoemId",
                table: "userrecitations",
                column: "PoemId");

            migrationBuilder.CreateIndex(
                name: "IX_userrecitations_UserId",
                table: "userrecitations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_annotations_poems_PoemId",
                table: "annotations",
                column: "PoemId",
                principalTable: "poems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_annotations_users_UserId",
                table: "annotations",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_audioratings_audios_AudioId",
                table: "audioratings",
                column: "AudioId",
                principalTable: "audios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_audioratings_users_UserId",
                table: "audioratings",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_audios_poems_PoemId",
                table: "audios",
                column: "PoemId",
                principalTable: "poems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_audios_users_UploaderId",
                table: "audios",
                column: "UploaderId",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_authorrelationships_authors_FromAuthorId",
                table: "authorrelationships",
                column: "FromAuthorId",
                principalTable: "authors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_authorrelationships_authors_ToAuthorId",
                table: "authorrelationships",
                column: "ToAuthorId",
                principalTable: "authors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categories_ParentId",
                table: "categories",
                column: "ParentId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_poemcategories_categories_CategoryId",
                table: "poemcategories",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_poemcategories_poems_PoemId",
                table: "poemcategories",
                column: "PoemId",
                principalTable: "poems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_poems_authors_AuthorId",
                table: "poems",
                column: "AuthorId",
                principalTable: "authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_pointsrecords_users_UserId",
                table: "pointsrecords",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_quotes_authors_AuthorId",
                table: "quotes",
                column: "AuthorId",
                principalTable: "authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_quotes_poems_PoemId",
                table: "quotes",
                column: "PoemId",
                principalTable: "poems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_userfavorites_poems_PoemId",
                table: "userfavorites",
                column: "PoemId",
                principalTable: "poems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userfavorites_users_UserId",
                table: "userfavorites",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userquotefavorites_quotes_QuoteId",
                table: "userquotefavorites",
                column: "QuoteId",
                principalTable: "quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userquotefavorites_users_UserId",
                table: "userquotefavorites",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_annotations_poems_PoemId",
                table: "annotations");

            migrationBuilder.DropForeignKey(
                name: "FK_annotations_users_UserId",
                table: "annotations");

            migrationBuilder.DropForeignKey(
                name: "FK_audioratings_audios_AudioId",
                table: "audioratings");

            migrationBuilder.DropForeignKey(
                name: "FK_audioratings_users_UserId",
                table: "audioratings");

            migrationBuilder.DropForeignKey(
                name: "FK_audios_poems_PoemId",
                table: "audios");

            migrationBuilder.DropForeignKey(
                name: "FK_audios_users_UploaderId",
                table: "audios");

            migrationBuilder.DropForeignKey(
                name: "FK_authorrelationships_authors_FromAuthorId",
                table: "authorrelationships");

            migrationBuilder.DropForeignKey(
                name: "FK_authorrelationships_authors_ToAuthorId",
                table: "authorrelationships");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_categories_ParentId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_poemcategories_categories_CategoryId",
                table: "poemcategories");

            migrationBuilder.DropForeignKey(
                name: "FK_poemcategories_poems_PoemId",
                table: "poemcategories");

            migrationBuilder.DropForeignKey(
                name: "FK_poems_authors_AuthorId",
                table: "poems");

            migrationBuilder.DropForeignKey(
                name: "FK_pointsrecords_users_UserId",
                table: "pointsrecords");

            migrationBuilder.DropForeignKey(
                name: "FK_quotes_authors_AuthorId",
                table: "quotes");

            migrationBuilder.DropForeignKey(
                name: "FK_quotes_poems_PoemId",
                table: "quotes");

            migrationBuilder.DropForeignKey(
                name: "FK_userfavorites_poems_PoemId",
                table: "userfavorites");

            migrationBuilder.DropForeignKey(
                name: "FK_userfavorites_users_UserId",
                table: "userfavorites");

            migrationBuilder.DropForeignKey(
                name: "FK_userquotefavorites_quotes_QuoteId",
                table: "userquotefavorites");

            migrationBuilder.DropForeignKey(
                name: "FK_userquotefavorites_users_UserId",
                table: "userquotefavorites");

            migrationBuilder.DropTable(
                name: "recitationreviews");

            migrationBuilder.DropTable(
                name: "userachievements");

            migrationBuilder.DropTable(
                name: "userrecitations");

            migrationBuilder.DropTable(
                name: "achievements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userquotefavorites",
                table: "userquotefavorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userfavorites",
                table: "userfavorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_quotes",
                table: "quotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pointsrecords",
                table: "pointsrecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_poems",
                table: "poems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_poemcategories",
                table: "poemcategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_categories",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_authors",
                table: "authors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_authorrelationships",
                table: "authorrelationships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_audios",
                table: "audios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_audioratings",
                table: "audioratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_annotations",
                table: "annotations");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "userquotefavorites",
                newName: "UserQuoteFavorites");

            migrationBuilder.RenameTable(
                name: "userfavorites",
                newName: "UserFavorites");

            migrationBuilder.RenameTable(
                name: "quotes",
                newName: "Quotes");

            migrationBuilder.RenameTable(
                name: "pointsrecords",
                newName: "PointsRecords");

            migrationBuilder.RenameTable(
                name: "poems",
                newName: "Poems");

            migrationBuilder.RenameTable(
                name: "poemcategories",
                newName: "PoemCategories");

            migrationBuilder.RenameTable(
                name: "categories",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "authors",
                newName: "Authors");

            migrationBuilder.RenameTable(
                name: "authorrelationships",
                newName: "AuthorRelationships");

            migrationBuilder.RenameTable(
                name: "audios",
                newName: "Audios");

            migrationBuilder.RenameTable(
                name: "audioratings",
                newName: "AudioRatings");

            migrationBuilder.RenameTable(
                name: "annotations",
                newName: "Annotations");

            migrationBuilder.RenameIndex(
                name: "IX_userquotefavorites_QuoteId",
                table: "UserQuoteFavorites",
                newName: "IX_UserQuoteFavorites_QuoteId");

            migrationBuilder.RenameIndex(
                name: "IX_userfavorites_PoemId",
                table: "UserFavorites",
                newName: "IX_UserFavorites_PoemId");

            migrationBuilder.RenameIndex(
                name: "IX_quotes_PoemId",
                table: "Quotes",
                newName: "IX_Quotes_PoemId");

            migrationBuilder.RenameIndex(
                name: "IX_quotes_AuthorId",
                table: "Quotes",
                newName: "IX_Quotes_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_pointsrecords_UserId",
                table: "PointsRecords",
                newName: "IX_PointsRecords_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_poems_AuthorId",
                table: "Poems",
                newName: "IX_Poems_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_poemcategories_CategoryId",
                table: "PoemCategories",
                newName: "IX_PoemCategories_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_categories_ParentId",
                table: "Categories",
                newName: "IX_Categories_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_authorrelationships_ToAuthorId",
                table: "AuthorRelationships",
                newName: "IX_AuthorRelationships_ToAuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_authorrelationships_FromAuthorId",
                table: "AuthorRelationships",
                newName: "IX_AuthorRelationships_FromAuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_audios_UploaderId",
                table: "Audios",
                newName: "IX_Audios_UploaderId");

            migrationBuilder.RenameIndex(
                name: "IX_audios_PoemId",
                table: "Audios",
                newName: "IX_Audios_PoemId");

            migrationBuilder.RenameIndex(
                name: "IX_audioratings_UserId",
                table: "AudioRatings",
                newName: "IX_AudioRatings_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_audioratings_AudioId",
                table: "AudioRatings",
                newName: "IX_AudioRatings_AudioId");

            migrationBuilder.RenameIndex(
                name: "IX_annotations_UserId",
                table: "Annotations",
                newName: "IX_Annotations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_annotations_PoemId",
                table: "Annotations",
                newName: "IX_Annotations_PoemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserQuoteFavorites",
                table: "UserQuoteFavorites",
                columns: new[] { "UserId", "QuoteId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFavorites",
                table: "UserFavorites",
                columns: new[] { "UserId", "PoemId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quotes",
                table: "Quotes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PointsRecords",
                table: "PointsRecords",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Poems",
                table: "Poems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PoemCategories",
                table: "PoemCategories",
                columns: new[] { "PoemId", "CategoryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorRelationships",
                table: "AuthorRelationships",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Audios",
                table: "Audios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AudioRatings",
                table: "AudioRatings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Annotations",
                table: "Annotations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Annotations_Poems_PoemId",
                table: "Annotations",
                column: "PoemId",
                principalTable: "Poems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Annotations_Users_UserId",
                table: "Annotations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AudioRatings_Audios_AudioId",
                table: "AudioRatings",
                column: "AudioId",
                principalTable: "Audios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AudioRatings_Users_UserId",
                table: "AudioRatings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Audios_Poems_PoemId",
                table: "Audios",
                column: "PoemId",
                principalTable: "Poems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Audios_Users_UploaderId",
                table: "Audios",
                column: "UploaderId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorRelationships_Authors_FromAuthorId",
                table: "AuthorRelationships",
                column: "FromAuthorId",
                principalTable: "Authors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorRelationships_Authors_ToAuthorId",
                table: "AuthorRelationships",
                column: "ToAuthorId",
                principalTable: "Authors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentId",
                table: "Categories",
                column: "ParentId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PoemCategories_Categories_CategoryId",
                table: "PoemCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PoemCategories_Poems_PoemId",
                table: "PoemCategories",
                column: "PoemId",
                principalTable: "Poems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Poems_Authors_AuthorId",
                table: "Poems",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PointsRecords_Users_UserId",
                table: "PointsRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Authors_AuthorId",
                table: "Quotes",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Poems_PoemId",
                table: "Quotes",
                column: "PoemId",
                principalTable: "Poems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavorites_Poems_PoemId",
                table: "UserFavorites",
                column: "PoemId",
                principalTable: "Poems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavorites_Users_UserId",
                table: "UserFavorites",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuoteFavorites_Quotes_QuoteId",
                table: "UserQuoteFavorites",
                column: "QuoteId",
                principalTable: "Quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuoteFavorites_Users_UserId",
                table: "UserQuoteFavorites",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
