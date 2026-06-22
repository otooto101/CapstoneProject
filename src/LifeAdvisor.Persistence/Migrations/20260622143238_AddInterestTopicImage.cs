using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeAdvisor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInterestTopicImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "InterestTopics",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "InterestTopics",
                keyColumn: "InterestTopicId",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1518770660439-4636190af475?auto=format&fit=crop&w=700&q=70");

            migrationBuilder.UpdateData(
                table: "InterestTopics",
                keyColumn: "InterestTopicId",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1579621970563-ebec7560ff3e?auto=format&fit=crop&w=700&q=70");

            migrationBuilder.UpdateData(
                table: "InterestTopics",
                keyColumn: "InterestTopicId",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1490645935967-10de6ba17061?auto=format&fit=crop&w=700&q=70");

            migrationBuilder.UpdateData(
                table: "InterestTopics",
                keyColumn: "InterestTopicId",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1462331940025-496dfbfc7564?auto=format&fit=crop&w=700&q=70");

            migrationBuilder.UpdateData(
                table: "InterestTopics",
                keyColumn: "InterestTopicId",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1542744173-8e7e53415bb0?auto=format&fit=crop&w=700&q=70");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "InterestTopics");
        }
    }
}
