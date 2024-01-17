using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class fixEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c4f83f8-9a72-4b02-816e-cea1c2751b7e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5c48894f-4da2-4a78-8bf6-47ae1ea93231");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "866b72e6-9a84-4f69-8fef-66f50d5a3ad6");

            migrationBuilder.RenameColumn(
                name: "ReferenceNumber",
                table: "Payments",
                newName: "PaymentStatus");

            migrationBuilder.RenameColumn(
                name: "PaymentTime",
                table: "Payments",
                newName: "CreatedPaymentTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletePaymentTime",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OrderReferenceId",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CompletePaymentTime",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderReferenceId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                table: "Payments",
                newName: "ReferenceNumber");

            migrationBuilder.RenameColumn(
                name: "CreatedPaymentTime",
                table: "Payments",
                newName: "PaymentTime");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2c4f83f8-9a72-4b02-816e-cea1c2751b7e", "1", "ADMIN", "ADMIN" },
                    { "5c48894f-4da2-4a78-8bf6-47ae1ea93231", "3", "CAMGIRL", "CAMGIRL" },
                    { "866b72e6-9a84-4f69-8fef-66f50d5a3ad6", "2", "USER", "USER" }
                });
        }
    }
}
