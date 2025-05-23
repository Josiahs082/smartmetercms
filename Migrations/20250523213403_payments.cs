using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smartmetercms.Migrations
{
    /// <inheritdoc />
    public partial class payments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_User_MeterID",
                table: "Bill");

            migrationBuilder.DropIndex(
                name: "IX_Bill_MeterID",
                table: "Bill");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountDue",
                table: "Bill",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "UserMeterID",
                table: "Bill",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bill_UserMeterID",
                table: "Bill",
                column: "UserMeterID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_User_UserMeterID",
                table: "Bill",
                column: "UserMeterID",
                principalTable: "User",
                principalColumn: "MeterID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_User_UserMeterID",
                table: "Bill");

            migrationBuilder.DropIndex(
                name: "IX_Bill_UserMeterID",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "UserMeterID",
                table: "Bill");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountDue",
                table: "Bill",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_MeterID",
                table: "Bill",
                column: "MeterID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_User_MeterID",
                table: "Bill",
                column: "MeterID",
                principalTable: "User",
                principalColumn: "MeterID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
