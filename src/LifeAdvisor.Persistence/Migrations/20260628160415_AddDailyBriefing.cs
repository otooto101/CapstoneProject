using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeAdvisor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyBriefing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyBriefings",
                columns: table => new
                {
                    DailyBriefingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DigitalTwinId = table.Column<int>(type: "int", nullable: false),
                    BriefingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Greeting = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyBriefings", x => x.DailyBriefingId);
                    table.ForeignKey(
                        name: "FK_DailyBriefings_DigitalTwins_DigitalTwinId",
                        column: x => x.DigitalTwinId,
                        principalTable: "DigitalTwins",
                        principalColumn: "DigitalTwinId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BriefingItems",
                columns: table => new
                {
                    BriefingItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DailyBriefingId = table.Column<int>(type: "int", nullable: false),
                    Headline = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Blurb = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    WhyItMatters = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    SourceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MatchedInterest = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BriefingItems", x => x.BriefingItemId);
                    table.ForeignKey(
                        name: "FK_BriefingItems_DailyBriefings_DailyBriefingId",
                        column: x => x.DailyBriefingId,
                        principalTable: "DailyBriefings",
                        principalColumn: "DailyBriefingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BriefingItems_DailyBriefingId",
                table: "BriefingItems",
                column: "DailyBriefingId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyBriefings_DigitalTwinId_BriefingDate",
                table: "DailyBriefings",
                columns: new[] { "DigitalTwinId", "BriefingDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BriefingItems");

            migrationBuilder.DropTable(
                name: "DailyBriefings");
        }
    }
}
