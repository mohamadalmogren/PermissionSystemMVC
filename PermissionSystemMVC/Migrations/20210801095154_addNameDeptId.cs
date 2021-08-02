using Microsoft.EntityFrameworkCore.Migrations;

namespace PermissionSystemMVC.Migrations
{
    public partial class addNameDeptId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Manager_Departments_DepartmentForeignKeyId",
                table: "Manager");

            migrationBuilder.DropIndex(
                name: "IX_Manager_DepartmentForeignKeyId",
                table: "Manager");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Manager",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Manager_DepartmentId",
                table: "Manager",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Manager_Departments_DepartmentId",
                table: "Manager",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Manager_Departments_DepartmentId",
                table: "Manager");

            migrationBuilder.DropIndex(
                name: "IX_Manager_DepartmentId",
                table: "Manager");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Manager");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Manager_DepartmentForeignKeyId",
                table: "Manager",
                column: "DepartmentForeignKeyId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Manager_Departments_DepartmentForeignKeyId",
                table: "Manager",
                column: "DepartmentForeignKeyId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
