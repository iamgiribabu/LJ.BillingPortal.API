namespace LJ.BillingPortal.API.Exceptions;

/// <summary>
/// Exception thrown when parallel invoice creation operations fail
/// </summary>
public class InvoiceCreationException : Exception
{
    public List<string> FailedOperations { get; }

    public InvoiceCreationException(string message)
        : base(message)
    {
        FailedOperations = new List<string>();
    }

    public InvoiceCreationException(string message, List<string> failedOperations)
        : base(message)
    {
        FailedOperations = failedOperations;
    }

    public InvoiceCreationException(string message, Exception innerException)
        : base(message, innerException)
    {
        FailedOperations = new List<string>();
    }
}
