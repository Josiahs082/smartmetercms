﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smartmetercms.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserMeterID",
                table: "EnergyUsage",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserMeterID",
                table: "Bill",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnergyUsage_UserMeterID",
                table: "EnergyUsage",
                column: "UserMeterID");

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

            migrationBuilder.AddForeignKey(
                name: "FK_EnergyUsage_User_UserMeterID",
                table: "EnergyUsage",
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

            migrationBuilder.DropForeignKey(
                name: "FK_EnergyUsage_User_UserMeterID",
                table: "EnergyUsage");

            migrationBuilder.DropIndex(
                name: "IX_EnergyUsage_UserMeterID",
                table: "EnergyUsage");

            migrationBuilder.DropIndex(
                name: "IX_Bill_UserMeterID",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "UserMeterID",
                table: "EnergyUsage");

            migrationBuilder.DropColumn(
                name: "UserMeterID",
                table: "Bill");
        }
    }
}
