using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DD.Shared.Data.Migrations;

/// <inheritdoc />
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1413:Use trailing comma in multi-line initializers", Justification = "Generated code")]
[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1122:Use string.Empty for empty strings", Justification = "Generated code")]
public partial class ExtractTelegramUser : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Settings_UserId",
            table: "Settings");

        migrationBuilder.DropColumn(
            name: "TelegramChatId",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "TelegramChatKey",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "TelegramTimeAdjustment",
            table: "AspNetUsers");

        migrationBuilder.AlterColumn<string>(
            name: "DisplayName",
            table: "AspNetUsers",
            type: "text",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.CreateTable(
            name: "TelegramUsers",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<string>(type: "text", nullable: false),
                TelegramChatKey = table.Column<string>(type: "text", nullable: false),
                TelegramChatId = table.Column<int>(type: "integer", nullable: false),
                TelegramTimeAdjustment = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TelegramUsers", x => x.Id);
                table.ForeignKey(
                    name: "FK_TelegramUsers_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Settings_UserId",
            table: "Settings",
            column: "UserId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_TelegramUsers_UserId",
            table: "TelegramUsers",
            column: "UserId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TelegramUsers");

        migrationBuilder.DropIndex(
            name: "IX_Settings_UserId",
            table: "Settings");

        migrationBuilder.AlterColumn<string>(
            name: "DisplayName",
            table: "AspNetUsers",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AddColumn<int>(
            name: "TelegramChatId",
            table: "AspNetUsers",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "TelegramChatKey",
            table: "AspNetUsers",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "TelegramTimeAdjustment",
            table: "AspNetUsers",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "IX_Settings_UserId",
            table: "Settings",
            column: "UserId");
    }
}
