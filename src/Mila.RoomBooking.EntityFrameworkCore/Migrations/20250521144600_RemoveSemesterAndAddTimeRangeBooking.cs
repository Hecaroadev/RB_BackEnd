using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mila.RoomBooking.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSemesterAndAddTimeRangeBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingRequests_Semesters_SemesterId",
                table: "BookingRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_RoomId_TimeSlotId_DayId_SemesterId_Status",
                table: "BookingRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_SemesterId",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "BookingRequests");

            migrationBuilder.AddColumn<int>(
                name: "AvailableTools",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "Bookings",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "Bookings",
                type: "time",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TimeSlotId",
                table: "BookingRequests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "BookingRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "BookingRequests",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "InstructorName",
                table: "BookingRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "BookingRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfStudents",
                table: "BookingRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecurringWeeks",
                table: "BookingRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequiredTools",
                table: "BookingRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "BookingRequests",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "BookingRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AvailabilityAnnouncements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    MinCapacity = table.Column<int>(type: "int", nullable: true),
                    AvailableTools = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
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
                    table.PrimaryKey("PK_AvailabilityAnnouncements", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_RoomId_TimeSlotId_DayId_Status",
                table: "BookingRequests",
                columns: new[] { "RoomId", "TimeSlotId", "DayId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityAnnouncements_Category",
                table: "AvailabilityAnnouncements",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityAnnouncements_EndDate",
                table: "AvailabilityAnnouncements",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityAnnouncements_IsActive",
                table: "AvailabilityAnnouncements",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityAnnouncements_StartDate",
                table: "AvailabilityAnnouncements",
                column: "StartDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvailabilityAnnouncements");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_RoomId_TimeSlotId_DayId_Status",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "AvailableTools",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "InstructorName",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "NumberOfStudents",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "RecurringWeeks",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "RequiredTools",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "BookingRequests");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "BookingRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "TimeSlotId",
                table: "BookingRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "BookingRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SemesterId",
                table: "BookingRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_RoomId_TimeSlotId_DayId_SemesterId_Status",
                table: "BookingRequests",
                columns: new[] { "RoomId", "TimeSlotId", "DayId", "SemesterId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_SemesterId",
                table: "BookingRequests",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRequests_Semesters_SemesterId",
                table: "BookingRequests",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
