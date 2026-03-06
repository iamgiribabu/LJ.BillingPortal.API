namespace LJ.BillingPortal.API.Models;

/// <summary>
/// Invoice particular entity - represents invoice line items
/// </summary>
public class InvoiceParticular
{
    public int ServiceId { get; set; }
    
    public int InvoiceId { get; set; }
    
    public string Description { get; set; } = null!;
    
    public string HsnSac { get; set; } = null!;
    
    public int Quantity { get; set; }
    
    public decimal Rate { get; set; }
    
    public decimal TaxableValue { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual InvoiceDetails Invoice { get; set; } = null!;
}
