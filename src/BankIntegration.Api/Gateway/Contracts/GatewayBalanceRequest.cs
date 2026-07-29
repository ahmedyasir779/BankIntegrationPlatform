namespace BankIntegration.Api.Gateway.Contracts;

public class GatewayBalanceRequest
{
    public string BankCode { get; set; } = string.Empty;

    public string AccountNumber { get; set; } = string.Empty;
}