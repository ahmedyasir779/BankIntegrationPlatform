namespace BankIntegration.Api.Gateway.Contracts;

public class GatewayBalanceResponse
{
    public string AccountNumber { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    public string Currency { get; set; } = string.Empty;
}