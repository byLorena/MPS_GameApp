using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcGame.Migrations
{
    /// <inheritdoc />
    public partial class AddImageColumnToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddImage",
                table: "Game",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddImage",
                table: "Game");
        }
    }
}
