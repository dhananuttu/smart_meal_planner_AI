using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMealPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LinkPantryItemsToIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pantry_items_pantry_categories_category_id",
                schema: "public",
                table: "pantry_items");

            migrationBuilder.DropIndex(
                name: "ix_pantry_items_category_id",
                schema: "public",
                table: "pantry_items");

            migrationBuilder.DropIndex(
                name: "ix_pantry_items_household_id_category_id",
                schema: "public",
                table: "pantry_items");

            migrationBuilder.DropIndex(
                name: "ix_pantry_items_household_id_name",
                schema: "public",
                table: "pantry_items");

            migrationBuilder.DropColumn(
                name: "category_id",
                schema: "public",
                table: "pantry_items");

            migrationBuilder.DropColumn(
                name: "name",
                schema: "public",
                table: "pantry_items");

            migrationBuilder.DropColumn(
                name: "notes",
                schema: "public",
                table: "pantry_items");

            migrationBuilder.AddColumn<Guid>(
                name: "ingredient_id",
                schema: "public",
                table: "pantry_items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_pantry_items_household_id_ingredient_id",
                schema: "public",
                table: "pantry_items",
                columns: new[] { "household_id", "ingredient_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pantry_items_ingredient_id",
                schema: "public",
                table: "pantry_items",
                column: "ingredient_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pantry_items_ingredients_ingredient_id",
                schema: "public",
                table: "pantry_items",
                column: "ingredient_id",
                principalSchema: "public",
                principalTable: "ingredients",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pantry_items_ingredients_ingredient_id",
                schema: "public",
                table: "pantry_items");

            migrationBuilder.DropIndex(
                name: "ix_pantry_items_household_id_ingredient_id",
                schema: "public",
                table: "pantry_items");

            migrationBuilder.DropIndex(
                name: "ix_pantry_items_ingredient_id",
                schema: "public",
                table: "pantry_items");

            migrationBuilder.DropColumn(
                name: "ingredient_id",
                schema: "public",
                table: "pantry_items");

            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                schema: "public",
                table: "pantry_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                schema: "public",
                table: "pantry_items",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "notes",
                schema: "public",
                table: "pantry_items",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_pantry_items_category_id",
                schema: "public",
                table: "pantry_items",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_pantry_items_household_id_category_id",
                schema: "public",
                table: "pantry_items",
                columns: new[] { "household_id", "category_id" });

            migrationBuilder.CreateIndex(
                name: "ix_pantry_items_household_id_name",
                schema: "public",
                table: "pantry_items",
                columns: new[] { "household_id", "name" });

            migrationBuilder.AddForeignKey(
                name: "fk_pantry_items_pantry_categories_category_id",
                schema: "public",
                table: "pantry_items",
                column: "category_id",
                principalSchema: "public",
                principalTable: "pantry_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
