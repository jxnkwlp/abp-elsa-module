using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations;

/// <inheritdoc />
public partial class Add_Index : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_ElsaWorkflowInstanceVariables_Key",
            table: "ElsaWorkflowInstanceVariables",
            column: "Key");

        migrationBuilder.CreateIndex(
            name: "IX_ElsaWorkflowInstances_Name",
            table: "ElsaWorkflowInstances",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_ElsaWorkflowInstances_WorkflowDefinitionId",
            table: "ElsaWorkflowInstances",
            column: "WorkflowDefinitionId");

        migrationBuilder.CreateIndex(
            name: "IX_ElsaWorkflowInstances_WorkflowDefinitionId_WorkflowDefinitionVersionId",
            table: "ElsaWorkflowInstances",
            columns: new[] { "WorkflowDefinitionId", "WorkflowDefinitionVersionId" });

        migrationBuilder.CreateIndex(
            name: "IX_ElsaWorkflowInstances_WorkflowStatus",
            table: "ElsaWorkflowInstances",
            column: "WorkflowStatus");

        migrationBuilder.CreateIndex(
            name: "IX_ElsaWorkflowGroups_Name",
            table: "ElsaWorkflowGroups",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_ElsaWorkflowGroups_RoleName",
            table: "ElsaWorkflowGroups",
            column: "RoleName");

        migrationBuilder.CreateIndex(
            name: "IX_ElsaWorkflowExecutionLogs_WorkflowInstanceId",
            table: "ElsaWorkflowExecutionLogs",
            column: "WorkflowInstanceId");

        migrationBuilder.CreateIndex(
            name: "IX_ElsaWorkflowDefinitions_Name",
            table: "ElsaWorkflowDefinitions",
            column: "Name");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_ElsaWorkflowInstanceVariables_Key",
            table: "ElsaWorkflowInstanceVariables");

        migrationBuilder.DropIndex(
            name: "IX_ElsaWorkflowInstances_Name",
            table: "ElsaWorkflowInstances");

        migrationBuilder.DropIndex(
            name: "IX_ElsaWorkflowInstances_WorkflowDefinitionId",
            table: "ElsaWorkflowInstances");

        migrationBuilder.DropIndex(
            name: "IX_ElsaWorkflowInstances_WorkflowDefinitionId_WorkflowDefinitionVersionId",
            table: "ElsaWorkflowInstances");

        migrationBuilder.DropIndex(
            name: "IX_ElsaWorkflowInstances_WorkflowStatus",
            table: "ElsaWorkflowInstances");

        migrationBuilder.DropIndex(
            name: "IX_ElsaWorkflowGroups_Name",
            table: "ElsaWorkflowGroups");

        migrationBuilder.DropIndex(
            name: "IX_ElsaWorkflowGroups_RoleName",
            table: "ElsaWorkflowGroups");

        migrationBuilder.DropIndex(
            name: "IX_ElsaWorkflowExecutionLogs_WorkflowInstanceId",
            table: "ElsaWorkflowExecutionLogs");

        migrationBuilder.DropIndex(
            name: "IX_ElsaWorkflowDefinitions_Name",
            table: "ElsaWorkflowDefinitions");
    }
}
