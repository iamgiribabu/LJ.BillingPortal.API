namespace LJ.BillingPortal.API.Models;

/// <summary>
/// Client details entity - represents billed to information
/// </summary>
public class ClientDetails
{
    public int ClientId { get; set; }
    
    public string BilledToName { get; set; } = null!;
    
    public string AddressLine1 { get; set; } = null!;
    
    public string AddressLine2 { get; set; } = null!;
    
    public string AddressLine3 { get; set; } = null!;
    
    public string Gstin { get; set; } = null!;
    
    public string State { get; set; } = null!;
    
    public string StateCode { get; set; } = null!;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? ModifiedDate { get; set; }
    
    // Navigation property
    public virtual ICollection<InvoiceDetails> Invoices { get; set; } = new List<InvoiceDetails>();
}
