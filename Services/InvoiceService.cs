using LJ.BillingPortal.API.Data;
using LJ.BillingPortal.API.DTOs;
using LJ.BillingPortal.API.Models;
using LJ.BillingPortal.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LJ.BillingPortal.API.Services;

/// <summary>
/// Service implementation for invoice operations with business logic
/// </summary>
public class InvoiceService : IInvoiceService
{
    private readonly BillingPortalDbContext _dbContext;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(
        BillingPortalDbContext dbContext,
        ILogger<InvoiceService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<InvoiceResponseDto>> GetAllInvoicesAsync()
    {
        _logger.LogInformation("Fetching all invoices");

        var invoices = await _dbContext.InvoiceDetails
            .Include(i => i.Client)
            .Include(i => i.Particulars)
            .OrderByDescending(i => i.CreatedDate)
            .ToListAsync();

        return invoices.Select(MapToInvoiceResponseDto).ToList();
    }

    public async Task<string> GetNextInvoiceNumberAsync()
    {
        _logger.LogInformation("Fetching next invoice number");

        var lastInvoice = await _dbContext.InvoiceDetails
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync();

        int nextNumber = 1001;
        if (lastInvoice != null && int.TryParse(lastInvoice.InvoiceNumber, out int lastNum))
        {
            nextNumber = lastNum + 1;
        }

        return nextNumber.ToString();
    }

    public async Task<InvoiceResponseDto> CreateInvoiceAsync(CreateCompleteInvoiceDto request)
    {
        _logger.LogInformation("Creating new invoice");

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            // Create client
            var clientDetails = new ClientDetails
            {
                BilledToName = request.ClientDetails.BilledToName,
                AddressLine1 = request.ClientDetails.AddressLine1,
                AddressLine2 = request.ClientDetails.AddressLine2,
                AddressLine3 = request.ClientDetails.AddressLine3,
                Gstin = request.ClientDetails.Gstin,
                State = request.ClientDetails.State,
                StateCode = request.ClientDetails.StateCode,
                CreatedDate = DateTime.UtcNow
            };

            _dbContext.ClientDetails.Add(clientDetails);
            await _dbContext.SaveChangesAsync();

            // Get next invoice number
            var nextInvoiceNumber = await GetNextInvoiceNumberAsync();

            // Create invoice
            var invoice = new InvoiceDetails
            {
                ClientId = clientDetails.ClientId,
                InvoiceNumber = nextInvoiceNumber,
                InvoiceDate = request.InvoiceDetails.InvoiceDate,
                PlaceOfSupply = request.InvoiceDetails.PlaceOfSupply,
                PoNumber = request.InvoiceDetails.PoNumber,
                CraneReg = request.InvoiceDetails.CraneReg,
                TotalAmountBeforeTax = request.InvoiceDetails.TotalAmountBeforeTax,
                Cgst = request.InvoiceDetails.Cgst,
                Sgst = request.InvoiceDetails.Sgst,
                Igst = request.InvoiceDetails.Igst,
                NetAmountAfterTax = request.InvoiceDetails.NetAmountAfterTax,
                CreatedDate = DateTime.UtcNow,
                Client = clientDetails
            };

            _dbContext.InvoiceDetails.Add(invoice);
            await _dbContext.SaveChangesAsync();

            // Create particulars
            var particulars = request.InvoiceParticulars
                .Select(p => new InvoiceParticular
                {
                    InvoiceId = invoice.InvoiceId,
                    Description = p.Description,
                    HsnSac = p.HsnSac,
                    Quantity = p.Quantity,
                    Rate = p.Rate,
                    TaxableValue = p.TaxableValue,
                    CreatedDate = DateTime.UtcNow
                })
                .ToList();

            _dbContext.InvoiceParticulars.AddRange(particulars);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            _logger.LogInformation($"Invoice {nextInvoiceNumber} created successfully for client {clientDetails.ClientId}");

            // Construct response
            invoice.Particulars = particulars;
            return MapToInvoiceResponseDto(invoice);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating invoice");
            throw;
        }
    }

    public async Task<List<ClientDetailsDto>> GetAllClientAddressesAsync()
    {
        _logger.LogInformation("Fetching all client addresses");

        var clients = await _dbContext.ClientDetails
            .OrderByDescending(c => c.CreatedDate)
            .ToListAsync();

        return clients.Select(MapToClientDetailsDto).ToList();
    }

