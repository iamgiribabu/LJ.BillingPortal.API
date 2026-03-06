namespace LJ.BillingPortal.API.DTOs;

/// <summary>
/// Composite DTO for creating a complete invoice with all details
/// </summary>
public class CreateCompleteInvoiceDto
{
    public CreateClientDetailsDto ClientDetails { get; set; } = null!;
    
    public CreateInvoiceDetailsDto InvoiceDetails { get; set; } = null!;
    
    public List<CreateInvoiceParticularDto> InvoiceParticulars { get; set; } = new();
}

/// <summary>
/// Composite DTO for responding with complete invoice
/// </summary>
public class InvoiceResponseDto
{
    public int InvoiceId { get; set; }
    
    public ClientDetailsDto ClientDetails { get; set; } = null!;
    
    public InvoiceDetailsDto InvoiceDetails { get; set; } = null!;
    
    public List<InvoiceParticularDto> InvoiceParticulars { get; set; } = new();
}

/// <summary>
/// DTO for generating PDF invoice request
/// </summary>
public class GenerateInvoiceRequest
{
    public ClientDetailsDto ClientDetails { get; set; } = null!;
    
    public InvoiceDetailsDto InvoiceDetails { get; set; } = null!;
    
    public List<InvoiceParticularDto> InvoiceParticulars { get; set; } = new();
}

/// <summary>
/// DTO for PDF generation response
/// </summary>
public class PdfGenerationResponse
{
    public string Message { get; set; } = null!;
    
    public string Url { get; set; } = null!;
}
