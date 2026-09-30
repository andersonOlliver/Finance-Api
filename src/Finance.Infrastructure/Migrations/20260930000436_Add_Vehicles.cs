using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Vehicles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "vehicle_id",
                table: "transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nickname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    license_plate = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    mileage = table.Column<decimal>(type: "numeric", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicles", x => x.id);
                    table.ForeignKey(
                        name: "fk_vehicles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_refuels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    liters = table.Column<decimal>(type: "numeric", nullable: false),
                    price_per_liter = table.Column<decimal>(type: "numeric", nullable: false),
                    mileage = table.Column<decimal>(type: "numeric", nullable: false),
                    consumption_since_last_refuel = table.Column<decimal>(type: "numeric", nullable: true),
                    refueled_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle_refuels", x => x.id);
                    table.ForeignKey(
                        name: "fk_vehicle_refuels_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_vehicle_refuels_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_transactions_vehicle_id",
                table: "transactions",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_refuels_transaction_id",
                table: "vehicle_refuels",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_refuels_vehicle_id",
                table: "vehicle_refuels",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_user_id",
                table: "vehicles",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_vehicle_vehicle_id",
                table: "transactions",
                column: "vehicle_id",
                principalTable: "vehicles",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transactions_vehicle_vehicle_id",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "vehicle_refuels");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DropIndex(
                name: "ix_transactions_vehicle_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "vehicle_id",
                table: "transactions");
        }
    }
}
