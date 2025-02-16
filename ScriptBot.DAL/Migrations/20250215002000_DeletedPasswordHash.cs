using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScriptBot.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeletedPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServerPasswordHash",
                table: "Uploads");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServerPasswordHash",
                table: "Uploads",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
