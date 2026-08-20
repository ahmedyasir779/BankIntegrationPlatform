namespace B2B.AccountInformation.Shared.Contracts;

public class ApiResponse<T>
{
    public ResponseHeader Header { get; set; } = new();

    public T? Data { get; set; }
}