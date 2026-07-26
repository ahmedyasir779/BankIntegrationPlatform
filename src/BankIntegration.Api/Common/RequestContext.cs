namespace BankIntegrationPlatform.Common;

public class RequestContext
{
    public Guid CorrelationId { get; set; }

    public Guid MessageId { get; set; }

    public DateTime RequestTimeUtc { get; set; }

    public string ServiceName { get; set; } = string.Empty;
}