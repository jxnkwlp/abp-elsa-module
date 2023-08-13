using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations;

/// <inheritdoc />
public partial class Update_Elsa_7 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ElsaWorkflowGroupUsers");

        migrationBuilder.DropTable(
            name: "ElsaWorkflowGroups");

        migrationBuilder.CreateTable(
            name: "ElsaWorkflowTeams",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
            constraints: table =>
            {
                table.PrimaryKey("PK_ElsaWorkflowTeams", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ElsaWorkflowTeamRoleScopes",
            columns: table => new
            {
                WorkflowTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RoleName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Values = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ElsaWorkflowTeamRoleScopes", x => new { x.WorkflowTeamId, x.RoleName });
                table.ForeignKey(
                    name: "FK_ElsaWorkflowTeamRoleScopes_ElsaWorkflowTeams_WorkflowTeamId",
                    column: x => x.WorkflowTeamId,
                    principalTable: "ElsaWorkflowTeams",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ElsaWorkflowTeamUsers",
            columns: table => new
            {
                WorkflowTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ElsaWorkflowTeamUsers", x => new { x.WorkflowTeamId, x.UserId });
                table.ForeignKey(
                    name: "FK_ElsaWorkflowTeamUsers_ElsaWorkflowTeams_WorkflowTeamId",
                    column: x => x.WorkflowTeamId,
                    principalTable: "ElsaWorkflowTeams",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ElsaWorkflowTeams_Name",
            table: "ElsaWorkflowTeams",
            column: "Name");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ElsaWorkflowTeamRoleScopes");

        migrationBuilder.DropTable(
            name: "ElsaWorkflowTeamUsers");

        migrationBuilder.DropTable(
            name: "ElsaWorkflowTeams");

        migrationBuilder.CreateTable(
            name: "ElsaWorkflowGroups",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RoleName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ElsaWorkflowGroups", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ElsaWorkflowGroupUsers",
            columns: table => new
            {
                WorkflowGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ElsaWorkflowGroupUsers", x => new { x.WorkflowGroupId, x.UserId });
                table.ForeignKey(
                    name: "FK_ElsaWorkflowGroupUsers_ElsaWorkflowGroups_WorkflowGroupId",
                    column: x => x.WorkflowGroupId,
                    principalTable: "ElsaWorkflowGroups",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ElsaWorkflowGroups_Name",
            table: "ElsaWorkflowGroups",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_ElsaWorkflowGroups_RoleName",
            table: "ElsaWorkflowGroups",
            column: "RoleName");
    }
}
