namespace Identity.Api.Domain.Exceptions;

public class InvalidClientException : Exception
{
    public InvalidClientException()
        : base("Invalid client credentials.")
    {
    }
}