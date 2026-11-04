using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMealPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryAlertsAndReads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_inventory_alert_dismissals_alert_type",
                schema: "public",
                table: "inventory_alert_dismissals");

            migrationBuilder.CreateTable(
                name: "inventory_alerts",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pantry_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alert_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    condition_snapshot = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory_alerts", x => x.id);
                    table.CheckConstraint("ck_inventory_alerts_alert_type", "alert_type IN ('low_stock', 'out_of_stock', 'expiring_soon', 'expired')");
                    table.ForeignKey(
                        name: "fk_inventory_alerts_households_household_id",
                        column: x => x.household_id,
                        principalSchema: "public",
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_inventory_alerts_pantry_items_pantry_item_id",
                        column: x => x.pantry_item_id,
                        principalSchema: "public",
                        principalTable: "pantry_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory_alert_reads",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alert_id = table.Column<Guid>(type: "uuid", nullable: false),
                    read_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory_alert_reads", x => x.id);
                    table.ForeignKey(
                        name: "fk_inventory_alert_reads_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_inventory_alert_reads_inventory_alerts_alert_id",
                        column: x => x.alert_id,
                        principalSchema: "public",
                        principalTable: "inventory_alerts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_inventory_alert_dismissals_alert_type",
                schema: "public",
                table: "inventory_alert_dismissals",
                sql: "alert_type IN ('low_stock', 'out_of_stock', 'expiring_soon', 'expired')");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_alert_reads_alert_id",
                schema: "public",
                table: "inventory_alert_reads",
                column: "alert_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_alert_reads_user_id_alert_id",
                schema: "public",
                table: "inventory_alert_reads",
                columns: new[] { "user_id", "alert_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inventory_alerts_household_id_is_active",
                schema: "public",
                table: "inventory_alerts",
                columns: new[] { "household_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_inventory_alerts_pantry_item_id_alert_type",
                schema: "public",
                table: "inventory_alerts",
                columns: new[] { "pantry_item_id", "alert_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventory_alert_reads",
                schema: "public");

            migrationBuilder.DropTable(
                name: "inventory_alerts",
                schema: "public");

            migrationBuilder.DropCheckConstraint(
                name: "ck_inventory_alert_dismissals_alert_type",
                schema: "public",
                table: "inventory_alert_dismissals");

            migrationBuilder.AddCheckConstraint(
                name: "ck_inventory_alert_dismissals_alert_type",
                schema: "public",
                table: "inventory_alert_dismissals",
                sql: "alert_type IN ('low_stock', 'expiring', 'expired')");
        }
    }
}
