using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeAdvisor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDecisionHistoryToTwinNarrative : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DecisionTitle",
                table: "TwinNarratives",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompletedDecision",
                table: "TwinNarratives",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDecision",
                table: "TwinNarratives",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ScenarioOptionsJson",
                table: "TwinNarratives",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SelectedScenarioTitle",
                table: "TwinNarratives",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecisionTitle",
                table: "TwinNarratives");

            migrationBuilder.DropColumn(
                name: "IsCompletedDecision",
                table: "TwinNarratives");

            migrationBuilder.DropColumn(
                name: "IsDecision",
                table: "TwinNarratives");

            migrationBuilder.DropColumn(
                name: "ScenarioOptionsJson",
                table: "TwinNarratives");

            migrationBuilder.DropColumn(
                name: "SelectedScenarioTitle",
                table: "TwinNarratives");
        }
    }
}
