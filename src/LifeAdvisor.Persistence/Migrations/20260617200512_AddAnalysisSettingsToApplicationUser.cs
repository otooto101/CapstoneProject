using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeAdvisor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisSettingsToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxRelatedDecisions",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "SimilarityThreshold",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxRelatedDecisions",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SimilarityThreshold",
                table: "Users");
        }
    }
}
