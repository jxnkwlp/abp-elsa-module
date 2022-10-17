using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations
{
    public partial class Update_Elsa_2 : Migration
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
