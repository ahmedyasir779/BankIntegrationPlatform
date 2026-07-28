namespace BankIntegration.Api.Infrastructure.Configurations;

public class BankConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;

    public string BalanceEndpoint { get; set; } = string.Empty;

    public int Timeout { get; set; }

    public string Authentication { get; set; } = string.Empty;
}