namespace LJ.BillingPortal.API.DTOs;

/// <summary>
/// DTO for creating invoice details
/// </summary>
public class CreateInvoiceDetailsDto
{
    public string InvoiceNumber { get; set; } = null!;
    public DateTime InvoiceDate { get; set; }
    public string PlaceOfSupply { get; set; } = null!;
    public string PoNumber { get; set; } = null!;
    public string CraneReg { get; set; } = null!;
    public decimal TotalAmountBeforeTax { get; set; }
    public decimal Cgst { get; set; }
    public decimal Sgst { get; set; }
    public decimal Igst { get; set; }
    public decimal NetAmountAfterTax { get; set; }
}

/// <summary>
/// DTO for updating invoice details
/// </summary>
public class UpdateInvoiceDetailsDto
{
    public DateTime InvoiceDate { get; set; }
    public string PlaceOfSupply { get; set; } = null!;
    public string PoNumber { get; set; } = null!;
    public string CraneReg { get; set; } = null!;
    public decimal TotalAmountBeforeTax { get; set; }
    public decimal Cgst { get; set; }
    public decimal Sgst { get; set; }
    public decimal Igst { get; set; }
    public decimal NetAmountAfterTax { get; set; }
}

/// <summary>
/// DTO for responding with invoice details
/// </summary>
public class InvoiceDetailsDto
{
    public int InvoiceId { get; set; }
    public int ClientId { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public DateTime InvoiceDate { get; set; }
    public string PlaceOfSupply { get; set; } = null!;
    public string PoNumber { get; set; } = null!;
    public string CraneReg { get; set; } = null!;
    public decimal TotalAmountBeforeTax { get; set; }
    public decimal Cgst { get; set; }
    public decimal Sgst { get; set; }
    public decimal Igst { get; set; }
    public decimal NetAmountAfterTax { get; set; }
}
