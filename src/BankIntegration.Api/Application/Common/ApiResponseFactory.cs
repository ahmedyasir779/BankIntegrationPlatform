using BankIntegrationPlatform.Common;
using BankIntegrationPlatform.Domain.Messages;

namespace BankIntegrationPlatform.Application.Common;

public class ApiResponseFactory : IApiResponseFactory
{
    private readonly IRequestContextAccessor _requestContext;

    public ApiResponseFactory(
        IRequestContextAccessor requestContext)
    {
        _requestContext = requestContext;
    }

    public ApiResponse<T> Success<T>(T data)
    {
        var context = _requestContext.Context;

        return new ApiResponse<T>
        {
            Header = new ResponseHeader
            {
                CorrelationId = context.CorrelationId,
                MessageId = context.MessageId,
                TimestampUtc = context.RequestTimeUtc,

                Status = new ResponseStatus
                {
                    StatusType = "Success",
                    StatusCode = "000",
                    StatusDescription = "Request completed successfully."
                }
            },

            Data = data
        };
    }

    public ApiResponse<T> Failure<T>(
    string statusCode,
    string description)
    {
        var context = _requestContext.Context;

        return new ApiResponse<T>
        {
            Header = new ResponseHeader
            {
                CorrelationId = context.CorrelationId,
                MessageId = context.MessageId,
                TimestampUtc = DateTime.UtcNow,

                Status = new ResponseStatus
                {
                    StatusType = "Error",
                    StatusCode = statusCode,
                    StatusDescription = description
                }
            }
        };
    }
}