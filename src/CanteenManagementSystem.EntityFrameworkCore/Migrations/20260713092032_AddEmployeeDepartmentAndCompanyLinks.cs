using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeDepartmentAndCompanyLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "AppEmployees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "AppEmployees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DesignationId",
                table: "AppEmployees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "AppDepartments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppEmployees_CategoryId",
                table: "AppEmployees",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEmployees_DepartmentId",
                table: "AppEmployees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEmployees_DesignationId",
                table: "AppEmployees",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDepartments_CompanyId",
                table: "AppDepartments",
                column: "CompanyId");

            migrationBuilder.Sql(@"
                IF OBJECT_ID('dbo.__MigrationLog', 'U') IS NULL
                CREATE TABLE dbo.__MigrationLog (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    MigrationName NVARCHAR(200) NOT NULL,
                    Message NVARCHAR(MAX) NOT NULL,
                    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                );
            ");

            migrationBuilder.Sql(@"
                WITH MatchedDepartments AS (
                    SELECT 
                        e.Id AS EmployeeId,
                        d.Id AS DepartmentId,
                        ROW_NUMBER() OVER (PARTITION BY e.Id ORDER BY 
                            CASE WHEN LTRIM(RTRIM(LOWER(e.Department))) = LTRIM(RTRIM(LOWER(d.Name))) THEN 0 ELSE 1 END,
                            d.Name
                        ) AS rn
                    FROM AppEmployees e
                    INNER JOIN AppDepartments d
                        ON LTRIM(RTRIM(LOWER(e.Department))) = LTRIM(RTRIM(LOWER(d.Name)))
                        OR LTRIM(RTRIM(LOWER(e.Department))) = LTRIM(RTRIM(LOWER(d.CCCode)))
                    WHERE e.Department IS NOT NULL AND e.Department <> ''
                )
                UPDATE e
                SET e.DepartmentId = md.DepartmentId
                FROM AppEmployees e
                INNER JOIN MatchedDepartments md ON e.Id = md.EmployeeId
                WHERE md.rn = 1;

                INSERT INTO dbo.__MigrationLog (MigrationName, Message)
                SELECT 'AddEmployeeDepartmentAndCompanyLinks',
                       'Could not match Employee.Department value ''' + ISNULL(e.Department, 'NULL') + ''' for EmployeeId ''' + ISNULL(e.EmployeeId, 'NULL') + '''. DepartmentId set to NULL.'
                FROM AppEmployees e
                WHERE e.Department IS NOT NULL
                  AND e.Department <> ''
                  AND e.DepartmentId IS NULL;
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDepartments_AppCompanies_CompanyId",
                table: "AppDepartments",
                column: "CompanyId",
                principalTable: "AppCompanies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEmployees_AppCategories_CategoryId",
                table: "AppEmployees",
                column: "CategoryId",
                principalTable: "AppCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEmployees_AppDepartments_DepartmentId",
                table: "AppEmployees",
                column: "DepartmentId",
                principalTable: "AppDepartments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEmployees_AppDesignations_DesignationId",
                table: "AppEmployees",
                column: "DesignationId",
                principalTable: "AppDesignations",
                principalColumn: "Id");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "AppEmployees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppDepartments_AppCompanies_CompanyId",
                table: "AppDepartments");

            migrationBuilder.DropForeignKey(
                name: "FK_AppEmployees_AppCategories_CategoryId",
                table: "AppEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_AppEmployees_AppDepartments_DepartmentId",
                table: "AppEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_AppEmployees_AppDesignations_DesignationId",
                table: "AppEmployees");

            migrationBuilder.DropIndex(
                name: "IX_AppEmployees_CategoryId",
                table: "AppEmployees");

            migrationBuilder.DropIndex(
                name: "IX_AppEmployees_DepartmentId",
                table: "AppEmployees");

            migrationBuilder.DropIndex(
                name: "IX_AppEmployees_DesignationId",
                table: "AppEmployees");

            migrationBuilder.DropIndex(
                name: "IX_AppDepartments_CompanyId",
                table: "AppDepartments");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "DesignationId",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AppDepartments");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "AppEmployees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
