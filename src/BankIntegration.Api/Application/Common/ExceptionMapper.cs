// using BankIntegrationPlatform.Domain.Exceptions;

namespace BankIntegrationPlatform.Application.Common;

public static class ExceptionMapper
{
    public static ExceptionMapping Map(Exception exception)
    {
        return exception switch
        {
            BankAdapterNotFoundException => new ExceptionMapping
            {
                HttpStatusCode = StatusCodes.Status404NotFound,
                ErrorCode = "404",
                Description = exception.Message
            },

            ArgumentException => new ExceptionMapping
            {
                HttpStatusCode = StatusCodes.Status400BadRequest,
                ErrorCode = "400",
                Description = exception.Message
            },

            UnauthorizedAccessException => new ExceptionMapping
            {
                HttpStatusCode = StatusCodes.Status401Unauthorized,
                ErrorCode = "401",
                Description = exception.Message
            },

            _ => new ExceptionMapping
            {
                HttpStatusCode = StatusCodes.Status500InternalServerError,
                ErrorCode = "500",
                Description = "Internal server error."
            }
        };
    }
}