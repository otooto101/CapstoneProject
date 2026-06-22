using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LifeAdvisor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInterestTopicsAndTwinInterests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterestTopics",
                columns: table => new
                {
                    InterestTopicId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Emoji = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestTopics", x => x.InterestTopicId);
                });

            migrationBuilder.CreateTable(
                name: "TwinInterests",
                columns: table => new
                {
                    TwinInterestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DigitalTwinId = table.Column<int>(type: "int", nullable: false),
                    InterestTopicId = table.Column<int>(type: "int", nullable: false),
                    IsInterested = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwinInterests", x => x.TwinInterestId);
                    table.ForeignKey(
                        name: "FK_TwinInterests_DigitalTwins_DigitalTwinId",
                        column: x => x.DigitalTwinId,
                        principalTable: "DigitalTwins",
                        principalColumn: "DigitalTwinId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TwinInterests_InterestTopics_InterestTopicId",
                        column: x => x.InterestTopicId,
                        principalTable: "InterestTopics",
                        principalColumn: "InterestTopicId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "InterestTopics",
                columns: new[] { "InterestTopicId", "Category", "Description", "DisplayOrder", "Emoji", "IsActive", "Title" },
                values: new object[,]
                {
                    { 1, "Tech", "Breakthroughs, new tools, and how AI is reshaping the way we live and work.", 1, "🤖", true, "Technology & AI" },
                    { 2, "Finance", "Markets, saving strategies, and smart money moves that compound over time.", 2, "💸", true, "Money & Investing" },
                    { 3, "Wellbeing", "Fitness, nutrition, sleep, and the science of living a longer, better life.", 3, "🌿", true, "Health & Longevity" },
                    { 4, "Science", "Discoveries, the cosmos, and the ideas quietly rewriting what we know.", 4, "🔭", true, "Science & Space" },
                    { 5, "Growth", "Work, startups, side projects, and growing into the next version of you.", 5, "🚀", true, "Career & Hustle" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TwinInterests_DigitalTwinId_InterestTopicId",
                table: "TwinInterests",
                columns: new[] { "DigitalTwinId", "InterestTopicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TwinInterests_InterestTopicId",
                table: "TwinInterests",
                column: "InterestTopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwinInterests");

            migrationBuilder.DropTable(
                name: "InterestTopics");
        }
    }
}
