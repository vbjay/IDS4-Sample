using Microsoft.EntityFrameworkCore.Migrations;

namespace AdminUI.Admin.EntityFramework.SqlServer.Migrations.Identity
{
    public partial class Fidouselong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_FidoCredentials", "FidoCredentials");
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "FidoCredentials",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddPrimaryKey("PK_FidoCredentials", "FidoCredentials", "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "FidoCredentials",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
