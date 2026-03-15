using LJ.BillingPortal.API.Data.Repositories.Interfaces;
using LJ.BillingPortal.API.DTOs;
using LJ.BillingPortal.API.Exceptions;
using LJ.BillingPortal.API.Models;
using LJ.BillingPortal.API.Services.Interfaces;
using LJ.BillingPortal.API.Services.Models;

namespace LJ.BillingPortal.API.Services;

/// <summary>
/// Service implementation for invoice operations with business logic
/// </summary>
public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repository;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(
        IInvoiceRepository repository,
        ILogger<InvoiceService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<InvoiceResponseDto>> GetAllInvoicesAsync()
    {
        _logger.LogInformation("Fetching all invoices");

        var invoices = await _repository.GetAllInvoicesAsync();
        return invoices.Select(MapToInvoiceResponseDto).ToList();
    }

    public async Task<string> GetNextInvoiceNumberAsync()
    {
        _logger.LogInformation("Fetching next invoice number");
        return await _repository.GetNextInvoiceNumberAsync();
    }

    public async Task<InvoiceResponseDto> CreateInvoiceAsync(CreateCompleteInvoiceDto request)
    {
        _logger.LogInformation("Creating new invoice");

        var transaction = await _repository.BeginTransactionAsync();

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

            // Add client and save (sequential, single-threaded)
            try
            {
                clientDetails = await _repository.AddClientAsync(clientDetails);
                await _repository.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client");
                await _repository.RollbackTransactionAsync();
                throw new InvoiceCreationException("Failed to create client", ex);
            }

            // Get next invoice number
            string nextInvoiceNumber;
            try
            {
                nextInvoiceNumber = await _repository.GetNextInvoiceNumberAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice number");
                await _repository.RollbackTransactionAsync();
                throw new InvoiceCreationException("Failed to generate invoice number", ex);
            }

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

            // Add invoice and save (sequential, single-threaded)
            try
            {
                invoice = await _repository.AddInvoiceAsync(invoice);
                await _repository.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice");
                await _repository.RollbackTransactionAsync();
                throw new InvoiceCreationException("Failed to create invoice", ex);
            }

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

            // Add particulars and save (sequential, single-threaded)
            try
            {
                await AddParticularsSequentiallyAsync(particulars);
                await _repository.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding invoice particulars");
                await _repository.RollbackTransactionAsync();
                throw new InvoiceCreationException("Failed to add invoice particulars", ex);
            }

            await _repository.CommitTransactionAsync();

            _logger.LogInformation($"Invoice {nextInvoiceNumber} created successfully for client {clientDetails.ClientId}");

            // Construct response
            invoice.Particulars = particulars;
            return MapToInvoiceResponseDto(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice - all operations rolled back");
            throw;
        }
        finally
        {
            transaction.Dispose();
        }
    }

    /// <summary>
    /// Adds invoice particulars sequentially (single-threaded)
    /// All Add() calls happen before SaveAsync() to ensure thread safety
    /// </summary>
    private async Task AddParticularsSequentiallyAsync(List<InvoiceParticular> particulars)
    {
        if (!particulars.Any())
        {
            return;
        }

        _logger.LogInformation($"Adding {particulars.Count} invoice particulars sequentially");

        try
        {
            foreach (var particular in particulars)
            {
                try
                {
                    await _repository.AddParticularAsync(particular);
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Failed to track particular for Invoice ID {particular.InvoiceId}: {ex.Message}";
                    _logger.LogError(ex, errorMessage);
                    throw new InvoiceCreationException(errorMessage, ex);
                }
            }

            _logger.LogInformation($"All {particulars.Count} invoice particulars tracked successfully");
        }
        catch (InvoiceCreationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sequential tracking of particulars");
            throw new InvoiceCreationException("Unexpected error during sequential tracking of particulars", ex);
        }
    }

    public async Task<List<ClientDetailsDto>> GetAllClientAddressesAsync()
    {
        _logger.LogInformation("Fetching all client addresses");

        var clients = await _repository.GetAllClientsAsync();
        return clients.Select(MapToClientDetailsDto).ToList();
    }

    public async Task<bool> UpdateClientAddressAsync(UpdateClientDetailsDto request)
    {
        _logger.LogInformation($"Updating client address for client {request.ClientId}");

        var client = await _repository.GetClientByIdAsync(request.ClientId);
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

            await _repository.UpdateClientAsync(client);

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

        var invoice = await _repository.GetInvoiceByNumberAsync(request.InvoiceNumber);

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

            await _repository.UpdateInvoiceAsync(invoice);

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

        var particular = await _repository.GetParticularByIdAsync(request.ServiceId);

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

            await _repository.UpdateParticularAsync(particular);

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
