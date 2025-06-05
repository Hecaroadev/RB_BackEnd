using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mila.RoomBooking.Migrations
{
    /// <inheritdoc />
    public partial class init55 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SchaduledBookingId",
                table: "TimeSlots",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SchaduledBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReservedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchaduledBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchaduledBookings_AbpUsers_ReservedByUserId",
                        column: x => x.ReservedByUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchaduledBookings_Days_DayId",
                        column: x => x.DayId,
                        principalTable: "Days",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchaduledBookings_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_SchaduledBookingId",
                table: "TimeSlots",
                column: "SchaduledBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_SchaduledBookings_DayId",
                table: "SchaduledBookings",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_SchaduledBookings_ReservedByUserId",
                table: "SchaduledBookings",
                column: "ReservedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SchaduledBookings_RoomId",
                table: "SchaduledBookings",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_SchaduledBookings_SchaduledBookingId",
                table: "TimeSlots",
                column: "SchaduledBookingId",
                principalTable: "SchaduledBookings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_SchaduledBookings_SchaduledBookingId",
                table: "TimeSlots");

            migrationBuilder.DropTable(
                name: "SchaduledBookings");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_SchaduledBookingId",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "SchaduledBookingId",
                table: "TimeSlots");
        }
    }
}
