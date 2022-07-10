using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Passingwind.Abp.ElsaModule.Migrations
{
    public partial class Update_2 : Migration
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

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "ElsaWorkflowInstances",
                type: "nvarchar(max)",
                nullable: true);

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "Id",
            //    table: "ElsaWorkflowExecutionLogs",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

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
            migrationBuilder.DropForeignKey(
                name: "FK_ElsaActivities_ElsaWorkflowDefinitionVersions_WorkflowDefinitionVersionId",
                table: "ElsaActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_ElsaActivityConnections_ElsaWorkflowDefinitionVersions_WorkflowDefinitionVersionId",
                table: "ElsaActivityConnections");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "ElsaWorkflowInstances");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "ElsaWorkflowInstances");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "ElsaWorkflowExecutionLogs");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "ElsaWorkflowExecutionLogs");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "ElsaWorkflowDefinitionVersions");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "ElsaWorkflowDefinitionVersions");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "ElsaWorkflowDefinitions");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "ElsaWorkflowDefinitions");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "ElsaTriggers");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "ElsaTriggers");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "ElsaBookmarks");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "ElsaBookmarks");

            migrationBuilder.RenameColumn(
                name: "WorkflowDefinitionVersionId",
                table: "ElsaWorkflowInstances",
                newName: "DefinitionVersionId");

            migrationBuilder.RenameColumn(
                name: "WorkflowDefinitionId",
                table: "ElsaWorkflowInstances",
                newName: "DefinitionId");

            migrationBuilder.RenameColumn(
                name: "Attributes",
                table: "ElsaActivityConnections",
                newName: "Arrtibutes");

            migrationBuilder.RenameColumn(
                name: "WorkflowDefinitionVersionId",
                table: "ElsaActivityConnections",
                newName: "DefinitionVersionId");

            migrationBuilder.RenameColumn(
                name: "Attributes",
                table: "ElsaActivities",
                newName: "Arrtibutes");

            migrationBuilder.RenameColumn(
                name: "WorkflowDefinitionVersionId",
                table: "ElsaActivities",
                newName: "DefinitionVersionId");

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "ElsaWorkflowExecutionLogs",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldType: "uniqueidentifier")
            //    .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "ElsaActivityConnections",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "ElsaActivityConnections",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "ElsaActivities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "ElsaActivities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "ElsaActivities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "ElsaActivities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElsaWorkflowExecutionLogs_WorkflowInstanceId",
                table: "ElsaWorkflowExecutionLogs",
                column: "WorkflowInstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ElsaActivities_ElsaWorkflowDefinitionVersions_DefinitionVersionId",
                table: "ElsaActivities",
                column: "DefinitionVersionId",
                principalTable: "ElsaWorkflowDefinitionVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ElsaActivityConnections_ElsaWorkflowDefinitionVersions_DefinitionVersionId",
                table: "ElsaActivityConnections",
                column: "DefinitionVersionId",
                principalTable: "ElsaWorkflowDefinitionVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ElsaWorkflowExecutionLogs_ElsaWorkflowInstances_WorkflowInstanceId",
                table: "ElsaWorkflowExecutionLogs",
                column: "WorkflowInstanceId",
                principalTable: "ElsaWorkflowInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
