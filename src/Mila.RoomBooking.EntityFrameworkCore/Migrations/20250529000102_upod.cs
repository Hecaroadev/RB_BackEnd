using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mila.RoomBooking.Migrations
{
    /// <inheritdoc />
    public partial class upod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingRequests_Days_DayId",
                table: "BookingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingRequests_TimeSlots_TimeSlotId",
                table: "BookingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Days_DayId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Days_DayId1",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TimeSlots_TimeSlotId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_DayId1",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_RoomId_TimeSlotId_BookingDate_Status",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_RoomId_TimeSlotId_DayId_Status",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "DayId1",
                table: "Bookings");

            migrationBuilder.AlterColumn<Guid>(
                name: "TimeSlotId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TimeSlotId",
                table: "BookingRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DayId",
                table: "BookingRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_RoomId",
                table: "BookingRequests",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRequests_Days_DayId",
                table: "BookingRequests",
                column: "DayId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRequests_TimeSlots_TimeSlotId",
                table: "BookingRequests",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Days_DayId",
                table: "Bookings",
                column: "DayId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TimeSlots_TimeSlotId",
                table: "Bookings",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingRequests_Days_DayId",
                table: "BookingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingRequests_TimeSlots_TimeSlotId",
                table: "BookingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Days_DayId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TimeSlots_TimeSlotId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_RoomId",
                table: "BookingRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "TimeSlotId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "DayId1",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TimeSlotId",
                table: "BookingRequests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DayId",
                table: "BookingRequests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DayId1",
                table: "Bookings",
                column: "DayId1");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId_TimeSlotId_BookingDate_Status",
                table: "Bookings",
                columns: new[] { "RoomId", "TimeSlotId", "BookingDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_RoomId_TimeSlotId_DayId_Status",
                table: "BookingRequests",
                columns: new[] { "RoomId", "TimeSlotId", "DayId", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRequests_Days_DayId",
                table: "BookingRequests",
                column: "DayId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRequests_TimeSlots_TimeSlotId",
                table: "BookingRequests",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Days_DayId",
                table: "Bookings",
                column: "DayId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Days_DayId1",
                table: "Bookings",
                column: "DayId1",
                principalTable: "Days",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TimeSlots_TimeSlotId",
                table: "Bookings",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
