using Microsoft.EntityFrameworkCore.Migrations;

namespace DarkDeeds.Data.Migrations
{
    public partial class N005_Migrate_TimeType_to_no_AfterTime : Migration
    {
        private const string TableTasks = "Tasks";
        private const string ColumnType = "TimeType";
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(TableTasks, ColumnType, 2, ColumnType, 1);
            migrationBuilder.UpdateData(TableTasks, ColumnType, 3, ColumnType, 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
