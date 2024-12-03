using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotekosValdymoSistema.Migrations
{
    /// <inheritdoc />
    public partial class AddLibraryWorkerBooksRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LibraryWorkerBooks",
                columns: table => new
                {
                    BooksId = table.Column<int>(type: "int", nullable: false),
                    LibraryWorkersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryWorkerBooks", x => new { x.BooksId, x.LibraryWorkersId });
                    table.ForeignKey(
                        name: "FK_LibraryWorkerBooks_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LibraryWorkerBooks_LibraryWorkers_LibraryWorkersId",
                        column: x => x.LibraryWorkersId,
                        principalTable: "LibraryWorkers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryWorkerBooks_LibraryWorkersId",
                table: "LibraryWorkerBooks",
                column: "LibraryWorkersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LibraryWorkerBooks");
        }
    }
}
