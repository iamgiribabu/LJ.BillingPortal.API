using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LJ.BillingPortal.API.Migrations
{
    /// <inheritdoc />
    public partial class MakeInvoiceNumberUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_InvoiceNumber",
                table: "InvoiceDetails");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoiceNumber",
                table: "InvoiceDetails",
                column: "InvoiceNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_InvoiceNumber",
                table: "InvoiceDetails");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoiceNumber",
                table: "InvoiceDetails",
                column: "InvoiceNumber");
        }
    }
}
