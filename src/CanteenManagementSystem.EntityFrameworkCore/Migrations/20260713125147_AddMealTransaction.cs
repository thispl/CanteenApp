using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMealTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppMealTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PunchTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
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
                    table.PrimaryKey("PK_AppMealTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppMealTransactions_AppDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "AppDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppMealTransactions_AppEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "AppEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppMealTransactions_AppItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "AppItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppMealTransactions_AppTimeSchedules_TimeScheduleId",
                        column: x => x.TimeScheduleId,
                        principalTable: "AppTimeSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppMealTransactions_DeviceId",
                table: "AppMealTransactions",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMealTransactions_EmployeeId_PunchTime",
                table: "AppMealTransactions",
                columns: new[] { "EmployeeId", "PunchTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppMealTransactions_ItemId",
                table: "AppMealTransactions",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMealTransactions_PunchTime",
                table: "AppMealTransactions",
                column: "PunchTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppMealTransactions_Source",
                table: "AppMealTransactions",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_AppMealTransactions_TimeScheduleId",
                table: "AppMealTransactions",
                column: "TimeScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppMealTransactions");
        }
    }
}
