using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeScheduleItemLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ItemId",
                table: "AppTimeSchedules",
                type: "uniqueidentifier",
                nullable: true);

            // Seed ItemId links for existing TimeSchedule rows.
            // Clear matches: Breakfast -> Breakfast/Tiffen, Snacks -> Snacks.
            // Ambiguous matches flagged for review: Lunch/Dinner -> Veg-Lunch/Dinner.
            migrationBuilder.Sql(@"
                -- Map Breakfast -> Breakfast/Tiffen
                UPDATE ts SET ItemId = i.Id
                FROM AppTimeSchedules ts
                JOIN AppItems i ON i.Description = 'Breakfast/Tiffen'
                WHERE ts.Name = 'Breakfast';

                -- Map Snacks -> Snacks
                UPDATE ts SET ItemId = i.Id
                FROM AppTimeSchedules ts
                JOIN AppItems i ON i.Description = 'Snacks'
                WHERE ts.Name = 'Snacks';

                -- Map Lunch -> Veg-Lunch/Dinner (ambiguous; flagged below for review)
                UPDATE ts SET ItemId = i.Id
                FROM AppTimeSchedules ts
                JOIN AppItems i ON i.Description = 'Veg-Lunch/Dinner'
                WHERE ts.Name = 'Lunch';

                -- Map Dinner -> Veg-Lunch/Dinner (ambiguous; flagged below for review)
                UPDATE ts SET ItemId = i.Id
                FROM AppTimeSchedules ts
                JOIN AppItems i ON i.Description = 'Veg-Lunch/Dinner'
                WHERE ts.Name = 'Dinner';

                -- Log ambiguous or unmapped schedules for review
                INSERT INTO dbo.__MigrationLog (MigrationName, Message, CreatedAt)
                SELECT 'AddTimeScheduleItemLink',
                       'TimeSchedule ' + ISNULL(Name, 'NULL') + ' linked to ItemId ' + ISNULL(CAST(ItemId AS VARCHAR(36)), 'NULL') + '. Please confirm/correct via UI.',
                       GETUTCDATE()
                FROM AppTimeSchedules
                WHERE Name IN ('Lunch', 'Dinner')
                   OR ItemId IS NULL;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_AppTimeSchedules_ItemId",
                table: "AppTimeSchedules",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTimeSchedules_AppItems_ItemId",
                table: "AppTimeSchedules",
                column: "ItemId",
                principalTable: "AppItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTimeSchedules_AppItems_ItemId",
                table: "AppTimeSchedules");

            migrationBuilder.DropIndex(
                name: "IX_AppTimeSchedules_ItemId",
                table: "AppTimeSchedules");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "AppTimeSchedules");
        }
    }
}
