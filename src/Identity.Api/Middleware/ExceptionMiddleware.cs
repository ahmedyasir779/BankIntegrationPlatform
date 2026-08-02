using Identity.Api.Domain.Exceptions;

namespace Identity.Api.Middleware;

// Middleware responsible for handling all unhandled exceptions
// and converting them into consistent HTTP responses.
public class ExceptionMiddleware
{
    // Store the next middleware in the ASP.NET Core pipeline.
    private readonly RequestDelegate _next;

    // Logger used to record unexpected errors.
    private readonly ILogger<ExceptionMiddleware> _logger;

    // Constructor that receives dependencies from Dependency Injection.
    public ExceptionMiddleware(
        // Reference to the next middleware in the pipeline.
        RequestDelegate next,

        // Logger instance for writing logs.
        ILogger<ExceptionMiddleware> logger)
    {
        // Save the next middleware.
        _next = next;

        // Save the logger instance.
        _logger = logger;
    }

    // Main middleware method executed for every incoming HTTP request.
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Continue processing the request by calling the next middleware.
            await _next(context);
        }

        // Handle duplicate client exceptions.
        catch (ClientAlreadyExistsException ex)
        {
            // Return HTTP 409 (Conflict).
            context.Response.StatusCode = StatusCodes.Status409Conflict;

            // Return a standardized JSON error response.
            await context.Response.WriteAsJsonAsync(new
            {
                error = "client_exists",
                message = ex.Message
            });
        }

        // Handle client not found exceptions.
        catch (ClientNotFoundException ex)
        {
            // Return HTTP 404 (Not Found).
            context.Response.StatusCode = StatusCodes.Status404NotFound;

            // Return a standardized JSON error response.
            await context.Response.WriteAsJsonAsync(new
            {
                error = "client_not_found",
                message = ex.Message
            });
        }

        // Handle invalid authentication requests.
        catch (InvalidClientException ex)
        {
            // Return HTTP 401 (Unauthorized).
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            // Return a standardized JSON error response.
            await context.Response.WriteAsJsonAsync(new
            {
                error = "invalid_client",
                message = ex.Message
            });
        }

        // Handle any unexpected exceptions that were not explicitly caught.
        catch (Exception ex)
        {
            // Log the exception for troubleshooting.
            _logger.LogError(ex, "Unhandled exception");

            // Return HTTP 500 (Internal Server Error).
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // Return a generic error response without exposing internal details.
            await context.Response.WriteAsJsonAsync(new
            {
                error = "server_error",
                message = "Internal server error."
            });
        }
    }
}