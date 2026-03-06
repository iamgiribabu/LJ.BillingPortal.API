namespace LJ.BillingPortal.API.Services.Interfaces;

/// <summary>
/// Service interface for PDF generation
/// </summary>
public interface IPdfGenerationService
{
    /// <summary>
    /// Generate PDF from HTML content
    /// </summary>
    /// <param name="htmlContent">HTML string content</param>
    /// <param name="fileName">Output file name</param>
    /// <returns>Path to generated PDF file</returns>
    Task<string> GeneratePdfAsync(string htmlContent, string fileName);

    /// <summary>
    /// Generate HTML for invoice
    /// </summary>
    string GenerateInvoiceHtml(
        DTOs.ClientDetailsDto clientDetails,
        DTOs.InvoiceDetailsDto invoiceDetails,
        List<DTOs.InvoiceParticularDto> particulars);
}
