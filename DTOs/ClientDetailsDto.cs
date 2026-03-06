namespace LJ.BillingPortal.API.DTOs;

/// <summary>
/// DTO for creating client address
/// </summary>
public class CreateClientDetailsDto
{
    public string BilledToName { get; set; } = null!;
    public string AddressLine1 { get; set; } = null!;
    public string AddressLine2 { get; set; } = null!;
    public string AddressLine3 { get; set; } = null!;
    public string Gstin { get; set; } = null!;
    public string State { get; set; } = null!;
    public string StateCode { get; set; } = null!;
}

/// <summary>
/// DTO for updating client address
/// </summary>
public class UpdateClientDetailsDto
{
    public int ClientId { get; set; }
    public string BilledToName { get; set; } = null!;
    public string AddressLine1 { get; set; } = null!;
    public string AddressLine2 { get; set; } = null!;
    public string AddressLine3 { get; set; } = null!;
    public string Gstin { get; set; } = null!;
    public string State { get; set; } = null!;
    public string StateCode { get; set; } = null!;
}

/// <summary>
/// DTO for responding with client address
/// </summary>
public class ClientDetailsDto
{
    public int ClientId { get; set; }
    public string BilledToName { get; set; } = null!;
    public string AddressLine1 { get; set; } = null!;
    public string AddressLine2 { get; set; } = null!;
    public string AddressLine3 { get; set; } = null!;
    public string Gstin { get; set; } = null!;
    public string State { get; set; } = null!;
    public string StateCode { get; set; } = null!;
}
