using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LJ.BillingPortal.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModifiedDateConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "InvoiceDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "ClientDetails",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "ClientDetails");
        }
    }
}
