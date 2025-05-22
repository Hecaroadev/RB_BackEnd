using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mila.RoomBooking.Migrations
{
    /// <inheritdoc />
    public partial class init34 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Semesters_SemesterId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_RoomId_TimeSlotId_DayId_SemesterId_Status",
                table: "Bookings");

            migrationBuilder.AlterColumn<Guid>(
                name: "TimeSlotId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Semesters_SemesterId",
                table: "Bookings",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Semesters_SemesterId",
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

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId_TimeSlotId_DayId_SemesterId_Status",
                table: "Bookings",
                columns: new[] { "RoomId", "TimeSlotId", "DayId", "SemesterId", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Semesters_SemesterId",
                table: "Bookings",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
