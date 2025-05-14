using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smartmetercms.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKeyUpdate4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntervalEnergyUsage_User_UserMeterID",
                table: "IntervalEnergyUsage");

            migrationBuilder.DropIndex(
                name: "IX_IntervalEnergyUsage_UserMeterID",
                table: "IntervalEnergyUsage");

            migrationBuilder.DropColumn(
                name: "UserMeterID",
                table: "IntervalEnergyUsage");

            migrationBuilder.CreateIndex(
                name: "IX_IntervalEnergyUsage_MeterID",
                table: "IntervalEnergyUsage",
                column: "MeterID");

            migrationBuilder.AddForeignKey(
                name: "FK_IntervalEnergyUsage_User_MeterID",
                table: "IntervalEnergyUsage",
                column: "MeterID",
                principalTable: "User",
                principalColumn: "MeterID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntervalEnergyUsage_User_MeterID",
                table: "IntervalEnergyUsage");

            migrationBuilder.DropIndex(
                name: "IX_IntervalEnergyUsage_MeterID",
                table: "IntervalEnergyUsage");

            migrationBuilder.AddColumn<string>(
                name: "UserMeterID",
                table: "IntervalEnergyUsage",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntervalEnergyUsage_UserMeterID",
                table: "IntervalEnergyUsage",
                column: "UserMeterID");

            migrationBuilder.AddForeignKey(
                name: "FK_IntervalEnergyUsage_User_UserMeterID",
                table: "IntervalEnergyUsage",
                column: "UserMeterID",
                principalTable: "User",
                principalColumn: "MeterID");
        }
    }
}
