using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexaWork.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_SoftDelete_to_Organization_JobListing_JobApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Organizations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Organizations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsValidated",
                table: "Organizations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidatedAt",
                table: "Organizations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "JobListings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "JobListings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "JobApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "JobApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "IsValidated",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "ValidatedAt",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "JobListings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "JobListings");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "JobApplications");
        }
    }
}
