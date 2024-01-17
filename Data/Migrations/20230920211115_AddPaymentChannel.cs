using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class AddPaymentChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "PaymentChannel",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0438a214-b0bc-4cec-ae00-50bb1f05913a", "3", "CAMGIRL", "CAMGIRL" },
                    { "69ecb9a9-562d-4eed-a63d-0b070cf9ce34", "2", "USER", "USER" },
                    { "e54c32fc-ec4d-4270-ac15-3293c8d09eff", "1", "ADMIN", "ADMIN" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0438a214-b0bc-4cec-ae00-50bb1f05913a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "69ecb9a9-562d-4eed-a63d-0b070cf9ce34");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e54c32fc-ec4d-4270-ac15-3293c8d09eff");

            migrationBuilder.DropColumn(
                name: "PaymentChannel",
                table: "Payments");

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
    }
}
