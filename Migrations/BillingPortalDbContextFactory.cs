using LJ.BillingPortal.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LJ.BillingPortal.API.Migrations;

public class BillingPortalDbContextFactory : IDesignTimeDbContextFactory<BillingPortalDbContext>
{
    public BillingPortalDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BillingPortalDbContext>();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("BillingPortalDBConnection")
            ?? throw new InvalidOperationException("Connection string 'BillingPortalDBConnection' not found in appsettings.json");

        optionsBuilder.UseSqlServer(connectionString, x => x.CommandTimeout(300));

        return new BillingPortalDbContext(optionsBuilder.Options);
    }
}
