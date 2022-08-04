using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogProject.DAL.Migrations
{
    public partial class V5_AppUser_OldPassword3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "10e24d94-9ad6-4131-80fa-e358f0e9879a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f6db0cfc-b156-4232-b984-47205480fc46");

            migrationBuilder.AddColumn<string>(
                name: "OldPassword3",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8d366351-02ac-48dd-ab41-d7f9982624ef", "aca14380-1b17-4b49-a066-2a64a3b478f9", "Member", "MEMBER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a1f6f95e-f23e-4eb3-a1d7-127b10425be5", "7cf2b878-4911-4428-82d5-0ba73b72fc3b", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8d366351-02ac-48dd-ab41-d7f9982624ef");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a1f6f95e-f23e-4eb3-a1d7-127b10425be5");

            migrationBuilder.DropColumn(
                name: "OldPassword3",
                table: "AppUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "10e24d94-9ad6-4131-80fa-e358f0e9879a", "eec007e6-a495-4bb0-a974-3dffe1fd522f", "Member", "MEMBER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f6db0cfc-b156-4232-b984-47205480fc46", "b94d81ae-7360-42f1-bfa7-83c02ef3cc7f", "Admin", "ADMIN" });
        }
    }
}
