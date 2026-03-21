using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantOrderTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMetadataToPaymentTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMetadata",
                table: "PaymentTransactions",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMetadata",
                table: "PaymentTransactions");
        }
    }
}
