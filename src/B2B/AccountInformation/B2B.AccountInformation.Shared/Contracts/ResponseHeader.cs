namespace B2B.AccountInformation.Shared.Contracts;

public class ResponseHeader
{
    public ResponseStatus Status { get; set; } = new();

    public string CorrelationId { get; set; } = string.Empty;

    public string MessageId { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }
}