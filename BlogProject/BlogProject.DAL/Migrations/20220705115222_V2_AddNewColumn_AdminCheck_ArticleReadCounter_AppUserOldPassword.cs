using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogProject.DAL.Migrations
{
    public partial class V2_AddNewColumn_AdminCheck_ArticleReadCounter_AppUserOldPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "467eaa68-ab99-4f76-888c-9d11e77bd04f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "68c2d8f0-2fb3-45a8-8a34-780d32d5fec6");

            migrationBuilder.AddColumn<bool>(
                name: "Checked",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Checked",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Checked",
                table: "Articles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReadCounter",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Checked",
                table: "AppUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OldPassword1",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldPassword2",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "18e3de0d-c3fe-4dc0-adae-b87dd91870db", "ab88b368-407a-44e8-891f-44ec555c5970", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bcb51792-29e8-41a9-b5f1-39318eb295ba", "dccf4bf1-5344-4698-a85c-fabffbca2a5a", "Member", "MEMBER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18e3de0d-c3fe-4dc0-adae-b87dd91870db");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bcb51792-29e8-41a9-b5f1-39318eb295ba");

            migrationBuilder.DropColumn(
                name: "Checked",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Checked",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Checked",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ReadCounter",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Checked",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "OldPassword1",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "OldPassword2",
                table: "AppUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "467eaa68-ab99-4f76-888c-9d11e77bd04f", "cf639498-beff-4170-8a80-e7f5b0d4b6aa", "Member", "MEMBER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "68c2d8f0-2fb3-45a8-8a34-780d32d5fec6", "3e1dc622-1f10-424e-8fcf-2e0cfa2090bb", "Admin", "ADMIN" });
        }
    }
}
