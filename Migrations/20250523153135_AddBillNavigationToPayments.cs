using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smartmetercms.Migrations
{
    /// <inheritdoc />
    public partial class AddBillNavigationToPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Payments_BillID",
                table: "Payments",
                column: "BillID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Bill_BillID",
                table: "Payments",
                column: "BillID",
                principalTable: "Bill",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Bill_BillID",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_BillID",
                table: "Payments");
        }
    }
}
