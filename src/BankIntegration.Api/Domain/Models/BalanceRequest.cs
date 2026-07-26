namespace BankIntegrationPlatform.Domain.Models;

public class BalanceRequest
{
    public string AccountNumber { get; set; } = string.Empty;

    public string BankCode { get; set; } = string.Empty;
}