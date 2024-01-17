using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class AddOnCallMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "UserIsOnCall",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FavouriteCamGirls",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CamgirlUserName = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteCamGirls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavouriteCamGirls_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteCamGirls_UserId",
                table: "FavouriteCamGirls",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavouriteCamGirls");

            migrationBuilder.DropColumn(
                name: "UserIsOnCall",
                table: "AspNetUsers");

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
    }
}
