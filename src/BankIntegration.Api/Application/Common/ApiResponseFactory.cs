using BankIntegration.Api.Common;
using BankIntegration.Api.Domain.Messages;

namespace BankIntegration.Api.Application.Common;

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
                TimestampUtc = DateTime.UtcNow,

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
    string description,
    string statusType = "Error")
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
                    StatusType = statusType,
                    StatusCode = statusCode,
                    StatusDescription = description,

                }
            }
        };
    }
}