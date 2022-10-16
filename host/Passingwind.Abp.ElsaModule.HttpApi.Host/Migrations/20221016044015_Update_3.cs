using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Passingwind.Abp.ElsaModule.Migrations
{
    public partial class Update_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "ActivityData",
            //    table: "ElsaWorkflowInstances");

            //migrationBuilder.DropColumn(
            //    name: "BlockingActivities",
            //    table: "ElsaWorkflowInstances");

            //migrationBuilder.DropColumn(
            //    name: "Metadata",
            //    table: "ElsaWorkflowInstances");

            //migrationBuilder.DropColumn(
            //    name: "ScheduledActivities",
            //    table: "ElsaWorkflowInstances");

            //migrationBuilder.DropColumn(
            //    name: "Scopes",
            //    table: "ElsaWorkflowInstances");

            //migrationBuilder.DropColumn(
            //    name: "Variables",
            //    table: "ElsaWorkflowInstances");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElsaWorkflowInstanceActivityData");

            migrationBuilder.DropTable(
                name: "ElsaWorkflowInstanceBlockingActivities");

            migrationBuilder.DropTable(
                name: "ElsaWorkflowInstanceMetadata");

            migrationBuilder.DropTable(
                name: "ElsaWorkflowInstanceScheduledActivities");

            migrationBuilder.DropTable(
                name: "ElsaWorkflowInstanceScopes");

            migrationBuilder.DropTable(
                name: "ElsaWorkflowInstanceVariables");

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

            //migrationBuilder.AddColumn<string>(
            //    name: "Variables",
            //    table: "ElsaWorkflowInstances",
            //    type: "nvarchar(max)",
            //    nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ActivityType",
                table: "ElsaWorkflowExecutionLogs",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
