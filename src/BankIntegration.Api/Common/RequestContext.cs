namespace BankIntegrationPlatform.Common;

public class RequestContext
{
    public Guid CorrelationId { get; set; }

    public Guid MessageId { get; set; }

    public DateTime RequestTimeUtc { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public string ApiVersion { get; set; } = string.Empty;

    public string RequestPath { get; set; } = string.Empty;

    public string HttpMethod { get; set; } = string.Empty;
}