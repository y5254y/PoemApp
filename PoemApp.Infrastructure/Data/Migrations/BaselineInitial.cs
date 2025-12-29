using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoemApp.Infrastructure.Data.Migrations
{
    public partial class BaselineInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Baseline migration: no schema changes (used to record current DB as baseline)
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // no-op
        }
    }
}