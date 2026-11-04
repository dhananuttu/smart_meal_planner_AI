using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMealPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeCategoriesAndInstructions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                schema: "public",
                table: "recipes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "instructions",
                schema: "public",
                table: "recipes",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "recipe_categories",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipe_categories_asp_net_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_recipe_categories_households_household_id",
                        column: x => x.household_id,
                        principalSchema: "public",
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_recipes_category_id",
                schema: "public",
                table: "recipes",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipes_household_id_category_id",
                schema: "public",
                table: "recipes",
                columns: new[] { "household_id", "category_id" });

            migrationBuilder.CreateIndex(
                name: "ix_recipe_categories_created_by_user_id",
                schema: "public",
                table: "recipe_categories",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_categories_household_id_name",
                schema: "public",
                table: "recipe_categories",
                columns: new[] { "household_id", "name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_recipes_recipe_categories_category_id",
                schema: "public",
                table: "recipes",
                column: "category_id",
                principalSchema: "public",
                principalTable: "recipe_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_recipes_recipe_categories_category_id",
                schema: "public",
                table: "recipes");

            migrationBuilder.DropTable(
                name: "recipe_categories",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "ix_recipes_category_id",
                schema: "public",
                table: "recipes");

            migrationBuilder.DropIndex(
                name: "ix_recipes_household_id_category_id",
                schema: "public",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "category_id",
                schema: "public",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "instructions",
                schema: "public",
                table: "recipes");
        }
    }
}
