using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations;

/// <inheritdoc />
public partial class Add_GlobalCode : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ElsaGlobalCodeContents",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GlobalCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Version = table.Column<int>(type: "int", nullable: false),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_ElsaGlobalCodeContents", x => x.Id));

        migrationBuilder.CreateTable(
            name: "ElsaGlobalCodes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Language = table.Column<int>(type: "int", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                LatestVersion = table.Column<int>(type: "int", nullable: false),
                PublishedVersion = table.Column<int>(type: "int", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_ElsaGlobalCodes", x => x.Id));

        migrationBuilder.CreateTable(
            name: "ElsaGlobalCodeVersions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GlobalCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Version = table.Column<int>(type: "int", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_ElsaGlobalCodeVersions", x => x.Id));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ElsaGlobalCodeContents");

        migrationBuilder.DropTable(
            name: "ElsaGlobalCodes");

        migrationBuilder.DropTable(
            name: "ElsaGlobalCodeVersions");
    }
}
