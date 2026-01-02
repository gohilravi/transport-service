using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TransportService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CarrierId = table.Column<int>(type: "integer", nullable: false),
                    PurchaseId = table.Column<int>(type: "integer", nullable: false),
                    PickupLocation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DeliveryLocation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VehicleDetails = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Scheduled"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "(NOW() AT TIME ZONE 'UTC')"),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "(NOW() AT TIME ZONE 'UTC')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transport", x => x.Id);
                    table.CheckConstraint("CK_Transports_Status", "\"Status\" IN ('Assigned', 'InTransit', 'Completed', 'Canceled', 'Scheduled')");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transport_CarrierId",
                table: "Transport",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Transport_PurchaseId",
                table: "Transport",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Transport_ScheduleDate",
                table: "Transport",
                column: "ScheduleDate");

            migrationBuilder.CreateIndex(
                name: "IX_Transport_Status",
                table: "Transport",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transport");
        }
    }
}
