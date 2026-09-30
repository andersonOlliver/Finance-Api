using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Credit_Cards_And_Installment_Purchases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "credit_card_id",
                table: "transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "installment_number",
                table: "transactions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "installment_purchase_id",
                table: "transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "credit_cards",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nickname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    brand = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    due_day = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_credit_cards", x => x.id);
                    table.ForeignKey(
                        name: "fk_credit_cards_user_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "installment_purchases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    total_amount_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    total_amount_currency = table.Column<string>(type: "text", nullable: false),
                    installment_count = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    credit_card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_installment_released_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_installment_purchases", x => x.id);
                    table.ForeignKey(
                        name: "fk_installment_purchases_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_installment_purchases_credit_cards_credit_card_id",
                        column: x => x.credit_card_id,
                        principalTable: "credit_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_installment_purchases_user_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_transactions_credit_card_id",
                table: "transactions",
                column: "credit_card_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_installment_purchase_id",
                table: "transactions",
                column: "installment_purchase_id");

            migrationBuilder.CreateIndex(
                name: "ix_credit_cards_user_id",
                table: "credit_cards",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_installment_purchases_category_id",
                table: "installment_purchases",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_installment_purchases_credit_card_id",
                table: "installment_purchases",
                column: "credit_card_id");

            migrationBuilder.CreateIndex(
                name: "ix_installment_purchases_user_id",
                table: "installment_purchases",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_credit_cards_credit_card_id",
                table: "transactions",
                column: "credit_card_id",
                principalTable: "credit_cards",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_installment_purchases_installment_purchase_id",
                table: "transactions",
                column: "installment_purchase_id",
                principalTable: "installment_purchases",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transactions_credit_cards_credit_card_id",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_installment_purchases_installment_purchase_id",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "installment_purchases");

            migrationBuilder.DropTable(
                name: "credit_cards");

            migrationBuilder.DropIndex(
                name: "ix_transactions_credit_card_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "ix_transactions_installment_purchase_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "credit_card_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "installment_number",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "installment_purchase_id",
                table: "transactions");
        }
    }
}