    public async Task<bool> UpdateClientAddressAsync(UpdateClientDetailsDto request)
    {
        _logger.LogInformation($"Updating client address for client {request.ClientId}");

        var client = await _dbContext.ClientDetails.FindAsync(request.ClientId);
        if (client == null)
        {
            _logger.LogWarning($"Client {request.ClientId} not found");
            return false;
        }

        try
        {
            client.BilledToName = request.BilledToName;
            client.AddressLine1 = request.AddressLine1;
            client.AddressLine2 = request.AddressLine2;
            client.AddressLine3 = request.AddressLine3;
            client.Gstin = request.Gstin;
            client.State = request.State;
            client.StateCode = request.StateCode;
            client.ModifiedDate = DateTime.UtcNow;

            _dbContext.ClientDetails.Update(client);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Client {request.ClientId} updated successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating client {request.ClientId}");
            throw;
        }
    }

    public async Task<bool> UpdateInvoiceDetailsAsync(UpdateInvoiceDetailsDto request)
    {
        _logger.LogInformation($"Updating invoice {request.InvoiceNumber}");

        var invoice = await _dbContext.InvoiceDetails
            .FirstOrDefaultAsync(i => i.InvoiceNumber == request.InvoiceNumber);

        if (invoice == null)
        {
            _logger.LogWarning($"Invoice {request.InvoiceNumber} not found");
            return false;
        }

        try
        {
            invoice.InvoiceDate = request.InvoiceDate;
            invoice.PlaceOfSupply = request.PlaceOfSupply;
            invoice.PoNumber = request.PoNumber;
            invoice.CraneReg = request.CraneReg;
            invoice.TotalAmountBeforeTax = request.TotalAmountBeforeTax;
            invoice.Cgst = request.Cgst;
            invoice.Sgst = request.Sgst;
            invoice.Igst = request.Igst;
            invoice.NetAmountAfterTax = request.NetAmountAfterTax;
            invoice.ModifiedDate = DateTime.UtcNow;

            _dbContext.InvoiceDetails.Update(invoice);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Invoice {request.InvoiceNumber} updated successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating invoice {request.InvoiceNumber}");
            throw;
        }
    }

    public async Task<bool> UpdateInvoiceParticularAsync(UpdateInvoiceParticularDto request)
    {
        _logger.LogInformation($"Updating invoice particular {request.ServiceId}");

        var particular = await _dbContext.InvoiceParticulars.FindAsync(request.ServiceId);

        if (particular == null)
        {
            _logger.LogWarning($"Invoice particular {request.ServiceId} not found");
            return false;
        }

        try
        {
            particular.Description = request.Description;
            particular.HsnSac = request.HsnSac;
            particular.Quantity = request.Quantity;
            particular.Rate = request.Rate;
            particular.TaxableValue = request.TaxableValue;

            _dbContext.InvoiceParticulars.Update(particular);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Invoice particular {request.ServiceId} updated successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating invoice particular {request.ServiceId}");
            throw;
        }
    }

    // Private helper methods
    private InvoiceResponseDto MapToInvoiceResponseDto(InvoiceDetails invoice)
    {
        return new InvoiceResponseDto
        {
            InvoiceId = invoice.InvoiceId,
            ClientDetails = MapToClientDetailsDto(invoice.Client),
            InvoiceDetails = new InvoiceDetailsDto
            {
                InvoiceId = invoice.InvoiceId,
                ClientId = invoice.ClientId,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                PlaceOfSupply = invoice.PlaceOfSupply,
                PoNumber = invoice.PoNumber,
                CraneReg = invoice.CraneReg,
                TotalAmountBeforeTax = invoice.TotalAmountBeforeTax,
                Cgst = invoice.Cgst,
                Sgst = invoice.Sgst,
                Igst = invoice.Igst,
                NetAmountAfterTax = invoice.NetAmountAfterTax
            },
            InvoiceParticulars = invoice.Particulars
                .Select(p => new InvoiceParticularDto
                {
                    ServiceId = p.ServiceId,
                    InvoiceId = p.InvoiceId,
                    Description = p.Description,
                    HsnSac = p.HsnSac,
                    Quantity = p.Quantity,
                    Rate = p.Rate,
                    TaxableValue = p.TaxableValue
                })
                .ToList()
        };
    }

    private ClientDetailsDto MapToClientDetailsDto(ClientDetails client)
    {
        return new ClientDetailsDto
        {
            ClientId = client.ClientId,
            BilledToName = client.BilledToName,
            AddressLine1 = client.AddressLine1,
            AddressLine2 = client.AddressLine2,
            AddressLine3 = client.AddressLine3,
            Gstin = client.Gstin,
            State = client.State,
            StateCode = client.StateCode
        };
    }
}
