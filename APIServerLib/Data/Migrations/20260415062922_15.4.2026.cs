using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIServerLib.Data.Migrations
{
    /// <inheritdoc />
    public partial class _1542026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsUnrwa",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsSpecialNeeds",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EnName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "LookupValues",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "EmpCenters",
                keyColumns: new[] { "CenterId", "EmployeeId" },
                keyValues: new object[] { 1L, 1L },
                column: "FromDate",
                value: new DateOnly(2026, 4, 15));

            migrationBuilder.UpdateData(
                table: "EmpCenters",
                keyColumns: new[] { "CenterId", "EmployeeId" },
                keyValues: new object[] { 2L, 2L },
                column: "FromDate",
                value: new DateOnly(2026, 4, 15));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "IsSpecialNeeds", "IsUnrwa" },
                values: new object[] { false, false });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "IsSpecialNeeds", "IsUnrwa" },
                values: new object[] { false, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsUnrwa",
                table: "Students",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSpecialNeeds",
                table: "Students",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "EnName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "LookupValues",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

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

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "IsSpecialNeeds", "IsUnrwa" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "IsSpecialNeeds", "IsUnrwa" },
                values: new object[] { null, null });
        }
    }
}
