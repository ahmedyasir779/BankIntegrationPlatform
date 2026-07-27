using BankIntegrationPlatform.Domain.Messages;

namespace BankIntegrationPlatform.Application.Common;

public interface IApiResponseFactory
{
    ApiResponse<T> Success<T>(T data);

    ApiResponse<T> Failure<T>(
        string statusCode,
        string description,
        string statusType = "Error");
}