using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class updateUserField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "09331b03-5611-4a16-b126-42cd6a1a846d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5cb4753e-b341-4d21-b6a2-f186d515b8fc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e7413c01-955c-4b44-89d9-d3c002c7c647");

            migrationBuilder.AddColumn<bool>(
                name: "SuspendUser",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "SuspendUser",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "09331b03-5611-4a16-b126-42cd6a1a846d", "2", "USER", "USER" },
                    { "5cb4753e-b341-4d21-b6a2-f186d515b8fc", "3", "CAMGIRL", "CAMGIRL" },
                    { "e7413c01-955c-4b44-89d9-d3c002c7c647", "1", "ADMIN", "ADMIN" }
                });
        }
    }
}
