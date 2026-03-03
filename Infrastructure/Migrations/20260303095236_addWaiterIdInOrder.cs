using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantOrderTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addWaiterIdInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WaiterId",
                table: "Orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_WaiterId",
                table: "Orders",
                column: "WaiterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Accounts_WaiterId",
                table: "Orders",
                column: "WaiterId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Accounts_WaiterId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_WaiterId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WaiterId",
                table: "Orders");
        }
    }
}
