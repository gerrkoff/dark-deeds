using Microsoft.EntityFrameworkCore.Migrations;
// ReSharper disable All

namespace DarkDeeds.Data.Migrations
{
    public partial class N004_Add_Uid_to_Tasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Uid",
                table: "Tasks",
                type: "text",
                nullable: true,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE \"Tasks\" SET \"Uid\" = \"Id\";");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uid",
                table: "Tasks");
        }
    }
}
