namespace LJ.BillingPortal.API.Services.Models;

/// <summary>
/// Represents the result of a parallel operation
/// </summary>
public class OperationResult
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Service ID of the particular being processed
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// Error message if the operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}
