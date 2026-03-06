using LJ.BillingPortal.API.DTOs;
using LJ.BillingPortal.API.Services.Interfaces;
using System.Globalization;

namespace LJ.BillingPortal.API.Services;

/// <summary>
/// Service implementation for PDF generation using SelectPdf
/// </summary>
public class PdfGenerationService : IPdfGenerationService
{
    private readonly ILogger<PdfGenerationService> _logger;
    private readonly string _invoiceOutputPath;

    public PdfGenerationService(
        ILogger<PdfGenerationService> logger,
        IWebHostEnvironment environment)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _invoiceOutputPath = Path.Combine(environment.WebRootPath, "invoices");

        if (!Directory.Exists(_invoiceOutputPath))
        {
            Directory.CreateDirectory(_invoiceOutputPath);
        }
    }

    public async Task<string> GeneratePdfAsync(string htmlContent, string fileName)
    {
        _logger.LogInformation($"Generating Invoice HTML: {fileName}");

        return await Task.Run(() =>
        {
            try
            {
                // Save HTML content to file
                var htmlFileName = Path.ChangeExtension(fileName, ".html");
                var htmlFilePath = Path.Combine(_invoiceOutputPath, htmlFileName);
                File.WriteAllText(htmlFilePath, htmlContent);

                _logger.LogInformation($"Invoice HTML generated successfully: {htmlFileName}");
                return $"/invoices/{htmlFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating invoice HTML: {fileName}");
                throw;
            }
        });
    }

    public string GenerateInvoiceHtml(
        ClientDetailsDto clientDetails,
        InvoiceDetailsDto invoiceDetails,
        List<InvoiceParticularDto> particulars)
    {
        _logger.LogInformation($"Generating HTML for invoice {invoiceDetails.InvoiceNumber}");

        var cultureInfo = new CultureInfo("en-IN");
        var particularsHtml = GenerateParticularsHtml(particulars);
        var totalGst = invoiceDetails.Cgst + invoiceDetails.Sgst + invoiceDetails.Igst;

        return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Invoice {invoiceDetails.InvoiceNumber}</title>
    <link href='https://fonts.googleapis.com/css2?family=Cinzel:wght@400;600;700&display=swap' rel='stylesheet'>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}

        body {{
            font-family: 'Cinzel', 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: white;
            color: #333;
            line-height: 1.6;
        }}

        .invoice-container {{
            width: 8.27in;
            height: 11.69in;
            margin: 0 auto;
            padding: 40px 30px;
            background: white;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
        }}

        .header {{
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 30px;
            border-bottom: 2px solid #2c3e50;
            padding-bottom: 20px;
        }}

        .company-info {{
            flex: 1;
        }}

        .company-logo {{
            font-size: 24px;
            font-weight: 700;
            color: #2c3e50;
            margin-bottom: 5px;
        }}

        .invoice-meta {{
            text-align: right;
        }}

        .invoice-meta-item {{
            margin: 5px 0;
            font-size: 12px;
        }}

        .invoice-number {{
            font-size: 18px;
            font-weight: 700;
            color: #2c3e50;
        }}

        .section-title {{
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            margin-top: 20px;
            margin-bottom: 10px;
            color: #2c3e50;
            border-bottom: 1px solid #ecf0f1;
            padding-bottom: 5px;
        }}

        .address-section {{
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 30px;
            margin: 20px 0;
            font-size: 11px;
        }}

        .address-block {{
            line-height: 1.8;
        }}

        .address-block-title {{
            font-weight: 700;
            margin-bottom: 5px;
            font-size: 12px;
        }}

        table {{
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
            font-size: 11px;
        }}

        th {{
            background-color: #2c3e50;
            color: white;
            padding: 10px;
            text-align: left;
            font-size: 11px;
            font-weight: 700;
            border: 1px solid #2c3e50;
        }}

        td {{
            padding: 10px;
            border: 1px solid #ecf0f1;
        }}

        tr:nth-child(even) {{
            background-color: #f8f9fa;
        }}

        .text-right {{
            text-align: right;
        }}

        .text-center {{
            text-align: center;
        }}

        .amount {{
            font-weight: 600;
        }}

        .totals-section {{
            margin-top: 20px;
            text-align: right;
        }}

        .total-row {{
            display: grid;
            grid-template-columns: 2fr 1fr;
            gap: 10px;
            padding: 8px 0;
            font-size: 11px;
            border-bottom: 1px solid #ecf0f1;
        }}

        .total-row.grand-total {{
            border-top: 2px solid #2c3e50;
            border-bottom: 2px solid #2c3e50;
            font-weight: 700;
            font-size: 13px;
            padding: 10px 0;
        }}

        .footer {{
            margin-top: 40px;
            padding-top: 20px;
            border-top: 1px solid #ecf0f1;
            font-size: 10px;
            text-align: center;
            color: #7f8c8d;
        }}

        .gstin-label {{
            font-weight: 700;
            font-size: 10px;
        }}
    </style>
