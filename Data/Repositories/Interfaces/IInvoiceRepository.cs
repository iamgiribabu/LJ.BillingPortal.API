using LJ.BillingPortal.API.Models;

namespace LJ.BillingPortal.API.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for invoice-related data operations
/// </summary>
public interface IInvoiceRepository
{
    // ClientDetails operations
    Task<ClientDetails?> GetClientByIdAsync(int clientId);
    Task<List<ClientDetails>> GetAllClientsAsync();
    Task<ClientDetails> AddClientAsync(ClientDetails client);
    Task UpdateClientAsync(ClientDetails client);
    Task DeleteClientAsync(int clientId);

    // InvoiceDetails operations
    Task<InvoiceDetails?> GetInvoiceByIdAsync(int invoiceId);
    Task<InvoiceDetails?> GetInvoiceByNumberAsync(string invoiceNumber);
    Task<List<InvoiceDetails>> GetAllInvoicesAsync();
    Task<InvoiceDetails> AddInvoiceAsync(InvoiceDetails invoice);
    Task UpdateInvoiceAsync(InvoiceDetails invoice);
    Task DeleteInvoiceAsync(int invoiceId);
    Task<string> GetNextInvoiceNumberAsync();

    // InvoiceParticular operations
    Task<InvoiceParticular?> GetParticularByIdAsync(int serviceId);
    Task<List<InvoiceParticular>> GetParticularsForInvoiceAsync(int invoiceId);
    Task<InvoiceParticular> AddParticularAsync(InvoiceParticular particular);
    Task AddParticularsAsync(IEnumerable<InvoiceParticular> particulars);
    Task UpdateParticularAsync(InvoiceParticular particular);
    Task DeleteParticularAsync(int serviceId);

    // Transaction operations
    Task<IDisposable> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
