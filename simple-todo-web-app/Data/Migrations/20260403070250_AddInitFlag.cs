using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace simple_todo_web_app.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInitFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInit",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInit",
                table: "AspNetUsers");
        }
    }
}
