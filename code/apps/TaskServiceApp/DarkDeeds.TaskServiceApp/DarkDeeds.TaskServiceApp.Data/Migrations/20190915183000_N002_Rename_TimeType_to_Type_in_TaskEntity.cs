using Microsoft.EntityFrameworkCore.Migrations;
// ReSharper disable All

namespace DarkDeeds.Data.Migrations
{
    public partial class N002_Rename_TimeType_to_Type_in_TaskEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeType",
                table: "Tasks",
                newName: "Type");
            
            migrationBuilder.Sql(@"
UPDATE ""Tasks""
SET ""Type"" = 1
WHERE ""Type"" = 2;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Tasks",
                newName: "TimeType");
        }
    }
}
