using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace APIServerLib.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Centers",
                columns: new[] { "Id", "Address", "Comments", "DaysOfWeek", "Name", "OtherSpaces", "Rooms", "Tarpaulins" },
                values: new object[,]
                {
                    { 1L, "", null, "", "مركز الرياض", 0, 0, 0 },
                    { 2L, "", null, "", "مركز جدة", 0, 0, 0 }
                });

            migrationBuilder.InsertData(
                table: "LookupValues",
                columns: new[] { "Id", "Comments", "IsActive", "Name", "SortOrder", "ValueType" },
                values: new object[,]
                {
                    { 1L, null, true, "ذكر", 1, "Gender" },
                    { 2L, null, true, "أنثى", 2, "Gender" },
                    { 3L, null, true, "مدير مركز", 1, "Job" },
                    { 4L, null, true, "معلم", 2, "Job" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Address", "BirthDate", "CivilId", "Comments", "EmpId", "EnName", "GenderId", "JobId", "Mobile", "Name", "OrgJobId", "OrgSchool", "SpecializationId" },
                values: new object[,]
                {
                    { 2L, null, null, "", null, "", null, 1L, 1L, null, "أحمد", 1L, null, 1L },
                    { 3L, null, null, "", null, "", null, 2L, 2L, null, "سارة", 2L, null, 2L }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "BirthDate", "CivilId", "Comments", "EnName", "GenderId", "IsSpecialNeeds", "IsUnrwa", "LevelId", "Mobile", "Name", "SpecialNeeds" },
                values: new object[,]
                {
                    { 1L, null, "", null, "", 1L, null, null, 1L, null, "محمد", null },
                    { 2L, null, "", null, "", 2L, null, null, 2L, null, "ليلى", null }
                });

            migrationBuilder.InsertData(
                table: "EmpCenters",
                columns: new[] { "CenterId", "EmployeeId", "Comments", "FromDate", "ToDate" },
                values: new object[,]
                {
                    { 1L, 1L, null, new DateOnly(2026, 3, 2), null },
                    { 2L, 2L, null, new DateOnly(2026, 3, 2), null }
                });

            migrationBuilder.InsertData(
                table: "StdCenters",
                columns: new[] { "CenterId", "FromDate", "StudentId", "Comments", "ToDate" },
                values: new object[,]
                {
                    { 1L, new DateOnly(2024, 1, 1), 1L, null, null },
                    { 2L, new DateOnly(2024, 1, 1), 2L, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmpCenters",
                keyColumns: new[] { "CenterId", "EmployeeId" },
                keyValues: new object[] { 1L, 1L });

            migrationBuilder.DeleteData(
                table: "EmpCenters",
                keyColumns: new[] { "CenterId", "EmployeeId" },
                keyValues: new object[] { 2L, 2L });

            migrationBuilder.DeleteData(
                table: "StdCenters",
                keyColumns: new[] { "CenterId", "FromDate", "StudentId" },
                keyValues: new object[] { 1L, new DateOnly(2024, 1, 1), 1L });

            migrationBuilder.DeleteData(
                table: "StdCenters",
                keyColumns: new[] { "CenterId", "FromDate", "StudentId" },
                keyValues: new object[] { 2L, new DateOnly(2024, 1, 1), 2L });

            migrationBuilder.DeleteData(
                table: "Centers",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Centers",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "LookupValues",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "LookupValues",
                keyColumn: "Id",
                keyValue: 2L);
        }
    }
}
