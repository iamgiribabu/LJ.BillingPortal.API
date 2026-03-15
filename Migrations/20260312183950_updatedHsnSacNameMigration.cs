using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LJ.BillingPortal.API.Migrations
{
    /// <inheritdoc />
    public partial class updatedHsnSacNameMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HsnSac",
                table: "InvoiceParticulars",
                newName: "HSN_SAC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HSN_SAC",
                table: "InvoiceParticulars",
                newName: "HsnSac");
        }
    }
}
