namespace BankIntegration.Api.Infrastructure.Configurations;

public class BankOptions
{
    public Dictionary<string, BankConfiguration> Banks { get; set; } = [];
}