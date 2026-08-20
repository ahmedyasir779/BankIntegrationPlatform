namespace Logic.AccountInformation.Shared.Requests;

public class BalanceRequest
{
    public string BankCode { get; set; } = string.Empty;

    public string AccountNumber { get; set; } = string.Empty;
}