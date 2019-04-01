using Microsoft.EntityFrameworkCore.Migrations;

namespace DarkDeeds.Data.Migrations
{
    public partial class N004_Add_IsProbable_to_Task : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProbable",
                table: "Tasks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProbable",
                table: "Tasks");
        }
    }
}
