using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pfm.Migrations
{
    public partial class CategoryRefrence_To_Category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_categories_parent_code",
                table: "categories",
                column: "parent_code");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categories_parent_code",
                table: "categories",
                column: "parent_code",
                principalTable: "categories",
                principalColumn: "code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_categories_parent_code",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_categories_parent_code",
                table: "categories");
        }
    }
}
