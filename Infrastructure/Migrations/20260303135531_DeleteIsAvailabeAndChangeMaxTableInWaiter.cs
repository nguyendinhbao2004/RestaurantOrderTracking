using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantOrderTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteIsAvailabeAndChangeMaxTableInWaiter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Waiters");

            migrationBuilder.RenameColumn(
                name: "MaxTables",
                table: "Waiters",
                newName: "OrderCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderCount",
                table: "Waiters",
                newName: "MaxTables");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Waiters",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
