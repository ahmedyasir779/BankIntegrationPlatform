namespace Logic.AccountInformation.Shared.Contracts;

public class ErrorResponse
{
    public string Code { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string? Details { get; set; }
}