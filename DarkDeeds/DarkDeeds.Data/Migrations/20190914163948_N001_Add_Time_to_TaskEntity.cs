using Microsoft.EntityFrameworkCore.Migrations;

namespace DarkDeeds.Data.Migrations
{
    public partial class N001_Add_Time_to_TaskEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "Tasks",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "Time",
                table: "Tasks",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE ""Tasks"" SET ""Date"" = ""Date"" + interval '3 hour'; -- just because I know all my tasks was created in UTC+3h timezone

UPDATE ""Tasks""
SET ""Time"" = date_part('hour', ""Date"") * 60 + date_part('minute', ""Date""),
    ""Date"" = date_trunc('day', ""Date""),
    ""TimeType"" = 0
WHERE ""TimeType"" = 1;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Tasks",
                newName: "DateTime");
        }
    }
}
