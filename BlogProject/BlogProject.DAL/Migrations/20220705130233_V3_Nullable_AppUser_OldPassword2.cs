using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogProject.DAL.Migrations
{
    public partial class V3_Nullable_AppUser_OldPassword2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18e3de0d-c3fe-4dc0-adae-b87dd91870db");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bcb51792-29e8-41a9-b5f1-39318eb295ba");

            migrationBuilder.AlterColumn<string>(
                name: "OldPassword2",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3bdb47ac-02ee-443a-be6e-5bdc644d9bb1", "72c73ee3-db4d-4fdb-af9a-ce251f47c249", "Member", "MEMBER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b068b7f5-786e-4e11-a2f6-b1bce33044e6", "3d9559d5-ef14-4389-91c8-e282cdab8552", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3bdb47ac-02ee-443a-be6e-5bdc644d9bb1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b068b7f5-786e-4e11-a2f6-b1bce33044e6");

            migrationBuilder.AlterColumn<string>(
                name: "OldPassword2",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "18e3de0d-c3fe-4dc0-adae-b87dd91870db", "ab88b368-407a-44e8-891f-44ec555c5970", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bcb51792-29e8-41a9-b5f1-39318eb295ba", "dccf4bf1-5344-4698-a85c-fabffbca2a5a", "Member", "MEMBER" });
        }
    }
}
