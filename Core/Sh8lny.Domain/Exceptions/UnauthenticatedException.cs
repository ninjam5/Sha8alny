namespace Sh8lny.Domain.Exceptions;

/// <summary>
/// Exception thrown when user is not authenticated
/// </summary>
public class UnauthenticatedException : DomainException
{
    public UnauthenticatedException(string message) : base(message)
    {
    }

    public UnauthenticatedException()
        : base("Authentication is required to access this resource.")
    {
    }

    public UnauthenticatedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
