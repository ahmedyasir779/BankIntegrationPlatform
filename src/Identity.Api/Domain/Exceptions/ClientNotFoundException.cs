namespace Identity.Api.Domain.Exceptions;

public class ClientNotFoundException : Exception
{
    public ClientNotFoundException(int id)
        : base($"Client with id '{id}' was not found.")
    {
    }
}