</head>
<body>
    <div class='invoice-container'>
        <!-- Header -->
        <div class='header'>
            <div class='company-info'>
                <div class='company-logo'>LJ Lifters</div>
            </div>
            <div class='invoice-meta'>
                <div class='invoice-number'>Invoice # {invoiceDetails.InvoiceNumber}</div>
                <div class='invoice-meta-item'>
                    <strong>Invoice Date:</strong> {invoiceDetails.InvoiceDate:dd-MMM-yyyy}
                </div>
                <div class='invoice-meta-item'>
                    <strong>Place of Supply:</strong> {invoiceDetails.PlaceOfSupply}
                </div>
            </div>
        </div>

        <!-- Bill To Section -->
        <div class='section-title'>Bill To</div>
        <div class='address-section'>
            <div class='address-block'>
                <div class='address-block-title'>{clientDetails.BilledToName}</div>
                <div>{clientDetails.AddressLine1}</div>
                <div>{clientDetails.AddressLine2}</div>
                <div>{clientDetails.AddressLine3}</div>
                <div class='gstin-label'>GSTIN: {clientDetails.Gstin}</div>
                <div>State: {clientDetails.State} ({clientDetails.StateCode})</div>
            </div>
            <div class='address-block'>
                <div class='address-block-title'>Purchase Order Details</div>
                <div><strong>PO Number:</strong> {invoiceDetails.PoNumber}</div>
                <div><strong>Crane Reg:</strong> {invoiceDetails.CraneReg}</div>
            </div>
        </div>

        <!-- Items Table -->
        <div class='section-title'>Invoice Items</div>
        <table>
            <thead>
                <tr>
                    <th style='width: 35%'>Description</th>
                    <th style='width: 15%'>HSN/SAC</th>
                    <th style='width: 10%'>Qty</th>
                    <th style='width: 15%; text-align: right;'>Rate</th>
                    <th style='width: 15%; text-align: right;'>Amount</th>
                </tr>
            </thead>
            <tbody>
                {particularsHtml}
            </tbody>
        </table>

        <!-- Totals Section -->
        <div class='totals-section'>
            <div class='total-row'>
                <div>Subtotal (Before Tax)</div>
                <div class='amount text-right'>₹ {invoiceDetails.TotalAmountBeforeTax.ToString("N", cultureInfo)}</div>
            </div>
            <div class='total-row'>
                <div>CGST (9%)</div>
                <div class='amount text-right'>₹ {invoiceDetails.Cgst.ToString("N", cultureInfo)}</div>
            </div>
            <div class='total-row'>
                <div>SGST (9%)</div>
                <div class='amount text-right'>₹ {invoiceDetails.Sgst.ToString("N", cultureInfo)}</div>
            </div>
            <div class='total-row'>
                <div>IGST (18%)</div>
                <div class='amount text-right'>₹ {invoiceDetails.Igst.ToString("N", cultureInfo)}</div>
            </div>
            <div class='total-row grand-total'>
                <div>Total Amount After Tax</div>
                <div class='text-right'>₹ {invoiceDetails.NetAmountAfterTax.ToString("N", cultureInfo)}</div>
            </div>
        </div>

        <!-- Footer -->
        <div class='footer'>
            <p>Thank you for your business!</p>
            <p>This is a computer-generated invoice</p>
        </div>
    </div>
</body>
</html>";
    }

    private string GenerateParticularsHtml(List<InvoiceParticularDto> particulars)
    {
        var cultureInfo = new CultureInfo("en-IN");
        var html = new System.Text.StringBuilder();

        foreach (var particular in particulars)
        {
            html.AppendLine($@"
                <tr>
                    <td>{particular.Description}</td>
                    <td class='text-center'>{particular.HsnSac}</td>
                    <td class='text-center'>{particular.Quantity}</td>
                    <td class='text-right'>₹ {particular.Rate.ToString("N", cultureInfo)}</td>
                    <td class='text-right'>₹ {particular.TaxableValue.ToString("N", cultureInfo)}</td>
                </tr>");
        }

        return html.ToString();
    }
}
