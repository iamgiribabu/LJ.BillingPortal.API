using LJ.BillingPortal.API.DTOs;

namespace LJ.BillingPortal.API.Services.Interfaces;

/// <summary>
/// Service interface for invoice operations
/// </summary>
public interface IInvoiceService
{
    /// <summary>
    /// Get all invoices with their details
    /// </summary>
    Task<List<InvoiceResponseDto>> GetAllInvoicesAsync();

    /// <summary>
    /// Get next invoice number from sequence
    /// </summary>
    Task<string> GetNextInvoiceNumberAsync();

    /// <summary>
    /// Create a complete invoice with client details and particulars
    /// </summary>
    Task<InvoiceResponseDto> CreateInvoiceAsync(CreateCompleteInvoiceDto request);

    /// <summary>
    /// Get all client addresses
    /// </summary>
    Task<List<ClientDetailsDto>> GetAllClientAddressesAsync();

    /// <summary>
    /// Update client address details
    /// </summary>
    Task<bool> UpdateClientAddressAsync(UpdateClientDetailsDto request);

    /// <summary>
    /// Update invoice details
    /// </summary>
    Task<bool> UpdateInvoiceDetailsAsync(UpdateInvoiceDetailsDto request);

    /// <summary>
    /// Update invoice particular
    /// </summary>
    Task<bool> UpdateInvoiceParticularAsync(UpdateInvoiceParticularDto request);
}
