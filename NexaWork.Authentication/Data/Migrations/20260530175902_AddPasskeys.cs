using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexaWork.Authentication.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPasskeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Preferred2faMethod",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FidoStoredCredentials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PublicKey = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UserHandle = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    SignatureCounter = table.Column<long>(type: "bigint", nullable: false),
                    CredType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AaGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DescriptorId = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FidoStoredCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FidoStoredCredentials_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FidoStoredCredentials_UserId",
                table: "FidoStoredCredentials",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FidoStoredCredentials");

            migrationBuilder.DropColumn(
                name: "Preferred2faMethod",
                table: "AspNetUsers");
        }
    }
}
