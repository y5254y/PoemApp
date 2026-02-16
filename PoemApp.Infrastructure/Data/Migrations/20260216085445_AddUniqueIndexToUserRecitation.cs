using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoemApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToUserRecitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 不删除 IX_userrecitations_UserId，因为它被外键约束使用
            // 直接创建复合唯一索引即可

            migrationBuilder.CreateIndex(
                name: "UX_UserRecitation_User_Poem",
                table: "userrecitations",
                columns: new[] { "UserId", "PoemId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_UserRecitation_User_Poem",
                table: "userrecitations");
        }
    }
}
