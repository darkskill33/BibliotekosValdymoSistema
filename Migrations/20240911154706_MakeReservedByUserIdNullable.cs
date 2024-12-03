using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotekosValdymoSistema.Migrations
{
    /// <inheritdoc />
    public partial class MakeReservedByUserIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReservedByUserId",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true, // Change to true to allow nulls
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReservedByUserId",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false, // Revert to non-nullable if rolling back
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }

}
