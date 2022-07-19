using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pfm.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    code = table.Column<string>(type: "text", nullable: false),
                    parent_code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    beneficiary_name = table.Column<string>(type: "text", nullable: true),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    direction = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<double>(type: "double precision", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    mcc = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    kind = table.Column<string>(type: "text", nullable: false),
                    categoryId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_transactions_categories_categoryId",
                        column: x => x.categoryId,
                        principalTable: "categories",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "transaction_splits",
                columns: table => new
                {
                    categoryId = table.Column<string>(type: "text", nullable: false),
                    transactionId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_splits", x => new { x.transactionId, x.categoryId });
                    table.ForeignKey(
                        name: "FK_transaction_splits_categories_categoryId",
                        column: x => x.categoryId,
                        principalTable: "categories",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transaction_splits_transactions_transactionId",
                        column: x => x.transactionId,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_transaction_splits_categoryId",
                table: "transaction_splits",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_categoryId",
                table: "transactions",
                column: "categoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transaction_splits");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
