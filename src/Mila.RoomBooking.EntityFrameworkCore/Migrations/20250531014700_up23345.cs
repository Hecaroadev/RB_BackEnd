using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mila.RoomBooking.Migrations
{
    /// <inheritdoc />
    public partial class up23345 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AbpUsers_ReservedById",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ReservedById",
                table: "Bookings");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReservedById",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ReservedBy",
                table: "Bookings",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<Guid>(
                name: "ReservedByUserId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ReservedByUserId",
                table: "Bookings",
                column: "ReservedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AbpUsers_ReservedByUserId",
                table: "Bookings",
                column: "ReservedByUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AbpUsers_ReservedByUserId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ReservedByUserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ReservedByUserId",
                table: "Bookings");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReservedById",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReservedBy",
                table: "Bookings",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ReservedById",
                table: "Bookings",
                column: "ReservedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AbpUsers_ReservedById",
                table: "Bookings",
                column: "ReservedById",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
