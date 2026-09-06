using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EagleBank.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                id = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                name = table.Column<string>(type: "TEXT", nullable: false),
                phone_number = table.Column<string>(type: "TEXT", nullable: false),
                email = table.Column<string>(type: "TEXT", nullable: false),
                password_hash = table.Column<string>(type: "TEXT", nullable: false),
                created_timestamp = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                updated_timestamp = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                address_line1 = table.Column<string>(type: "TEXT", nullable: false),
                address_line2 = table.Column<string>(type: "TEXT", nullable: true),
                address_line3 = table.Column<string>(type: "TEXT", nullable: true),
                address_town = table.Column<string>(type: "TEXT", nullable: false),
                address_county = table.Column<string>(type: "TEXT", nullable: false),
                address_postcode = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "accounts",
            columns: table => new
            {
                account_number = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                user_id = table.Column<string>(type: "TEXT", nullable: false),
                name = table.Column<string>(type: "TEXT", nullable: false),
                account_type = table.Column<string>(type: "TEXT", nullable: false),
                sort_code = table.Column<string>(type: "TEXT", nullable: false),
                currency = table.Column<string>(type: "TEXT", nullable: false),
                balance_pence = table.Column<long>(type: "INTEGER", nullable: false),
                created_timestamp = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                updated_timestamp = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_accounts", x => x.account_number);
                table.ForeignKey(
                    name: "FK_accounts_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "transactions",
            columns: table => new
            {
                id = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                account_number = table.Column<string>(type: "TEXT", nullable: false),
                user_id = table.Column<string>(type: "TEXT", nullable: false),
                amount_pence = table.Column<long>(type: "INTEGER", nullable: false),
                currency = table.Column<string>(type: "TEXT", nullable: false),
                type = table.Column<string>(type: "TEXT", nullable: false),
                reference = table.Column<string>(type: "TEXT", nullable: true),
                created_timestamp = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_transactions", x => x.id);
                table.ForeignKey(
                    name: "FK_transactions_accounts_account_number",
                    column: x => x.account_number,
                    principalTable: "accounts",
                    principalColumn: "account_number",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_users_email",
            table: "users",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_accounts_user_id",
            table: "accounts",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "IX_transactions_account_number",
            table: "transactions",
            column: "account_number");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "transactions");
        migrationBuilder.DropTable(name: "accounts");
        migrationBuilder.DropTable(name: "users");
    }
}
