using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Passingwind.Abp.ElsaModule.Migrations
{
    public partial class Add_ActivityConnection_Arrtibutes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Arrtibutes",
                table: "ElsaActivityConnections",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Arrtibutes",
                table: "ElsaActivityConnections");
        }
    }
}
