using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class fixField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "11e55210-26bf-44b1-97af-00bddd629f85", "3", "CAMGIRL", "CAMGIRL" },
                    { "62848003-3201-4e77-aeeb-3391f91d5e3b", "2", "USER", "USER" },
                    { "cbedf1d9-e576-4245-9129-b8bf5318819b", "1", "ADMIN", "ADMIN" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "11e55210-26bf-44b1-97af-00bddd629f85");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "62848003-3201-4e77-aeeb-3391f91d5e3b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cbedf1d9-e576-4245-9129-b8bf5318819b");

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
    }
}
