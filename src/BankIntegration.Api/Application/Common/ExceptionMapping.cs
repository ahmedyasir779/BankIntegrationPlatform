namespace BankIntegration.Api.Application.Common;

public class ExceptionMapping
{
    public int HttpStatusCode { get; init; }

    public string ErrorCode { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}