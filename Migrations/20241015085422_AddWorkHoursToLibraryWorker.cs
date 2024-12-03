using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotekosValdymoSistema.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkHoursToLibraryWorker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkHours",
                table: "LibraryWorkers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkHours",
                table: "LibraryWorkers");
        }
    }
}
