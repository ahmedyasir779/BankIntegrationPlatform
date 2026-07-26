namespace BankIntegrationPlatform.Domain.Messages;

public class RequestHeader
{
    public Guid MessageId { get; set; }

    public Guid CorrelationId { get; set; }

    public DateTime TimestampUtc { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public string ApiVersion { get; set; } = "v1";
}