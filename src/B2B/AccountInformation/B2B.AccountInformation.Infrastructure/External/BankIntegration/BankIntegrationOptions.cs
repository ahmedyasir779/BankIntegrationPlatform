namespace B2B.AccountInformation.Infrastructure.External.BankIntegration;

public class BankIntegrationOptions
{
    public const string SectionName = "BankIntegration";

    public string BaseUrl { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;
}