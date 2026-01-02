using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTransportTableToLowercase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Transport",
                newName: "transport");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "transport",
                newName: "Transport");
        }
    }
}
