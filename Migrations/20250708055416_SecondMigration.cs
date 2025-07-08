using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentSimilarityComparison.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserCommunicationStatus",
                table: "ResumeDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserCommunicationStatus",
                table: "ResumeDetails");
        }
    }
}
