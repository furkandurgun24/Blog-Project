using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogProject.DAL.Migrations
{
    public partial class V4_AdminCheckEnum_BaseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3bdb47ac-02ee-443a-be6e-5bdc644d9bb1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b068b7f5-786e-4e11-a2f6-b1bce33044e6");

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
                name: "Checked",
                table: "AppUsers");

            migrationBuilder.AddColumn<int>(
                name: "AdminCheck",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdminCheck",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdminCheck",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdminCheck",
                table: "AppUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "10e24d94-9ad6-4131-80fa-e358f0e9879a", "eec007e6-a495-4bb0-a974-3dffe1fd522f", "Member", "MEMBER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f6db0cfc-b156-4232-b984-47205480fc46", "b94d81ae-7360-42f1-bfa7-83c02ef3cc7f", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "10e24d94-9ad6-4131-80fa-e358f0e9879a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f6db0cfc-b156-4232-b984-47205480fc46");

            migrationBuilder.DropColumn(
                name: "AdminCheck",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "AdminCheck",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "AdminCheck",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "AdminCheck",
                table: "AppUsers");

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

            migrationBuilder.AddColumn<bool>(
                name: "Checked",
                table: "AppUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3bdb47ac-02ee-443a-be6e-5bdc644d9bb1", "72c73ee3-db4d-4fdb-af9a-ce251f47c249", "Member", "MEMBER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b068b7f5-786e-4e11-a2f6-b1bce33044e6", "3d9559d5-ef14-4389-91c8-e282cdab8552", "Admin", "ADMIN" });
        }
    }
}
