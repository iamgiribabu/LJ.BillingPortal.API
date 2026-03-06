namespace LJ.BillingPortal.API.DTOs;

/// <summary>
/// DTO for creating invoice particulars
/// </summary>
public class CreateInvoiceParticularDto
{
    public string Description { get; set; } = null!;
    public string HsnSac { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal TaxableValue { get; set; }
}

/// <summary>
/// DTO for updating invoice particulars
/// </summary>
public class UpdateInvoiceParticularDto
{
    public int ServiceId { get; set; }
    public string Description { get; set; } = null!;
    public string HsnSac { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal TaxableValue { get; set; }
}

/// <summary>
/// DTO for responding with invoice particulars
/// </summary>
public class InvoiceParticularDto
{
    public int ServiceId { get; set; }
    public int InvoiceId { get; set; }
    public string Description { get; set; } = null!;
    public string HsnSac { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal TaxableValue { get; set; }
}
