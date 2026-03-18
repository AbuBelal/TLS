using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIServerLib.Data.Migrations
{
    /// <inheritdoc />
    public partial class CenterCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CenterCode",
                table: "Centers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Centers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CenterCode",
                value: null);

            migrationBuilder.UpdateData(
                table: "Centers",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CenterCode",
                value: null);

            migrationBuilder.UpdateData(
                table: "EmpCenters",
                keyColumns: new[] { "CenterId", "EmployeeId" },
                keyValues: new object[] { 1L, 1L },
                column: "FromDate",
                value: new DateOnly(2026, 3, 18));

            migrationBuilder.UpdateData(
                table: "EmpCenters",
                keyColumns: new[] { "CenterId", "EmployeeId" },
                keyValues: new object[] { 2L, 2L },
                column: "FromDate",
                value: new DateOnly(2026, 3, 18));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CenterCode",
                table: "Centers");

            migrationBuilder.UpdateData(
                table: "EmpCenters",
                keyColumns: new[] { "CenterId", "EmployeeId" },
                keyValues: new object[] { 1L, 1L },
                column: "FromDate",
                value: new DateOnly(2026, 3, 2));

            migrationBuilder.UpdateData(
                table: "EmpCenters",
                keyColumns: new[] { "CenterId", "EmployeeId" },
                keyValues: new object[] { 2L, 2L },
                column: "FromDate",
                value: new DateOnly(2026, 3, 2));
        }
    }
}
