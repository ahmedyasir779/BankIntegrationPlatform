namespace BankIntegrationPlatform.Domain.Messages;

public class ApiResponse<T>
{
    public ResponseHeader Header { get; set; } = new();

    public T Data { get; set; } = default!;
}