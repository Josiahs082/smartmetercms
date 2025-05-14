using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smartmetercms.Migrations
{
    /// <inheritdoc />
    public partial class AddNewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bill",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MeterID = table.Column<string>(type: "TEXT", nullable: false),
                    BillingPeriodStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BillingPeriodEnd = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalEnergyUsed = table.Column<double>(type: "REAL", nullable: false),
                    AmountDue = table.Column<decimal>(type: "TEXT", nullable: false),
                    PaidStatus = table.Column<bool>(type: "INTEGER", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bill", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EnergyUsage",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MeterID = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EnergyUsed = table.Column<double>(type: "REAL", nullable: false),
                    Voltage = table.Column<double>(type: "REAL", nullable: false),
                    Current = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnergyUsage", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BillID = table.Column<int>(type: "INTEGER", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "TEXT", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bill");

            migrationBuilder.DropTable(
                name: "EnergyUsage");

            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
