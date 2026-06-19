using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LifeAdvisor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedLifeStageOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LifeStageOptions",
                columns: new[] { "LifeStageOptionId", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, 1, true, "Student / Early Career" },
                    { 2, 2, true, "Building Career" },
                    { 3, 3, true, "Settling Down / Starting Family" },
                    { 4, 4, true, "Parenting / Raising Children" },
                    { 5, 5, true, "Career Transition / Reinvention" },
                    { 6, 6, true, "Midlife Growth / Reflection" },
                    { 7, 7, true, "Pre-Retirement Planning" },
                    { 8, 8, true, "Retirement / Later Life" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LifeStageOptions",
                keyColumn: "LifeStageOptionId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LifeStageOptions",
                keyColumn: "LifeStageOptionId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LifeStageOptions",
                keyColumn: "LifeStageOptionId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LifeStageOptions",
                keyColumn: "LifeStageOptionId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "LifeStageOptions",
                keyColumn: "LifeStageOptionId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "LifeStageOptions",
                keyColumn: "LifeStageOptionId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "LifeStageOptions",
                keyColumn: "LifeStageOptionId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "LifeStageOptions",
                keyColumn: "LifeStageOptionId",
                keyValue: 8);
        }
    }
}
