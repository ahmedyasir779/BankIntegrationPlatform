namespace BankIntegrationPlatform.Domain.Messages;

public class ResponseStatus
{
    public string StatusType { get; set; } = string.Empty;

    public string StatusCode { get; set; } = string.Empty;

    public string StatusDescription { get; set; } = string.Empty;
}