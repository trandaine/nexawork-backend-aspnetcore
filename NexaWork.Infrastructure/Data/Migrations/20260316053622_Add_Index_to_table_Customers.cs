using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexaWork.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Index_to_table_Customers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Customers_IdentityUserId",
                table: "Customers",
                column: "IdentityUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_IdentityUserId",
                table: "Customers");
        }
    }
}
