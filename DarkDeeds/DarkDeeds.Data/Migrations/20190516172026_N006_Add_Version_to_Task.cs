using Microsoft.EntityFrameworkCore.Migrations;

namespace DarkDeeds.Data.Migrations
{
    public partial class N006_Add_Version_to_Task : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Tasks",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Tasks");
        }
    }
}
