using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mila.RoomBooking.Migrations
{
    /// <inheritdoc />
    public partial class upod44 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingRequests_Days_DayId",
                table: "BookingRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_DayId",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "DayId",
                table: "BookingRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DayId",
                table: "BookingRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_DayId",
                table: "BookingRequests",
                column: "DayId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRequests_Days_DayId",
                table: "BookingRequests",
                column: "DayId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
