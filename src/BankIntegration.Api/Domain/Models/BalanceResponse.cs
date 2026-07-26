namespace BankIntegrationPlatform.Domain.Models;

public class BalanceResponse
{
    public string AccountNumber { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    public string Currency { get; set; } = string.Empty;
}