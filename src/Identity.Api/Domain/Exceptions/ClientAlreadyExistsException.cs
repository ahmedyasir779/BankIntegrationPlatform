namespace Identity.Api.Domain.Exceptions;

public class ClientAlreadyExistsException : Exception
{
    public ClientAlreadyExistsException(string clientId)
        : base($"Client '{clientId}' already exists.")
    {
    }
}