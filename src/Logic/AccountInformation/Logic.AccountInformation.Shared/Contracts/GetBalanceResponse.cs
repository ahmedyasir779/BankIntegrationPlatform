namespace Logic.AccountInformation.Shared.Contracts;

public class GetBalanceResponse
{
    public string AccountNumber { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    public string Currency { get; set; } = string.Empty;
}