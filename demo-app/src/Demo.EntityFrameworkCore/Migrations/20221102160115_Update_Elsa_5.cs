using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations
{
    public partial class Update_Elsa_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ElsaWorkflowInstanceFaults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FaultedActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Resuming = table.Column<bool>(type: "bit", nullable: false),
                    ActivityInput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElsaWorkflowInstanceFaults", x => new { x.Id, x.WorkflowInstanceId });
                    table.ForeignKey(
                        name: "FK_ElsaWorkflowInstanceFaults_ElsaWorkflowInstances_WorkflowInstanceId",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "ElsaWorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElsaGlobalVariables_Key",
                table: "ElsaGlobalVariables",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_ElsaWorkflowInstanceFaults_WorkflowInstanceId",
                table: "ElsaWorkflowInstanceFaults",
                column: "WorkflowInstanceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElsaWorkflowInstanceFaults");

            migrationBuilder.DropIndex(
                name: "IX_ElsaGlobalVariables_Key",
                table: "ElsaGlobalVariables");
        }
    }
}
