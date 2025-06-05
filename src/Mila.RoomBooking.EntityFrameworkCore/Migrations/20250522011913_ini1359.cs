using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mila.RoomBooking.Migrations
{
    /// <inheritdoc />
    public partial class ini1359 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "RoomId",
                table: "BookingRequests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId1",
                table: "BookingRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_RoomId1",
                table: "BookingRequests",
                column: "RoomId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRequests_Rooms_RoomId1",
                table: "BookingRequests",
                column: "RoomId1",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingRequests_Rooms_RoomId1",
                table: "BookingRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_RoomId1",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "RoomId1",
                table: "BookingRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoomId",
                table: "BookingRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
