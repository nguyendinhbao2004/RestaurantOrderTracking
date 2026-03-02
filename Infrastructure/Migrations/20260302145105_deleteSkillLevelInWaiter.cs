using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantOrderTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deleteSkillLevelInWaiter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkillLevel",
                table: "Waiters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SkillLevel",
                table: "Waiters",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
