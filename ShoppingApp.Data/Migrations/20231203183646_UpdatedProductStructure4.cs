using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedProductStructure4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_OrderHeaders_Uid",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Products_Uid",
                table: "OrderLines");

            migrationBuilder.AlterColumn<string>(
                name: "ProductUid",
                table: "OrderLines",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OrderHeaderUid",
                table: "OrderLines",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_OrderHeaderUid",
                table: "OrderLines",
                column: "OrderHeaderUid");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_ProductUid",
                table: "OrderLines",
                column: "ProductUid");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_OrderHeaders_OrderHeaderUid",
                table: "OrderLines",
                column: "OrderHeaderUid",
                principalTable: "OrderHeaders",
                principalColumn: "Uid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Products_ProductUid",
                table: "OrderLines",
                column: "ProductUid",
                principalTable: "Products",
                principalColumn: "Uid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_OrderHeaders_OrderHeaderUid",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Products_ProductUid",
                table: "OrderLines");

            migrationBuilder.DropIndex(
                name: "IX_OrderLines_OrderHeaderUid",
                table: "OrderLines");

            migrationBuilder.DropIndex(
                name: "IX_OrderLines_ProductUid",
                table: "OrderLines");

            migrationBuilder.AlterColumn<string>(
                name: "ProductUid",
                table: "OrderLines",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "OrderHeaderUid",
                table: "OrderLines",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_OrderHeaders_Uid",
                table: "OrderLines",
                column: "Uid",
                principalTable: "OrderHeaders",
                principalColumn: "Uid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Products_Uid",
                table: "OrderLines",
                column: "Uid",
                principalTable: "Products",
                principalColumn: "Uid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
