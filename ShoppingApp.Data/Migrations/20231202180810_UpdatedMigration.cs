using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "OrderHeaders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Uid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Uid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_AccountId",
                table: "OrderHeaders",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_Accounts_AccountId",
                table: "OrderHeaders",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Uid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_Accounts_AccountId",
                table: "OrderHeaders");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_AccountId",
                table: "OrderHeaders");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
