using FluentValidation;
using LJ.BillingPortal.API.DTOs;
using LJ.BillingPortal.API.Exceptions;
using LJ.BillingPortal.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LJ.BillingPortal.API.Controllers;

/// <summary>
/// Invoice API Controller - Handles all invoice-related operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly IPdfGenerationService _pdfService;
    private readonly IValidator<CreateCompleteInvoiceDto> _createInvoiceValidator;
    private readonly IValidator<UpdateClientDetailsDto> _updateClientValidator;
    private readonly IValidator<UpdateInvoiceDetailsDto> _updateInvoiceValidator;
    private readonly IValidator<UpdateInvoiceParticularDto> _updateParticularValidator;
    private readonly IValidator<GenerateInvoiceRequest> _generateInvoiceValidator;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(
        IInvoiceService invoiceService,
        IPdfGenerationService pdfService,
        IValidator<CreateCompleteInvoiceDto> createInvoiceValidator,
        IValidator<UpdateClientDetailsDto> updateClientValidator,
        IValidator<UpdateInvoiceDetailsDto> updateInvoiceValidator,
        IValidator<UpdateInvoiceParticularDto> updateParticularValidator,
        IValidator<GenerateInvoiceRequest> generateInvoiceValidator,
        ILogger<InvoicesController> logger)
    {
        _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
        _pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
        _createInvoiceValidator = createInvoiceValidator ?? throw new ArgumentNullException(nameof(createInvoiceValidator));
        _updateClientValidator = updateClientValidator ?? throw new ArgumentNullException(nameof(updateClientValidator));
        _updateInvoiceValidator = updateInvoiceValidator ?? throw new ArgumentNullException(nameof(updateInvoiceValidator));
        _updateParticularValidator = updateParticularValidator ?? throw new ArgumentNullException(nameof(updateParticularValidator));
        _generateInvoiceValidator = generateInvoiceValidator ?? throw new ArgumentNullException(nameof(generateInvoiceValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all invoices with their details and line items
    /// </summary>
    /// <returns>List of all invoices</returns>
    [HttpGet]
    [Route("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<InvoiceResponseDto>>> GetAllInvoices()
    {
        _logger.LogInformation("Get all invoices request received");
        var result = await _invoiceService.GetAllInvoicesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get the next invoice number from the sequence
    /// </summary>
    /// <returns>Next invoice number</returns>
    [HttpGet]
    [Route("next-number")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> GetNextInvoiceNumber()
    {
        _logger.LogInformation("Get next invoice number request received");
        var nextNumber = await _invoiceService.GetNextInvoiceNumberAsync();
        return Ok(new { nextInvoiceNumber = nextNumber });
    }

    /// <summary>
    /// Create a new complete invoice with client details and line items
    /// </summary>
    /// <param name="request">Complete invoice creation request</param>
    /// <returns>Created invoice with all details</returns>
    [HttpPost]
    [Route("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InvoiceResponseDto>> CreateInvoice([FromBody] CreateCompleteInvoiceDto request)
    {
        _logger.LogInformation("Create invoice request received");

        // Validate request
        var validationResult = await _createInvoiceValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var failures = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            throw new Exceptions.ValidationException(failures);
        }

        var result = await _invoiceService.CreateInvoiceAsync(request);
        return CreatedAtAction(nameof(GetAllInvoices), new { id = result.InvoiceId }, result);
    }

    /// <summary>
    /// Generate PDF invoice
    /// </summary>
    /// <param name="request">Invoice details for PDF generation</param>
    /// <returns>URL to the generated PDF</returns>
    [HttpPost]
    [Route("generate-pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PdfGenerationResponse>> GenerateInvoicePdf([FromBody] GenerateInvoiceRequest request)
    {
        _logger.LogInformation($"Generate PDF request received for invoice {request.InvoiceDetails.InvoiceNumber}");

        // Validate request
        var validationResult = await _generateInvoiceValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var failures = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            throw new Exceptions.ValidationException(failures);
        }

        var htmlContent = _pdfService.GenerateInvoiceHtml(
            request.ClientDetails,
            request.InvoiceDetails,
            request.InvoiceParticulars);

        var fileName = $"invoice_{request.InvoiceDetails.InvoiceNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        var pdfUrl = await _pdfService.GeneratePdfAsync(htmlContent, fileName);

        return Ok(new PdfGenerationResponse
        {
            Message = "Invoice PDF generated successfully",
            Url = pdfUrl
        });
    }

    /// <summary>
    /// Get all client addresses
    /// </summary>
    /// <returns>List of all client addresses</returns>
    [HttpGet]
    [Route("clients")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ClientDetailsDto>>> GetAllClientAddresses()
    {
        _logger.LogInformation("Get all client addresses request received");
        var result = await _invoiceService.GetAllClientAddressesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Update client address details
    /// </summary>
    /// <param name="request">Updated client address information</param>
    /// <returns>Success confirmation</returns>
    [HttpPut]
    [Route("clients")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> UpdateClientAddress([FromBody] UpdateClientDetailsDto request)
    {
        _logger.LogInformation($"Update client address request received for client {request.ClientId}");

        // Validate request
        var validationResult = await _updateClientValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var failures = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            throw new Exceptions.ValidationException(failures);
        }

        var result = await _invoiceService.UpdateClientAddressAsync(request);

        if (!result)
        {
            throw new NotFoundException($"Client with ID {request.ClientId} not found");
        }

        return Ok(new { message = "Client address updated successfully" });
    }

    /// <summary>
    /// Update invoice details
    /// </summary>
    /// <param name="request">Updated invoice information</param>
    /// <returns>Success confirmation</returns>
    [HttpPut]
    [Route("details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> UpdateInvoiceDetails([FromBody] UpdateInvoiceDetailsDto request)
    {
        _logger.LogInformation($"Update invoice details request received for invoice {request.InvoiceNumber}");

        // Validate request
        var validationResult = await _updateInvoiceValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var failures = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            throw new Exceptions.ValidationException(failures);
        }

        var result = await _invoiceService.UpdateInvoiceDetailsAsync(request);

        if (!result)
        {
            throw new NotFoundException($"Invoice {request.InvoiceNumber} not found");
        }

        return Ok(new { message = "Invoice details updated successfully" });
    }

    /// <summary>
    /// Update invoice line item
    /// </summary>
    /// <param name="request">Updated invoice particular information</param>
    /// <returns>Success confirmation</returns>
    [HttpPut]
    [Route("particulars")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> UpdateInvoiceParticular([FromBody] UpdateInvoiceParticularDto request)
    {
        _logger.LogInformation($"Update invoice particular request received for service {request.ServiceId}");

        // Validate request
        var validationResult = await _updateParticularValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var failures = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            throw new Exceptions.ValidationException(failures);
        }

        var result = await _invoiceService.UpdateInvoiceParticularAsync(request);

        if (!result)
        {
            throw new NotFoundException($"Invoice particular with ID {request.ServiceId} not found");
        }

        return Ok(new { message = "Invoice particular updated successfully" });
    }
}
