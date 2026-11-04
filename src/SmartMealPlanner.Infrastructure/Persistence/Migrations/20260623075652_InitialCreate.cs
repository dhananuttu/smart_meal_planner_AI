using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartMealPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "asp_net_roles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_users",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_role_claims",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "public",
                        principalTable: "asp_net_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_claims",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_logins",
                schema: "public",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_roles",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "public",
                        principalTable: "asp_net_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_tokens",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "households",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_households", x => x.id);
                    table.ForeignKey(
                        name: "fk_households_asp_net_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    replaced_by_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "household_members",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "member"),
                    display_label = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    joined_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_household_members", x => x.id);
                    table.CheckConstraint("ck_household_members_role", "role IN ('owner', 'member')");
                    table.ForeignKey(
                        name: "fk_household_members_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_household_members_households_household_id",
                        column: x => x.household_id,
                        principalSchema: "public",
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "meal_plans",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    week_start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meal_plans", x => x.id);
                    table.ForeignKey(
                        name: "fk_meal_plans_asp_net_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_meal_plans_households_household_id",
                        column: x => x.household_id,
                        principalSchema: "public",
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pantry_categories",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    household_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_system = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pantry_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_pantry_categories_asp_net_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_pantry_categories_households_household_id",
                        column: x => x.household_id,
                        principalSchema: "public",
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipes",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    servings = table.Column<int>(type: "integer", nullable: false, defaultValue: 4),
                    prep_time_minutes = table.Column<int>(type: "integer", nullable: true),
                    cook_time_minutes = table.Column<int>(type: "integer", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipes", x => x.id);
                    table.CheckConstraint("ck_recipes_cook_time_minutes", "cook_time_minutes IS NULL OR (cook_time_minutes >= 0 AND cook_time_minutes <= 1440)");
                    table.CheckConstraint("ck_recipes_prep_time_minutes", "prep_time_minutes IS NULL OR (prep_time_minutes >= 0 AND prep_time_minutes <= 1440)");
                    table.CheckConstraint("ck_recipes_servings", "servings >= 1 AND servings <= 50");
                    table.ForeignKey(
                        name: "fk_recipes_asp_net_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_recipes_households_household_id",
                        column: x => x.household_id,
                        principalSchema: "public",
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    avatar_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    dietary_preferences = table.Column<List<string>>(type: "text[]", nullable: false),
                    default_servings = table.Column<int>(type: "integer", nullable: false, defaultValue: 4),
                    timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "UTC"),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_profiles", x => x.id);
                    table.CheckConstraint("ck_user_profiles_default_servings", "default_servings >= 1 AND default_servings <= 20");
                    table.ForeignKey(
                        name: "fk_user_profiles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_profiles_households_household_id",
                        column: x => x.household_id,
                        principalSchema: "public",
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "grocery_lists",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    meal_plan_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grocery_lists", x => x.id);
                    table.ForeignKey(
                        name: "fk_grocery_lists_asp_net_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_grocery_lists_households_household_id",
                        column: x => x.household_id,
                        principalSchema: "public",
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_grocery_lists_meal_plans_meal_plan_id",
                        column: x => x.meal_plan_id,
                        principalSchema: "public",
                        principalTable: "meal_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "pantry_items",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    low_stock_threshold = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    expiry_date = table.Column<DateOnly>(type: "date", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pantry_items", x => x.id);
                    table.CheckConstraint("ck_pantry_items_low_stock_threshold", "low_stock_threshold IS NULL OR low_stock_threshold >= 0");
                    table.CheckConstraint("ck_pantry_items_quantity", "quantity >= 0");
                    table.ForeignKey(
                        name: "fk_pantry_items_asp_net_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pantry_items_households_household_id",
                        column: x => x.household_id,
                        principalSchema: "public",
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pantry_items_pantry_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "public",
                        principalTable: "pantry_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "meal_plan_entries",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    meal_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: true),
                    entry_date = table.Column<DateOnly>(type: "date", nullable: false),
                    meal_slot = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meal_plan_entries", x => x.id);
                    table.CheckConstraint("ck_meal_plan_entries_meal_slot", "meal_slot IN ('Breakfast', 'Lunch', 'Dinner', 'Snack')");
                    table.ForeignKey(
                        name: "fk_meal_plan_entries_meal_plans_meal_plan_id",
                        column: x => x.meal_plan_id,
                        principalSchema: "public",
                        principalTable: "meal_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_meal_plan_entries_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "public",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "recipe_ingredients",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_ingredients", x => x.id);
                    table.CheckConstraint("ck_recipe_ingredients_quantity", "quantity >= 0");
                    table.ForeignKey(
                        name: "fk_recipe_ingredients_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "public",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_steps",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_number = table.Column<int>(type: "integer", nullable: false),
                    instruction = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_steps", x => x.id);
                    table.CheckConstraint("ck_recipe_steps_step_number", "step_number >= 1");
                    table.ForeignKey(
                        name: "fk_recipe_steps_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "public",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_tags",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tag = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_tags", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipe_tags_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "public",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "grocery_list_items",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    grocery_list_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 1m),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_checked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    source_recipe_id = table.Column<Guid>(type: "uuid", nullable: true),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grocery_list_items", x => x.id);
                    table.CheckConstraint("ck_grocery_list_items_quantity", "quantity > 0");
                    table.ForeignKey(
                        name: "fk_grocery_list_items_grocery_lists_grocery_list_id",
                        column: x => x.grocery_list_id,
                        principalSchema: "public",
                        principalTable: "grocery_lists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_grocery_list_items_recipes_source_recipe_id",
                        column: x => x.source_recipe_id,
                        principalSchema: "public",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "inventory_alert_dismissals",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pantry_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alert_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    dismissed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    condition_snapshot = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory_alert_dismissals", x => x.id);
                    table.CheckConstraint("ck_inventory_alert_dismissals_alert_type", "alert_type IN ('low_stock', 'expiring', 'expired')");
                    table.ForeignKey(
                        name: "fk_inventory_alert_dismissals_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_inventory_alert_dismissals_pantry_items_pantry_item_id",
                        column: x => x.pantry_item_id,
                        principalSchema: "public",
                        principalTable: "pantry_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "todos",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    due_date = table.Column<DateOnly>(type: "date", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    meal_plan_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_todos", x => x.id);
                    table.CheckConstraint("ck_todos_status", "status IN ('Pending', 'Completed')");
                    table.ForeignKey(
                        name: "fk_todos_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_todos_meal_plan_entries_meal_plan_entry_id",
                        column: x => x.meal_plan_entry_id,
                        principalSchema: "public",
                        principalTable: "meal_plan_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "asp_net_roles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "seed-user-role", "User", "USER" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "seed-admin-role", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "pantry_categories",
                columns: new[] { "id", "created_at", "created_by_user_id", "household_id", "is_system", "name", "sort_order", "updated_at" },
                values: new object[,]
                {
                    { new Guid("c1000001-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, true, "Produce", 1, new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("c1000001-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, true, "Dairy", 2, new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("c1000001-0000-0000-0000-000000000003"), new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, true, "Meat", 3, new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("c1000001-0000-0000-0000-000000000004"), new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, true, "Pantry Staples", 4, new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("c1000001-0000-0000-0000-000000000005"), new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, true, "Frozen", 5, new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("c1000001-0000-0000-0000-000000000006"), new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, true, "Beverages", 6, new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("c1000001-0000-0000-0000-000000000099"), new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, true, "Other", 99, new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                schema: "public",
                table: "asp_net_role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "public",
                table: "asp_net_roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                schema: "public",
                table: "asp_net_user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                schema: "public",
                table: "asp_net_user_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_roles_role_id",
                schema: "public",
                table: "asp_net_user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "public",
                table: "asp_net_users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "public",
                table: "asp_net_users",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_grocery_list_items_grocery_list_id",
                schema: "public",
                table: "grocery_list_items",
                column: "grocery_list_id");

            migrationBuilder.CreateIndex(
                name: "ix_grocery_list_items_grocery_list_id_is_checked",
                schema: "public",
                table: "grocery_list_items",
                columns: new[] { "grocery_list_id", "is_checked" });

            migrationBuilder.CreateIndex(
                name: "ix_grocery_list_items_source_recipe_id",
                schema: "public",
                table: "grocery_list_items",
                column: "source_recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_grocery_lists_created_by_user_id",
                schema: "public",
                table: "grocery_lists",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_grocery_lists_household_id_is_archived_created_at",
                schema: "public",
                table: "grocery_lists",
                columns: new[] { "household_id", "is_archived", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_grocery_lists_meal_plan_id",
                schema: "public",
                table: "grocery_lists",
                column: "meal_plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_household_members_household_id",
                schema: "public",
                table: "household_members",
                column: "household_id",
                unique: true,
                filter: "role = 'owner'");

            migrationBuilder.CreateIndex(
                name: "ix_household_members_household_id_user_id",
                schema: "public",
                table: "household_members",
                columns: new[] { "household_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_household_members_user_id",
                schema: "public",
                table: "household_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_households_created_by_user_id",
                schema: "public",
                table: "households",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_alert_dismissals_pantry_item_id",
                schema: "public",
                table: "inventory_alert_dismissals",
                column: "pantry_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_alert_dismissals_user_id_dismissed_at",
                schema: "public",
                table: "inventory_alert_dismissals",
                columns: new[] { "user_id", "dismissed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_inventory_alert_dismissals_user_id_pantry_item_id_alert_type",
                schema: "public",
                table: "inventory_alert_dismissals",
                columns: new[] { "user_id", "pantry_item_id", "alert_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_meal_plan_entries_entry_date_meal_slot",
                schema: "public",
                table: "meal_plan_entries",
                columns: new[] { "entry_date", "meal_slot" });

            migrationBuilder.CreateIndex(
                name: "ix_meal_plan_entries_meal_plan_id",
                schema: "public",
                table: "meal_plan_entries",
                column: "meal_plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_meal_plan_entries_meal_plan_id_entry_date_meal_slot",
                schema: "public",
                table: "meal_plan_entries",
                columns: new[] { "meal_plan_id", "entry_date", "meal_slot" });

            migrationBuilder.CreateIndex(
                name: "ix_meal_plan_entries_recipe_id",
                schema: "public",
                table: "meal_plan_entries",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_meal_plans_created_by_user_id",
                schema: "public",
                table: "meal_plans",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_meal_plans_household_id_week_start_date",
                schema: "public",
                table: "meal_plans",
                columns: new[] { "household_id", "week_start_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pantry_categories_created_by_user_id",
                schema: "public",
                table: "pantry_categories",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_pantry_categories_household_id_is_system",
                schema: "public",
                table: "pantry_categories",
                columns: new[] { "household_id", "is_system" });

            migrationBuilder.CreateIndex(
                name: "ix_pantry_categories_household_id_name",
                schema: "public",
                table: "pantry_categories",
                columns: new[] { "household_id", "name" },
                unique: true,
                filter: "household_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_pantry_items_category_id",
                schema: "public",
                table: "pantry_items",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_pantry_items_created_by_user_id",
                schema: "public",
                table: "pantry_items",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_pantry_items_household_id_category_id",
                schema: "public",
                table: "pantry_items",
                columns: new[] { "household_id", "category_id" });

            migrationBuilder.CreateIndex(
                name: "ix_pantry_items_household_id_expiry_date",
                schema: "public",
                table: "pantry_items",
                columns: new[] { "household_id", "expiry_date" },
                filter: "expiry_date IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_pantry_items_household_id_name",
                schema: "public",
                table: "pantry_items",
                columns: new[] { "household_id", "name" });

            migrationBuilder.CreateIndex(
                name: "ix_pantry_items_household_id_quantity_low_stock_threshold",
                schema: "public",
                table: "pantry_items",
                columns: new[] { "household_id", "quantity", "low_stock_threshold" },
                filter: "low_stock_threshold IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_ingredients_recipe_id",
                schema: "public",
                table: "recipe_ingredients",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_ingredients_recipe_id_sort_order",
                schema: "public",
                table: "recipe_ingredients",
                columns: new[] { "recipe_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_recipe_steps_recipe_id",
                schema: "public",
                table: "recipe_steps",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_steps_recipe_id_step_number",
                schema: "public",
                table: "recipe_steps",
                columns: new[] { "recipe_id", "step_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_recipe_tags_recipe_id",
                schema: "public",
                table: "recipe_tags",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_tags_recipe_id_tag",
                schema: "public",
                table: "recipe_tags",
                columns: new[] { "recipe_id", "tag" });

            migrationBuilder.CreateIndex(
                name: "ix_recipe_tags_tag",
                schema: "public",
                table: "recipe_tags",
                column: "tag");

            migrationBuilder.CreateIndex(
                name: "ix_recipes_created_by_user_id",
                schema: "public",
                table: "recipes",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipes_household_id_is_deleted",
                schema: "public",
                table: "recipes",
                columns: new[] { "household_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_recipes_household_id_title",
                schema: "public",
                table: "recipes",
                columns: new[] { "household_id", "title" });

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token",
                schema: "public",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                schema: "public",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_todos_meal_plan_entry_id",
                schema: "public",
                table: "todos",
                column: "meal_plan_entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_todos_user_id_due_date",
                schema: "public",
                table: "todos",
                columns: new[] { "user_id", "due_date" },
                filter: "due_date IS NOT NULL AND is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_todos_user_id_status_is_deleted",
                schema: "public",
                table: "todos",
                columns: new[] { "user_id", "status", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_user_profiles_household_id",
                schema: "public",
                table: "user_profiles",
                column: "household_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_profiles_user_id",
                schema: "public",
                table: "user_profiles",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asp_net_role_claims",
                schema: "public");

            migrationBuilder.DropTable(
                name: "asp_net_user_claims",
                schema: "public");

            migrationBuilder.DropTable(
                name: "asp_net_user_logins",
                schema: "public");

            migrationBuilder.DropTable(
                name: "asp_net_user_roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "asp_net_user_tokens",
                schema: "public");

            migrationBuilder.DropTable(
                name: "grocery_list_items",
                schema: "public");

            migrationBuilder.DropTable(
                name: "household_members",
                schema: "public");

            migrationBuilder.DropTable(
                name: "inventory_alert_dismissals",
                schema: "public");

            migrationBuilder.DropTable(
                name: "recipe_ingredients",
                schema: "public");

            migrationBuilder.DropTable(
                name: "recipe_steps",
                schema: "public");

            migrationBuilder.DropTable(
                name: "recipe_tags",
                schema: "public");

            migrationBuilder.DropTable(
                name: "refresh_tokens",
                schema: "public");

            migrationBuilder.DropTable(
                name: "todos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_profiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "asp_net_roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "grocery_lists",
                schema: "public");

            migrationBuilder.DropTable(
                name: "pantry_items",
                schema: "public");

            migrationBuilder.DropTable(
                name: "meal_plan_entries",
                schema: "public");

            migrationBuilder.DropTable(
                name: "pantry_categories",
                schema: "public");

            migrationBuilder.DropTable(
                name: "meal_plans",
                schema: "public");

            migrationBuilder.DropTable(
                name: "recipes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "households",
                schema: "public");

            migrationBuilder.DropTable(
                name: "asp_net_users",
                schema: "public");
        }
    }
}
