namespace Logic.AccountInformation.Shared.Contracts;

public class GetBalanceRequest
{
    public string BankCode { get; set; } = string.Empty;

    public string AccountNumber { get; set; } = string.Empty;
}