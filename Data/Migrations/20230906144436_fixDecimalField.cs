using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class fixDecimalField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2be652d5-3f7d-4b72-b3c3-1e9e7bbb3081");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "acb909b8-5873-4ffc-a9ca-6d4de2133757");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f00a3d90-b741-4662-b78d-8ad4b0891e0d");

            migrationBuilder.AlterColumn<decimal>(
                name: "TimeUsed",
                table: "CallRecords",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "TimeAvailable",
                table: "AspNetUsers",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "312e2666-1e7d-4393-8b65-d055b6954c39", "3", "CAMGIRL", "CAMGIRL" },
                    { "7372f0a6-4313-4c9b-ac7a-e81506a72867", "2", "USER", "USER" },
                    { "f905ca9d-485e-42fd-b560-4904d97da263", "1", "ADMIN", "ADMIN" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "312e2666-1e7d-4393-8b65-d055b6954c39");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7372f0a6-4313-4c9b-ac7a-e81506a72867");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f905ca9d-485e-42fd-b560-4904d97da263");

            migrationBuilder.AlterColumn<int>(
                name: "TimeUsed",
                table: "CallRecords",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "TimeAvailable",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2be652d5-3f7d-4b72-b3c3-1e9e7bbb3081", "1", "ADMIN", "ADMIN" },
                    { "acb909b8-5873-4ffc-a9ca-6d4de2133757", "2", "USER", "USER" },
                    { "f00a3d90-b741-4662-b78d-8ad4b0891e0d", "3", "CAMGIRL", "CAMGIRL" }
                });
        }
    }
}
