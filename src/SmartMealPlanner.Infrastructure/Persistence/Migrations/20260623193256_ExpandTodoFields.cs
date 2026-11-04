using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMealPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExpandTodoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "category",
                schema: "public",
                table: "todos",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Personal");

            migrationBuilder.AddColumn<bool>(
                name: "is_completed",
                schema: "public",
                table: "todos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_recurring",
                schema: "public",
                table: "todos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "recurrence_type",
                schema: "public",
                table: "todos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE todos
                SET is_completed = (status = 'Completed');
                """);

            migrationBuilder.DropIndex(
                name: "ix_todos_user_id_status_is_deleted",
                schema: "public",
                table: "todos");

            migrationBuilder.DropCheckConstraint(
                name: "ck_todos_status",
                schema: "public",
                table: "todos");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "public",
                table: "todos");

            migrationBuilder.Sql(
                """
                ALTER TABLE todos
                ALTER COLUMN priority TYPE character varying(20)
                USING (
                    CASE COALESCE(priority, 0)
                        WHEN 1 THEN 'Low'
                        WHEN 2 THEN 'High'
                        WHEN 3 THEN 'Critical'
                        ELSE 'Medium'
                    END
                );
                """);

            migrationBuilder.AlterColumn<string>(
                name: "priority",
                schema: "public",
                table: "todos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Medium",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "ix_todos_user_id_is_completed_is_deleted",
                schema: "public",
                table: "todos",
                columns: new[] { "user_id", "is_completed", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_todos_user_id_is_recurring_is_deleted",
                schema: "public",
                table: "todos",
                columns: new[] { "user_id", "is_recurring", "is_deleted" });

            migrationBuilder.AddCheckConstraint(
                name: "ck_todos_category",
                schema: "public",
                table: "todos",
                sql: "category IN ('MealPreparation', 'Shopping', 'Household', 'Personal')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_todos_priority",
                schema: "public",
                table: "todos",
                sql: "priority IN ('Low', 'Medium', 'High', 'Critical')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_todos_recurrence_type",
                schema: "public",
                table: "todos",
                sql: "recurrence_type IS NULL OR recurrence_type IN ('Daily', 'Weekly', 'Monthly', 'Yearly')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_todos_user_id_is_completed_is_deleted",
                schema: "public",
                table: "todos");

            migrationBuilder.DropIndex(
                name: "ix_todos_user_id_is_recurring_is_deleted",
                schema: "public",
                table: "todos");

            migrationBuilder.DropCheckConstraint(
                name: "ck_todos_category",
                schema: "public",
                table: "todos");

            migrationBuilder.DropCheckConstraint(
                name: "ck_todos_priority",
                schema: "public",
                table: "todos");

            migrationBuilder.DropCheckConstraint(
                name: "ck_todos_recurrence_type",
                schema: "public",
                table: "todos");

            migrationBuilder.Sql(
                """
                ALTER TABLE todos
                ALTER COLUMN priority TYPE integer
                USING (
                    CASE priority
                        WHEN 'Low' THEN 1
                        WHEN 'High' THEN 2
                        WHEN 'Critical' THEN 3
                        ELSE 0
                    END
                );
                """);

            migrationBuilder.AlterColumn<int>(
                name: "priority",
                schema: "public",
                table: "todos",
                type: "integer",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Medium");

            migrationBuilder.AddColumn<string>(
                name: "status",
                schema: "public",
                table: "todos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.Sql(
                """
                UPDATE todos
                SET status = CASE WHEN is_completed THEN 'Completed' ELSE 'Pending' END;
                """);

            migrationBuilder.DropColumn(
                name: "category",
                schema: "public",
                table: "todos");

            migrationBuilder.DropColumn(
                name: "is_completed",
                schema: "public",
                table: "todos");

            migrationBuilder.DropColumn(
                name: "is_recurring",
                schema: "public",
                table: "todos");

            migrationBuilder.DropColumn(
                name: "recurrence_type",
                schema: "public",
                table: "todos");

            migrationBuilder.CreateIndex(
                name: "ix_todos_user_id_status_is_deleted",
                schema: "public",
                table: "todos",
                columns: new[] { "user_id", "status", "is_deleted" });

            migrationBuilder.AddCheckConstraint(
                name: "ck_todos_status",
                schema: "public",
                table: "todos",
                sql: "status IN ('Pending', 'Completed')");
        }
    }
}
