using LJ.BillingPortal.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LJ.BillingPortal.API.Data;

public class BillingPortalDbContext : DbContext
{
    public BillingPortalDbContext(DbContextOptions<BillingPortalDbContext> options)
        : base(options)
    {
    }

    public DbSet<ClientDetails> ClientDetails { get; set; } = null!;
    public DbSet<InvoiceDetails> InvoiceDetails { get; set; } = null!;
    public DbSet<InvoiceParticular> InvoiceParticulars { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ClientDetails>()
            .HasKey(c => c.ClientId);

        ConfigureClientDetails(modelBuilder);
        ConfigureInvoiceDetails(modelBuilder);
        ConfigureInvoiceParticular(modelBuilder);

        // seed data



    }

    private static void ConfigureClientDetails(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<ClientDetails>();

        builder.ToTable("ClientDetails");
        builder.HasKey(c => c.ClientId);

        builder.Property(c => c.ClientId)
            .HasColumnName("ClientID")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.BilledToName)
            .HasColumnName("CompanyName")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.AddressLine1)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.AddressLine2)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.AddressLine3)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.Gstin)
            .HasColumnName("GSTIN")
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(c => c.State)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.StateCode)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(c => c.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.ModifiedDate);

        builder.HasIndex(c => c.Gstin)
            .HasDatabaseName("IX_ClientDetails_GSTIN");

        builder.HasMany(c => c.Invoices)
            .WithOne(i => i.Client)
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureInvoiceDetails(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<InvoiceDetails>();

        builder.ToTable("InvoiceDetails");
        builder.HasKey(i => i.InvoiceId);

        builder.Property(i => i.InvoiceId)
            .HasColumnName("InvoiceID")
            .ValueGeneratedOnAdd();

        builder.Property(i => i.ClientId)
            .HasColumnName("ClientID");

        builder.Property(i => i.InvoiceNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.PlaceOfSupply)
            .HasMaxLength(100);

        builder.Property(i => i.PoNumber)
            .HasColumnName("PONumber")
            .HasMaxLength(50);

        builder.Property(i => i.CraneReg)
            .HasMaxLength(50);

        builder.Property(i => i.TotalAmountBeforeTax)
            .HasPrecision(18, 2);

        builder.Property(i => i.Cgst)
            .HasColumnName("CGST")
            .HasPrecision(18, 2);

        builder.Property(i => i.Sgst)
            .HasColumnName("SGST")
            .HasPrecision(18, 2);

        builder.Property(i => i.Igst)
            .HasColumnName("IGST")
            .HasPrecision(18, 2);

        builder.Property(i => i.NetAmountAfterTax)
            .HasPrecision(18, 2);

        builder.Property(i => i.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();

        builder.Property(i => i.ModifiedDate);

        builder.HasIndex(i => i.InvoiceNumber)
            .IsUnique()
            .HasDatabaseName("IX_InvoiceDetails_InvoiceNumber");

        builder.HasIndex(i => i.ClientId)
            .HasDatabaseName("IX_InvoiceDetails_ClientID");

        builder.HasMany(i => i.Particulars)
            .WithOne(p => p.Invoice)
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureInvoiceParticular(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<InvoiceParticular>();

        builder.ToTable("InvoiceParticulars");
        builder.HasKey(p => p.ServiceId);

        builder.Property(p => p.ServiceId)
            .HasColumnName("ServiceID")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.InvoiceId)
            .HasColumnName("InvoiceID");

        builder.Property(p => p.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.HsnSac)
            .HasColumnName("HsnSac")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Quantity)
            .HasPrecision(10, 2);

        builder.Property(p => p.Rate)
            .HasPrecision(18, 2);

        builder.Property(p => p.TaxableValue)
            .HasPrecision(18, 2);

        builder.Property(p => p.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();

        builder.HasIndex(p => p.InvoiceId)
            .HasDatabaseName("IX_InvoiceParticulars_InvoiceID");
    }
}
