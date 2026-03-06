namespace LJ.BillingPortal.API.Models;

/// <summary>
/// Invoice details entity - represents invoice Header information
/// </summary>
public class InvoiceDetails
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
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? ModifiedDate { get; set; }
    
    // Navigation properties
    public virtual ClientDetails Client { get; set; } = null!;
    
    public virtual ICollection<InvoiceParticular> Particulars { get; set; } = new List<InvoiceParticular>();
}
