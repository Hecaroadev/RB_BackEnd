using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mila.RoomBooking.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingDateField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWorkingDay",
                table: "Days",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "BookingDate",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DayId1",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "BookingRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingDate",
                table: "Bookings",
                column: "BookingDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DayId1",
                table: "Bookings",
                column: "DayId1");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId_TimeSlotId_BookingDate_Status",
                table: "Bookings",
                columns: new[] { "RoomId", "TimeSlotId", "BookingDate", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Days_DayId1",
                table: "Bookings",
                column: "DayId1",
                principalTable: "Days",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Days_DayId1",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_BookingDate",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_DayId1",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_RoomId_TimeSlotId_BookingDate_Status",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "IsWorkingDay",
                table: "Days");

            migrationBuilder.DropColumn(
                name: "BookingDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DayId1",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "BookingRequests");
        }
    }
}
