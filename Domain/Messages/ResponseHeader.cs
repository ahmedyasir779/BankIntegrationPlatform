using System;

namespace BankIntegrationPlatform.Domain.Messages;

public class ResponseHeader
{
    public Guid MessageId { get; set; }

    public Guid CorrelationId { get; set; }

    public DateTime TimestampUtc { get; set; }

    public ResponseStatus Status { get; set; } = new();
}