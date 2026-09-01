using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexaWork.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusBeforeBlock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusBeforeBlock",
                table: "Connections",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusBeforeBlock",
                table: "Connections");
        }
    }
}
