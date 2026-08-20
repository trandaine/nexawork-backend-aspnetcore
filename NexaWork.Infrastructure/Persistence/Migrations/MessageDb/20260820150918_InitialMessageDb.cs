using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexaWork.Infrastructure.Persistence.Migrations.MessageDb
{
    /// <inheritdoc />
    public partial class InitialMessageDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderCustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverCustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverCustomerId_SenderCustomerId_CreatedAt",
                table: "Messages",
                columns: new[] { "ReceiverCustomerId", "SenderCustomerId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderCustomerId_ReceiverCustomerId_CreatedAt",
                table: "Messages",
                columns: new[] { "SenderCustomerId", "ReceiverCustomerId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
