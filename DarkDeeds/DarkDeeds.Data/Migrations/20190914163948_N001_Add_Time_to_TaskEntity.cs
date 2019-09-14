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
UPDATE ""Tasks"" SET ""Date"" = ""Date"" - interval '3 hour'; -- just because I know all my tasks was created in UTC+3h timezone

UPDATE ""Tasks""
SET ""Time"" = CASE WHEN
                    date_part('hour', ""Date"") <> 0 OR
                    date_part('minute', ""Date"") <> 0
                THEN date_part('hour', ""Date"") * 60 + date_part('minute', ""Date"")
                ELSE NULL END,
""Date"" = date_trunc('day', ""Date"");
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
