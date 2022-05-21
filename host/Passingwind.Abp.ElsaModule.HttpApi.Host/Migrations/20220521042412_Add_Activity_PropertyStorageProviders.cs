using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Passingwind.Abp.ElsaModule.Migrations
{
    public partial class Add_Activity_PropertyStorageProviders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElsaWorkflowDefinitionVersions_ElsaWorkflowDefinitions_DefinitionId",
                table: "ElsaWorkflowDefinitionVersions");

            migrationBuilder.AddColumn<string>(
                name: "PropertyStorageProviders",
                table: "ElsaActivities",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyStorageProviders",
                table: "ElsaActivities");

            migrationBuilder.AddForeignKey(
                name: "FK_ElsaWorkflowDefinitionVersions_ElsaWorkflowDefinitions_DefinitionId",
                table: "ElsaWorkflowDefinitionVersions",
                column: "DefinitionId",
                principalTable: "ElsaWorkflowDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
