using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations
{
    public partial class Add_Elsa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ElsaBookmarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Hash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModelType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElsaBookmarks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElsaTriggers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Hash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModelType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElsaTriggers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElsaWorkflowDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LatestVersion = table.Column<int>(type: "int", nullable: false),
                    PublishedVersion = table.Column<int>(type: "int", nullable: true),
                    IsSingleton = table.Column<bool>(type: "bit", nullable: false),
                    DeleteCompletedInstances = table.Column<bool>(type: "bit", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Tag = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PersistenceBehavior = table.Column<int>(type: "int", nullable: false),
                    ContextOptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomAttributes = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_ElsaWorkflowDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElsaWorkflowInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefinitionVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    WorkflowStatus = table.Column<int>(type: "int", nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContextType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContextId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LastExecutedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinishedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FaultedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Input = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Output = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fault = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduledActivities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlockingActivities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scopes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentActivity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastExecutedActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActivityData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_ElsaWorkflowInstances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElsaWorkflowDefinitionVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsLatest = table.Column<bool>(type: "bit", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ElsaWorkflowDefinitionVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElsaWorkflowDefinitionVersions_ElsaWorkflowDefinitions_DefinitionId",
                        column: x => x.DefinitionId,
                        principalTable: "ElsaWorkflowDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElsaWorkflowExecutionLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElsaWorkflowExecutionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElsaWorkflowExecutionLogs_ElsaWorkflowInstances_WorkflowInstanceId",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "ElsaWorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElsaActivities",
                columns: table => new
                {
                    DefinitionVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PersistWorkflow = table.Column<bool>(type: "bit", nullable: false),
                    LoadWorkflowContext = table.Column<bool>(type: "bit", nullable: false),
                    SaveWorkflowContext = table.Column<bool>(type: "bit", nullable: false),
                    Arrtibutes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElsaActivities", x => new { x.DefinitionVersionId, x.ActivityId });
                    table.ForeignKey(
                        name: "FK_ElsaActivities_ElsaWorkflowDefinitionVersions_DefinitionVersionId",
                        column: x => x.DefinitionVersionId,
                        principalTable: "ElsaWorkflowDefinitionVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElsaActivityConnections",
                columns: table => new
                {
                    DefinitionVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElsaActivityConnections", x => new { x.DefinitionVersionId, x.SourceId, x.TargetId, x.Outcome });
                    table.ForeignKey(
                        name: "FK_ElsaActivityConnections_ElsaWorkflowDefinitionVersions_DefinitionVersionId",
                        column: x => x.DefinitionVersionId,
                        principalTable: "ElsaWorkflowDefinitionVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElsaWorkflowDefinitionVersions_DefinitionId_Version",
                table: "ElsaWorkflowDefinitionVersions",
                columns: new[] { "DefinitionId", "Version" });

            migrationBuilder.CreateIndex(
                name: "IX_ElsaWorkflowExecutionLogs_WorkflowInstanceId",
                table: "ElsaWorkflowExecutionLogs",
                column: "WorkflowInstanceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElsaActivities");

            migrationBuilder.DropTable(
                name: "ElsaActivityConnections");

            migrationBuilder.DropTable(
                name: "ElsaBookmarks");

            migrationBuilder.DropTable(
                name: "ElsaTriggers");

            migrationBuilder.DropTable(
                name: "ElsaWorkflowExecutionLogs");

            migrationBuilder.DropTable(
                name: "ElsaWorkflowDefinitionVersions");

            migrationBuilder.DropTable(
                name: "ElsaWorkflowInstances");

            migrationBuilder.DropTable(
                name: "ElsaWorkflowDefinitions");
        }
    }
}
