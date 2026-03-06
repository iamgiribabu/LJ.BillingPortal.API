namespace LJ.BillingPortal.API.Exceptions;

/// <summary>
/// Custom exception for not found scenarios
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Custom exception for validation failures
/// </summary>
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Failures { get; }

    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Failures = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> failures)
        : base("One or more validation failures have occurred.")
    {
        Failures = failures;
    }
}

/// <summary>
/// Custom exception for business rule violations
/// </summary>
public class BusinessLogicException : Exception
{
    public BusinessLogicException(string message) : base(message) { }
    public BusinessLogicException(string message, Exception innerException) : base(message, innerException) { }
}
