using LJ.BillingPortal.API.Data.Repositories.Interfaces;
using LJ.BillingPortal.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LJ.BillingPortal.API.Data.Repositories;

/// <summary>
/// Repository implementation for invoice-related data operations
/// </summary>
public class InvoiceRepository : IInvoiceRepository
{
    private readonly BillingPortalDbContext _context;
    private readonly ILogger<InvoiceRepository> _logger;

    public InvoiceRepository(
        BillingPortalDbContext context,
        ILogger<InvoiceRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region ClientDetails Operations

    public async Task<ClientDetails?> GetClientByIdAsync(int clientId)
    {
        try
        {
            _logger.LogInformation("Retrieving client {ClientId}", clientId);
            return await _context.ClientDetails.FindAsync(clientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving client {ClientId}", clientId);
            throw;
        }
    }

    public async Task<List<ClientDetails>> GetAllClientsAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all clients");
            return await _context.ClientDetails
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all clients");
            throw;
        }
    }

    public async Task<ClientDetails> AddClientAsync(ClientDetails client)
    {
        try
        {
            _logger.LogInformation("Adding new client {ClientName}", client.BilledToName);
            _context.ClientDetails.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding client");
            throw;
        }
    }

    public async Task UpdateClientAsync(ClientDetails client)
    {
        try
        {
            _logger.LogInformation("Updating client {ClientId}", client.ClientId);
            _context.ClientDetails.Update(client);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating client {ClientId}", client.ClientId);
            throw;
        }
    }

    public async Task DeleteClientAsync(int clientId)
    {
        try
        {
            _logger.LogInformation("Deleting client {ClientId}", clientId);
            var client = await GetClientByIdAsync(clientId);
            if (client != null)
            {
                _context.ClientDetails.Remove(client);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting client {ClientId}", clientId);
            throw;
        }
    }

    #endregion

    #region InvoiceDetails Operations

    public async Task<InvoiceDetails?> GetInvoiceByIdAsync(int invoiceId)
    {
        try
        {
            _logger.LogInformation("Retrieving invoice {InvoiceId}", invoiceId);
            return await _context.InvoiceDetails
                .Include(i => i.Client)
                .Include(i => i.Particulars)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoice {InvoiceId}", invoiceId);
            throw;
        }
    }

    public async Task<InvoiceDetails?> GetInvoiceByNumberAsync(string invoiceNumber)
    {
        try
        {
            _logger.LogInformation("Retrieving invoice {InvoiceNumber}", invoiceNumber);
            return await _context.InvoiceDetails
                .Include(i => i.Client)
                .Include(i => i.Particulars)
                .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoice {InvoiceNumber}", invoiceNumber);
            throw;
        }
    }

    public async Task<List<InvoiceDetails>> GetAllInvoicesAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all invoices");
            return await _context.InvoiceDetails
                .Include(i => i.Client)
                .Include(i => i.Particulars)
                .OrderByDescending(i => i.CreatedDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all invoices");
            throw;
        }
    }

    public async Task<InvoiceDetails> AddInvoiceAsync(InvoiceDetails invoice)
    {
        try
        {
            _logger.LogInformation("Adding new invoice {InvoiceNumber}", invoice.InvoiceNumber);
            _context.InvoiceDetails.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding invoice");
            throw;
        }
    }

    public async Task UpdateInvoiceAsync(InvoiceDetails invoice)
    {
        try
        {
            _logger.LogInformation("Updating invoice {InvoiceId}", invoice.InvoiceId);
            _context.InvoiceDetails.Update(invoice);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating invoice {InvoiceId}", invoice.InvoiceId);
            throw;
        }
    }

    public async Task DeleteInvoiceAsync(int invoiceId)
    {
        try
        {
            _logger.LogInformation("Deleting invoice {InvoiceId}", invoiceId);
            var invoice = await GetInvoiceByIdAsync(invoiceId);
            if (invoice != null)
            {
                _context.InvoiceDetails.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting invoice {InvoiceId}", invoiceId);
            throw;
        }
    }

    public async Task<string> GetNextInvoiceNumberAsync()
    {
        try
        {
            _logger.LogInformation("Fetching next invoice number");
            var lastInvoice = await _context.InvoiceDetails
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1001;
            if (lastInvoice != null && int.TryParse(lastInvoice.InvoiceNumber, out int lastNum))
            {
                nextNumber = lastNum + 1;
            }

            return nextNumber.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching next invoice number");
            throw;
        }
    }

    #endregion

    #region InvoiceParticular Operations

    public async Task<InvoiceParticular?> GetParticularByIdAsync(int serviceId)
    {
        try
        {
            _logger.LogInformation("Retrieving invoice particular {ServiceId}", serviceId);
            return await _context.InvoiceParticulars.FindAsync(serviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoice particular {ServiceId}", serviceId);
            throw;
        }
    }

    public async Task<List<InvoiceParticular>> GetParticularsForInvoiceAsync(int invoiceId)
    {
        try
        {
            _logger.LogInformation("Retrieving particulars for invoice {InvoiceId}", invoiceId);
            return await _context.InvoiceParticulars
                .Where(p => p.InvoiceId == invoiceId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving particulars for invoice {InvoiceId}", invoiceId);
            throw;
        }
    }

    public async Task<InvoiceParticular> AddParticularAsync(InvoiceParticular particular)
    {
        try
        {
            _logger.LogInformation("Adding invoice particular for invoice {InvoiceId}", particular.InvoiceId);
            _context.InvoiceParticulars.Add(particular);
            await _context.SaveChangesAsync();
            return particular;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding invoice particular");
            throw;
        }
    }

    public async Task AddParticularsAsync(IEnumerable<InvoiceParticular> particulars)
    {
        try
        {
            _logger.LogInformation("Adding multiple invoice particulars");
            _context.InvoiceParticulars.AddRange(particulars);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding invoice particulars");
            throw;
        }
    }

    public async Task UpdateParticularAsync(InvoiceParticular particular)
    {
        try
        {
            _logger.LogInformation("Updating invoice particular {ServiceId}", particular.ServiceId);
            _context.InvoiceParticulars.Update(particular);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating invoice particular {ServiceId}", particular.ServiceId);
            throw;
        }
    }

    public async Task DeleteParticularAsync(int serviceId)
    {
        try
        {
            _logger.LogInformation("Deleting invoice particular {ServiceId}", serviceId);
            var particular = await GetParticularByIdAsync(serviceId);
            if (particular != null)
            {
                _context.InvoiceParticulars.Remove(particular);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting invoice particular {ServiceId}", serviceId);
            throw;
        }
    }

    #endregion

    #region Transaction Operations

    public async Task<IDisposable> BeginTransactionAsync()
    {
        try
        {
            _logger.LogInformation("Beginning database transaction");
            return await _context.Database.BeginTransactionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error beginning transaction");
            throw;
        }
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            _logger.LogInformation("Committing database transaction");
            await _context.Database.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing transaction");
            throw;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            _logger.LogInformation("Rolling back database transaction");
            await _context.Database.RollbackTransactionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back transaction");
            throw;
        }
    }

    #endregion
}
