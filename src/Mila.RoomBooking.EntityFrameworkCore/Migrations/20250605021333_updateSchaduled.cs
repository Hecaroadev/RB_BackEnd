using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mila.RoomBooking.Migrations
{
    /// <inheritdoc />
    public partial class updateSchaduled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchaduledBookings_AbpUsers_ReservedByUserId",
                table: "SchaduledBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_SchaduledBookings_Days_DayId",
                table: "SchaduledBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_SchaduledBookings_Rooms_RoomId",
                table: "SchaduledBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_SchaduledBookings_SchaduledBookingId",
                table: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_SchaduledBookingId",
                table: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_SchaduledBookings_RoomId",
                table: "SchaduledBookings");

            migrationBuilder.DropColumn(
                name: "SchaduledBookingId",
                table: "TimeSlots");

            migrationBuilder.AlterColumn<string>(
                name: "Purpose",
                table: "SchaduledBookings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SchaduledBookingTimeSlots",
                columns: table => new
                {
                    SchaduledBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeSlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchaduledBookingTimeSlots", x => new { x.SchaduledBookingId, x.TimeSlotId });
                    table.ForeignKey(
                        name: "FK_SchaduledBookingTimeSlots_SchaduledBookings_SchaduledBookingId",
                        column: x => x.SchaduledBookingId,
                        principalTable: "SchaduledBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchaduledBookingTimeSlots_TimeSlots_TimeSlotId",
                        column: x => x.TimeSlotId,
                        principalTable: "TimeSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchaduledBookings_RoomDayStatus",
                table: "SchaduledBookings",
                columns: new[] { "RoomId", "DayId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SchaduledBookingTimeSlots_TimeSlotId",
                table: "SchaduledBookingTimeSlots",
                column: "TimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchaduledBookings_AbpUsers_ReservedByUserId",
                table: "SchaduledBookings",
                column: "ReservedByUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SchaduledBookings_Days_DayId",
                table: "SchaduledBookings",
                column: "DayId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SchaduledBookings_Rooms_RoomId",
                table: "SchaduledBookings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchaduledBookings_AbpUsers_ReservedByUserId",
                table: "SchaduledBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_SchaduledBookings_Days_DayId",
                table: "SchaduledBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_SchaduledBookings_Rooms_RoomId",
                table: "SchaduledBookings");

            migrationBuilder.DropTable(
                name: "SchaduledBookingTimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_SchaduledBookings_RoomDayStatus",
                table: "SchaduledBookings");

            migrationBuilder.AddColumn<Guid>(
                name: "SchaduledBookingId",
                table: "TimeSlots",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Purpose",
                table: "SchaduledBookings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_SchaduledBookingId",
                table: "TimeSlots",
                column: "SchaduledBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_SchaduledBookings_RoomId",
                table: "SchaduledBookings",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchaduledBookings_AbpUsers_ReservedByUserId",
                table: "SchaduledBookings",
                column: "ReservedByUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchaduledBookings_Days_DayId",
                table: "SchaduledBookings",
                column: "DayId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchaduledBookings_Rooms_RoomId",
                table: "SchaduledBookings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_SchaduledBookings_SchaduledBookingId",
                table: "TimeSlots",
                column: "SchaduledBookingId",
                principalTable: "SchaduledBookings",
                principalColumn: "Id");
        }
    }
}
