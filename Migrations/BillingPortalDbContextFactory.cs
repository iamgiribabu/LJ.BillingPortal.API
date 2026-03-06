using LJ.BillingPortal.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LJ.BillingPortal.API.Migrations;

public class BillingPortalDbContextFactory : IDesignTimeDbContextFactory<BillingPortalDbContext>
{
    public BillingPortalDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BillingPortalDbContext>();

        var connectionString = "Server=localhost;Database=InvoiceDB;User Id=Admin_Giribabu;Password=$LJLpass01;Encrypt=False;TrustServerCertificate=True;";

        optionsBuilder.UseSqlServer(connectionString, x => x.CommandTimeout(300));

        return new BillingPortalDbContext(optionsBuilder.Options);
    }
}
