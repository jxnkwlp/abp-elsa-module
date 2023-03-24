using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations;

public partial class Update_Elsa_3 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ElsaActivities_ElsaWorkflowDefinitionVersions_DefinitionVersionId",
            table: "ElsaActivities");

        migrationBuilder.DropForeignKey(
            name: "FK_ElsaActivityConnections_ElsaWorkflowDefinitionVersions_DefinitionVersionId",
            table: "ElsaActivityConnections");

        migrationBuilder.DropForeignKey(
            name: "FK_ElsaWorkflowExecutionLogs_ElsaWorkflowInstances_WorkflowInstanceId",
            table: "ElsaWorkflowExecutionLogs");

        migrationBuilder.DropIndex(
            name: "IX_ElsaWorkflowExecutionLogs_WorkflowInstanceId",
            table: "ElsaWorkflowExecutionLogs");

        migrationBuilder.DropColumn(
            name: "ActivityData",
            table: "ElsaWorkflowInstances");

        migrationBuilder.DropColumn(
            name: "BlockingActivities",
            table: "ElsaWorkflowInstances");

        migrationBuilder.DropColumn(
            name: "Metadata",
            table: "ElsaWorkflowInstances");

        migrationBuilder.DropColumn(
            name: "ScheduledActivities",
            table: "ElsaWorkflowInstances");

        migrationBuilder.DropColumn(
            name: "Scopes",
            table: "ElsaWorkflowInstances");

        migrationBuilder.DropColumn(
            name: "Variables",
            table: "ElsaWorkflowInstances");

        migrationBuilder.DropColumn(
            name: "CreationTime",
            table: "ElsaActivityConnections");

        migrationBuilder.DropColumn(
            name: "CreatorId",
            table: "ElsaActivityConnections");

        migrationBuilder.DropColumn(
            name: "CreationTime",
            table: "ElsaActivities");

        migrationBuilder.DropColumn(
            name: "CreatorId",
            table: "ElsaActivities");

        migrationBuilder.DropColumn(
            name: "LastModificationTime",
            table: "ElsaActivities");

        migrationBuilder.DropColumn(
            name: "LastModifierId",
            table: "ElsaActivities");

        migrationBuilder.AddColumn<string>(
            name: "ExtraProperties",
            table: "ElsaWorkflowInstances",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.RenameColumn(
            name: "DefinitionVersionId",
            table: "ElsaWorkflowInstances",
            newName: "WorkflowDefinitionVersionId");

        migrationBuilder.RenameColumn(
            name: "DefinitionId",
            table: "ElsaWorkflowInstances",
            newName: "WorkflowDefinitionId");

        migrationBuilder.RenameColumn(
            name: "Arrtibutes",
            table: "ElsaActivityConnections",
            newName: "Attributes");

        migrationBuilder.RenameColumn(
            name: "DefinitionVersionId",
            table: "ElsaActivityConnections",
            newName: "WorkflowDefinitionVersionId");

        migrationBuilder.RenameColumn(
            name: "Arrtibutes",
            table: "ElsaActivities",
            newName: "Attributes");

        migrationBuilder.RenameColumn(
            name: "DefinitionVersionId",
            table: "ElsaActivities",
            newName: "WorkflowDefinitionVersionId");

        migrationBuilder.AddColumn<string>(
            name: "ConcurrencyStamp",
            table: "ElsaWorkflowInstances",
            type: "nvarchar(40)",
            maxLength: 40,
            nullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ActivityType",
            table: "ElsaWorkflowExecutionLogs",
            type: "nvarchar(256)",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(128)",
            oldMaxLength: 128,
            oldNullable: true);

        // change by manual
        //migrationBuilder.AlterColumn<Guid>(
        //    name: "Id",
        //    table: "ElsaWorkflowExecutionLogs",
        //    type: "uniqueidentifier",
        //    nullable: false,
        //    oldClrType: typeof(long),
        //    oldType: "bigint")
        //    .OldAnnotation("SqlServer:Identity", "1, 1");

        migrationBuilder.Sql(@"
ALTER TABLE [dbo].[ElsaWorkflowExecutionLogs] DROP CONSTRAINT [PK_ElsaWorkflowExecutionLogs]
GO 
ALTER TABLE [dbo].[ElsaWorkflowExecutionLogs] DROP COLUMN [Id] 
GO

ALTER TABLE [dbo].[ElsaWorkflowExecutionLogs] ADD [Id] uniqueidentifier DEFAULT NEWID() NOT NULL
GO
ALTER TABLE [dbo].[ElsaWorkflowExecutionLogs] ADD PRIMARY KEY CLUSTERED ([Id])
GO ");

        migrationBuilder.AddColumn<string>(
            name: "ConcurrencyStamp",
            table: "ElsaWorkflowExecutionLogs",
            type: "nvarchar(40)",
            maxLength: 40,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ExtraProperties",
            table: "ElsaWorkflowExecutionLogs",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ConcurrencyStamp",
            table: "ElsaWorkflowDefinitionVersions",
            type: "nvarchar(40)",
            maxLength: 40,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ExtraProperties",
            table: "ElsaWorkflowDefinitionVersions",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ConcurrencyStamp",
            table: "ElsaWorkflowDefinitions",
            type: "nvarchar(40)",
            maxLength: 40,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ExtraProperties",
            table: "ElsaWorkflowDefinitions",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ConcurrencyStamp",
            table: "ElsaTriggers",
            type: "nvarchar(40)",
            maxLength: 40,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ExtraProperties",
            table: "ElsaTriggers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ConcurrencyStamp",
            table: "ElsaBookmarks",
            type: "nvarchar(40)",
            maxLength: 40,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ExtraProperties",
            table: "ElsaBookmarks",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "ElsaWorkflowInstanceActivityData",
            columns: table => new
            {
                WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Data = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ElsaWorkflowInstanceActivityData", x => new { x.WorkflowInstanceId, x.ActivityId });
                table.ForeignKey(
                    name: "FK_ElsaWorkflowInstanceActivityData_ElsaWorkflowInstances_WorkflowInstanceId",
                    column: x => x.WorkflowInstanceId,
                    principalTable: "ElsaWorkflowInstances",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ElsaWorkflowInstanceBlockingActivities",
            columns: table => new
            {
                WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ActivityType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Tag = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ElsaWorkflowInstanceBlockingActivities", x => new { x.WorkflowInstanceId, x.ActivityId });
                table.ForeignKey(
                    name: "FK_ElsaWorkflowInstanceBlockingActivities_ElsaWorkflowInstances_WorkflowInstanceId",
                    column: x => x.WorkflowInstanceId,
                    principalTable: "ElsaWorkflowInstances",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ElsaWorkflowInstanceMetadata",
            columns: table => new
            {
                WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Key = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ElsaWorkflowInstanceMetadata", x => new { x.WorkflowInstanceId, x.Key });
                table.ForeignKey(
                    name: "FK_ElsaWorkflowInstanceMetadata_ElsaWorkflowInstances_WorkflowInstanceId",
                    column: x => x.WorkflowInstanceId,
                    principalTable: "ElsaWorkflowInstances",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ElsaWorkflowInstanceScheduledActivities",
            columns: table => new
            {
                WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Input = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ElsaWorkflowInstanceScheduledActivities", x => new { x.WorkflowInstanceId, x.ActivityId });
                table.ForeignKey(
                    name: "FK_ElsaWorkflowInstanceScheduledActivities_ElsaWorkflowInstances_WorkflowInstanceId",
                    column: x => x.WorkflowInstanceId,
                    principalTable: "ElsaWorkflowInstances",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ElsaWorkflowInstanceScopes",
            columns: table => new
            {
                WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Variables = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ElsaWorkflowInstanceScopes", x => new { x.WorkflowInstanceId, x.ActivityId });
                table.ForeignKey(
                    name: "FK_ElsaWorkflowInstanceScopes_ElsaWorkflowInstances_WorkflowInstanceId",
                    column: x => x.WorkflowInstanceId,
                    principalTable: "ElsaWorkflowInstances",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ElsaWorkflowInstanceVariables",
            columns: table => new
            {
                WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Key = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ElsaWorkflowInstanceVariables", x => new { x.WorkflowInstanceId, x.Key });
                table.ForeignKey(
                    name: "FK_ElsaWorkflowInstanceVariables_ElsaWorkflowInstances_WorkflowInstanceId",
                    column: x => x.WorkflowInstanceId,
                    principalTable: "ElsaWorkflowInstances",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.AddForeignKey(
            name: "FK_ElsaActivities_ElsaWorkflowDefinitionVersions_WorkflowDefinitionVersionId",
            table: "ElsaActivities",
            column: "WorkflowDefinitionVersionId",
            principalTable: "ElsaWorkflowDefinitionVersions",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_ElsaActivityConnections_ElsaWorkflowDefinitionVersions_WorkflowDefinitionVersionId",
            table: "ElsaActivityConnections",
            column: "WorkflowDefinitionVersionId",
            principalTable: "ElsaWorkflowDefinitionVersions",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        //migrationBuilder.DropForeignKey(
        //    name: "FK_ElsaActivities_ElsaWorkflowDefinitionVersions_WorkflowDefinitionVersionId",
        //    table: "ElsaActivities");

        //migrationBuilder.DropForeignKey(
        //    name: "FK_ElsaActivityConnections_ElsaWorkflowDefinitionVersions_WorkflowDefinitionVersionId",
        //    table: "ElsaActivityConnections");

        //migrationBuilder.DropTable(
        //    name: "ElsaWorkflowInstanceActivityData");

        //migrationBuilder.DropTable(
        //    name: "ElsaWorkflowInstanceBlockingActivities");

        //migrationBuilder.DropTable(
        //    name: "ElsaWorkflowInstanceMetadata");

        //migrationBuilder.DropTable(
        //    name: "ElsaWorkflowInstanceScheduledActivities");

        //migrationBuilder.DropTable(
        //    name: "ElsaWorkflowInstanceScopes");

        //migrationBuilder.DropTable(
        //    name: "ElsaWorkflowInstanceVariables");

        //migrationBuilder.DropColumn(
        //    name: "ConcurrencyStamp",
        //    table: "ElsaWorkflowInstances");

        //migrationBuilder.DropColumn(
        //    name: "ConcurrencyStamp",
        //    table: "ElsaWorkflowExecutionLogs");

        //migrationBuilder.DropColumn(
        //    name: "ExtraProperties",
        //    table: "ElsaWorkflowExecutionLogs");

        //migrationBuilder.DropColumn(
        //    name: "ConcurrencyStamp",
        //    table: "ElsaWorkflowDefinitionVersions");

        //migrationBuilder.DropColumn(
        //    name: "ExtraProperties",
        //    table: "ElsaWorkflowDefinitionVersions");

        //migrationBuilder.DropColumn(
        //    name: "ConcurrencyStamp",
        //    table: "ElsaWorkflowDefinitions");

        //migrationBuilder.DropColumn(
        //    name: "ExtraProperties",
        //    table: "ElsaWorkflowDefinitions");

        //migrationBuilder.DropColumn(
        //    name: "ConcurrencyStamp",
        //    table: "ElsaTriggers");

        //migrationBuilder.DropColumn(
        //    name: "ExtraProperties",
        //    table: "ElsaTriggers");

        //migrationBuilder.DropColumn(
        //    name: "ConcurrencyStamp",
        //    table: "ElsaBookmarks");

        //migrationBuilder.DropColumn(
        //    name: "ExtraProperties",
        //    table: "ElsaBookmarks");

        //migrationBuilder.RenameColumn(
        //    name: "WorkflowDefinitionVersionId",
        //    table: "ElsaWorkflowInstances",
        //    newName: "DefinitionVersionId");

        //migrationBuilder.RenameColumn(
        //    name: "WorkflowDefinitionId",
        //    table: "ElsaWorkflowInstances",
        //    newName: "DefinitionId");

        //migrationBuilder.RenameColumn(
        //    name: "ExtraProperties",
        //    table: "ElsaWorkflowInstances",
        //    newName: "Variables");

        //migrationBuilder.RenameColumn(
        //    name: "Attributes",
        //    table: "ElsaActivityConnections",
        //    newName: "Arrtibutes");

        //migrationBuilder.RenameColumn(
        //    name: "WorkflowDefinitionVersionId",
        //    table: "ElsaActivityConnections",
        //    newName: "DefinitionVersionId");

        //migrationBuilder.RenameColumn(
        //    name: "Attributes",
        //    table: "ElsaActivities",
        //    newName: "Arrtibutes");

        //migrationBuilder.RenameColumn(
        //    name: "WorkflowDefinitionVersionId",
        //    table: "ElsaActivities",
        //    newName: "DefinitionVersionId");

        //migrationBuilder.AddColumn<string>(
        //    name: "ActivityData",
        //    table: "ElsaWorkflowInstances",
        //    type: "nvarchar(max)",
        //    nullable: true);

        //migrationBuilder.AddColumn<string>(
        //    name: "BlockingActivities",
        //    table: "ElsaWorkflowInstances",
        //    type: "nvarchar(max)",
        //    nullable: true);

        //migrationBuilder.AddColumn<string>(
        //    name: "Metadata",
        //    table: "ElsaWorkflowInstances",
        //    type: "nvarchar(max)",
        //    nullable: true);

        //migrationBuilder.AddColumn<string>(
        //    name: "ScheduledActivities",
        //    table: "ElsaWorkflowInstances",
        //    type: "nvarchar(max)",
        //    nullable: true);

        //migrationBuilder.AddColumn<string>(
        //    name: "Scopes",
        //    table: "ElsaWorkflowInstances",
        //    type: "nvarchar(max)",
        //    nullable: true);

        //migrationBuilder.AlterColumn<string>(
        //    name: "ActivityType",
        //    table: "ElsaWorkflowExecutionLogs",
        //    type: "nvarchar(128)",
        //    maxLength: 128,
        //    nullable: true,
        //    oldClrType: typeof(string),
        //    oldType: "nvarchar(256)",
        //    oldMaxLength: 256,
        //    oldNullable: true);

        //migrationBuilder.AlterColumn<long>(
        //    name: "Id",
        //    table: "ElsaWorkflowExecutionLogs",
        //    type: "bigint",
        //    nullable: false,
        //    oldClrType: typeof(Guid),
        //    oldType: "uniqueidentifier")
        //    .Annotation("SqlServer:Identity", "1, 1");

        //migrationBuilder.AddColumn<DateTime>(
        //    name: "CreationTime",
        //    table: "ElsaActivityConnections",
        //    type: "datetime2",
        //    nullable: false,
        //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        //migrationBuilder.AddColumn<Guid>(
        //    name: "CreatorId",
        //    table: "ElsaActivityConnections",
        //    type: "uniqueidentifier",
        //    nullable: true);

        //migrationBuilder.AddColumn<DateTime>(
        //    name: "CreationTime",
        //    table: "ElsaActivities",
        //    type: "datetime2",
        //    nullable: false,
        //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        //migrationBuilder.AddColumn<Guid>(
        //    name: "CreatorId",
        //    table: "ElsaActivities",
        //    type: "uniqueidentifier",
        //    nullable: true);

        //migrationBuilder.AddColumn<DateTime>(
        //    name: "LastModificationTime",
        //    table: "ElsaActivities",
        //    type: "datetime2",
        //    nullable: true);

        //migrationBuilder.AddColumn<Guid>(
        //    name: "LastModifierId",
        //    table: "ElsaActivities",
        //    type: "uniqueidentifier",
        //    nullable: true);

        //migrationBuilder.CreateIndex(
        //    name: "IX_ElsaWorkflowExecutionLogs_WorkflowInstanceId",
        //    table: "ElsaWorkflowExecutionLogs",
        //    column: "WorkflowInstanceId");

        //migrationBuilder.AddForeignKey(
        //    name: "FK_ElsaActivities_ElsaWorkflowDefinitionVersions_DefinitionVersionId",
        //    table: "ElsaActivities",
        //    column: "DefinitionVersionId",
        //    principalTable: "ElsaWorkflowDefinitionVersions",
        //    principalColumn: "Id",
        //    onDelete: ReferentialAction.Cascade);

        //migrationBuilder.AddForeignKey(
        //    name: "FK_ElsaActivityConnections_ElsaWorkflowDefinitionVersions_DefinitionVersionId",
        //    table: "ElsaActivityConnections",
        //    column: "DefinitionVersionId",
        //    principalTable: "ElsaWorkflowDefinitionVersions",
        //    principalColumn: "Id",
        //    onDelete: ReferentialAction.Cascade);

        //migrationBuilder.AddForeignKey(
        //    name: "FK_ElsaWorkflowExecutionLogs_ElsaWorkflowInstances_WorkflowInstanceId",
        //    table: "ElsaWorkflowExecutionLogs",
        //    column: "WorkflowInstanceId",
        //    principalTable: "ElsaWorkflowInstances",
        //    principalColumn: "Id",
        //    onDelete: ReferentialAction.Cascade);
    }
}
