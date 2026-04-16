using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIServerLib.Data.Migrations
{
    /// <inheritdoc />
    public partial class _15420261 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CivilId",
                table: "Employees",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<long>(
                name: "WhoursId",
                table: "Centers",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Centers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "WhoursId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Centers",
                keyColumn: "Id",
                keyValue: 2L,
                column: "WhoursId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Centers_WhoursId",
                table: "Centers",
                column: "WhoursId");

            migrationBuilder.AddForeignKey(
                name: "FK_Centers_LookupValues_WhoursId",
                table: "Centers",
                column: "WhoursId",
                principalTable: "LookupValues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Centers_LookupValues_WhoursId",
                table: "Centers");

            migrationBuilder.DropIndex(
                name: "IX_Centers_WhoursId",
                table: "Centers");

            migrationBuilder.DropColumn(
                name: "WhoursId",
                table: "Centers");

            migrationBuilder.AlterColumn<string>(
                name: "CivilId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(9)",
                oldMaxLength: 9);
        }
    }
}
