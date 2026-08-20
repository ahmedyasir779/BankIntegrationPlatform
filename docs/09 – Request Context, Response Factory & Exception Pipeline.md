# 09 – Request Context, Response Factory & Exception Pipeline

---

# Overview

As our Bank Integration Platform grows, simply building controllers and services is no longer enough.

Enterprise applications need a consistent way to:

- Track requests across multiple services.
- Generate standardised API responses.
- Handle errors in one place.
- Produce structured logs.
- Share request information throughout the application.

If every controller manually generated response headers, created message identifiers, and handled exceptions independently, the application would quickly become difficult to maintain.

To solve these problems we introduced several architectural components:

- Request Context
- Request Context Accessor
- Correlation Middleware
- API Response Factory
- Global Exception Middleware
- Structured Logging

Together these components form the application's request pipeline and establish the foundation required for future features such as OAuth, distributed tracing, microservices, and OpenTelemetry.

---

# Why Do We Need a Request Context?

Every incoming HTTP request carries information that multiple layers of the application need.

Examples include:

- Correlation ID
- Message ID
- Request timestamp
- Service name
- API version
- HTTP method
- Request path

Without a shared object, every controller and service would repeatedly access `HttpContext` or generate these values independently.

Instead, we create the information once and make it available everywhere.

---

# Before Request Context

Before introducing a shared context, our controller needed to retrieve values directly from `HttpContext`.

```csharp
Guid correlationId = Guid.Empty;

if (HttpContext.Items.TryGetValue(
    HttpContextKeys.CorrelationId,
    out var value))
{
    Guid.TryParse(value?.ToString(), out correlationId);
}
```

Every controller that required the Correlation ID would need to duplicate this code.

This creates unnecessary repetition and tightly couples controllers to ASP.NET Core.

---

# After Request Context

Now every component simply accesses:

```csharp
_requestContext.Context
```

Example:

```csharp
_logger.LogInformation(
    "Processing request {CorrelationId}",
    _requestContext.Context.CorrelationId);
```

The application no longer depends directly on `HttpContext`.

---

# What is Request Context?

Request Context is a simple object that stores metadata describing the current request.

Our implementation currently contains:

```csharp
public class RequestContext
{
    public Guid CorrelationId { get; set; }

    public Guid MessageId { get; set; }

    public DateTime RequestTimeUtc { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public string ApiVersion { get; set; } = string.Empty;

    public string RequestPath { get; set; } = string.Empty;

    public string HttpMethod { get; set; } = string.Empty;
}
```

Rather than scattering this information throughout the application, it is stored in one place.

Every component involved in processing the request receives exactly the same information.

---

# Correlation ID

A Correlation ID identifies an entire business transaction.

Imagine the future architecture of our platform.

```text
Client

↓

Identity

↓

B2B

↓

Logic

↓

INT

↓

Gateway

↓

SNB
```

A request may pass through several independent services.

Every service should receive exactly the same Correlation ID.

This allows engineers to follow one transaction from beginning to end.

For example:

```text
CorrelationId

191e1e1f-df2c-43fe-9e2a-b701943c5a75
```

Searching for this value in production logs immediately reveals every service involved in that request.

---

# Message ID

Message ID represents an individual message rather than an entire transaction.

Each request receives a unique Message ID.

Example:

```text
Client Request

↓

Message A

↓

Identity

↓

Message B

↓

B2B

↓

Message C

↓

Gateway
```

The Correlation ID remains constant.

The Message ID changes for every message exchanged.

This distinction becomes especially important when asynchronous messaging is introduced later in the project.

---

# Correlation ID vs Message ID

Although both are GUIDs, they serve different purposes.

| Correlation ID | Message ID |
|----------------|------------|
| Represents a complete business transaction | Represents a single message |
| Shared across multiple services | Unique for every request or response |
| Used for distributed tracing | Used for message tracking |
| Remains constant throughout the transaction | Changes whenever a new message is created |

Enterprise integration platforms typically use both values together.

---

# Correlation Middleware

The first component executed for every incoming request is the Correlation Middleware.

Its responsibilities include:

- Reading an existing Correlation ID from the request.
- Generating one if none exists.
- Creating a new Message ID.
- Recording the request timestamp.
- Building the Request Context.
- Storing the Request Context.
- Returning the Correlation ID in the response headers.

The middleware executes before the request reaches any controller.

```text
HTTP Request

↓

Correlation Middleware

↓

Request Context Created

↓

Controller
```

Every subsequent layer receives the same Request Context.

---

# RequestContextAccessor

Although the middleware creates the Request Context, other layers still need a way to access it.

Instead of exposing `HttpContext`, we introduce a dedicated accessor.

```csharp
public interface IRequestContextAccessor
{
    RequestContext Context { get; }
}
```

The implementation simply exposes the Request Context associated with the current request.

This provides a clean abstraction and avoids coupling application services to ASP.NET Core infrastructure.

---

# Dependency Injection

The accessor is registered using Dependency Injection.

```csharp
builder.Services.AddScoped<
    IRequestContextAccessor,
    RequestContextAccessor>();
```

Every request receives its own Request Context.

This aligns perfectly with the Scoped service lifetime because request metadata should never be shared between users.

---

# Using RequestContext in Services

Services can now retrieve request information without accessing `HttpContext`.

Example:

```csharp
_logger.LogInformation(
    "CorrelationId: {CorrelationId}",
    _requestContext.Context.CorrelationId);
```

The service knows nothing about controllers, middleware, or ASP.NET Core.

It simply consumes the information it requires.

This improves separation of concerns and keeps business logic independent from the web framework.

---

# ApiResponseFactory

One of the principles of good API design is that every endpoint should return responses in a consistent format.

Imagine having twenty controllers.

If every controller manually builds the response object, then every developer may create it slightly differently.

For example:

```csharp
return new ApiResponse<BalanceResponse>
{
    Header = ...
    Data = response
};
```

Soon the application contains duplicated code everywhere.

To solve this problem we introduced an **ApiResponseFactory**.

The factory is responsible for building every response returned by the application.

Controllers simply provide the data.

---

# Before ApiResponseFactory

Originally every controller contained something similar to:

```csharp
var apiResponse = new ApiResponse<BalanceResponse>
{
    Header = new ResponseHeader
    {
        CorrelationId = correlationId,
        MessageId = Guid.NewGuid(),
        TimestampUtc = DateTime.UtcNow,

        Status = new ResponseStatus
        {
            StatusType = "Success",
            StatusCode = "000",
            StatusDescription = "Request completed successfully."
        }
    },

    Data = response
};

return Ok(apiResponse);
```

Every endpoint duplicated this logic.

---

# After ApiResponseFactory

Now controllers become much simpler.

```csharp
return Ok(_responseFactory.Success(response));
```

All response construction happens inside one reusable class.

---

# Success Responses

The factory exposes a method for successful operations.

```csharp
_responseFactory.Success(response);
```

Internally it automatically:

- Reads the Request Context.
- Copies the Correlation ID.
- Copies the Message ID.
- Adds the timestamp.
- Creates a standard Success status.
- Wraps the response data.

Controllers only provide the business result.

---

# Failure Responses

Errors follow exactly the same pattern.

Instead of manually constructing error responses:

```csharp
_responseFactory.Failure<object>(
    "404",
    "No adapter registered.");
```

The factory generates a consistent response structure.

This ensures every error returned by the application follows the same format.

---

# Benefits of ApiResponseFactory

Centralising response generation provides several advantages.

- Eliminates duplicated code.
- Standardises API responses.
- Simplifies controllers.
- Makes future changes easier.
- Improves maintainability.

If tomorrow we decide to add:

- Request Duration
- Server Name
- Environment
- API Version

we only update one class.

---

# Global Exception Handling

Applications inevitably encounter errors.

Examples include:

- Invalid requests.
- Missing resources.
- External system failures.
- Database errors.
- Unexpected exceptions.

Without centralised exception handling, every controller would need:

```csharp
try
{
}
catch
{
}
```

This quickly becomes repetitive.

---

# Exception Pipeline

Instead of catching exceptions inside controllers, we allow them to propagate through the application.

```text
Controller

↓

BankService

↓

Adapter

↓

Exception

↓

Exception Middleware

↓

ApiResponseFactory

↓

HTTP Response
```

Only one component is responsible for handling errors.

---

# Exception Middleware

The Exception Middleware sits near the beginning of the ASP.NET Core pipeline.

Its responsibilities include:

- Catching unhandled exceptions.
- Logging the exception.
- Mapping the exception to an HTTP status code.
- Creating a standard error response.
- Returning JSON to the client.

Every controller automatically benefits from this behaviour.

---

# Exception Mapping

Different exceptions represent different problems.

Our middleware translates them into appropriate HTTP status codes.

| Exception | HTTP Status |
|-----------|------------:|
| BankAdapterNotFoundException | 404 |
| ArgumentException | 400 |
| UnauthorizedAccessException | 401 |
| Exception | 500 |

Clients receive meaningful responses without controllers needing to understand every possible failure.

---

# Example Error Response

If a client requests an unknown bank:

```json
{
    "bankCode": "ABC"
}
```

The response becomes:

```json
{
    "header": {
        "status": {
            "statusCode": "404",
            "statusDescription": "No adapter registered for bank 'ABC'."
        }
    },
    "data": null
}
```

Every error follows exactly the same structure.

---

# Structured Logging

Logging is far more useful when information is recorded as structured data rather than plain text.

Instead of:

```text
Processing request...
```

we log:

- Correlation ID
- Message ID
- Bank
- Adapter
- Duration
- HTTP Method
- Request Path

Example:

```text
Processing balance request.

CorrelationId:
191e1e1f...

MessageId:
6edc0182...

Bank:
SNB
```

Structured logs can easily be searched and filtered in production systems.

---

# Measuring Request Duration

Understanding application performance is essential.

We introduced a Stopwatch to measure execution time.

Example:

```csharp
var stopwatch = Stopwatch.StartNew();

...

stopwatch.Stop();
```

The elapsed time is written to the logs.

Example:

```text
Duration:
214 ms
```

Later this information can be stored in a database or visualised using monitoring tools.

---

# Current Request Flow

The current request lifecycle is:

```text
Client

↓

POST /api/v1/balance

↓

Correlation Middleware

↓

Request Context

↓

Balance Controller

↓

Bank Service

↓

Adapter Registry

↓

SNB Adapter

↓

Typed HttpClient

↓

Bank Mock Server

↓

Response

↓

ApiResponseFactory

↓

HTTP Response
```

Each component performs a single responsibility.

---

# Why This Architecture Matters

Enterprise integration platforms process thousands of requests every day.

Without:

- Request tracking
- Centralised responses
- Global exception handling
- Structured logging

diagnosing production issues becomes extremely difficult.

These architectural patterns provide the foundation for scalable and maintainable systems.

---

# Common Mistakes

Developers often make the following mistakes.

- Returning different response formats from different controllers.
- Using try/catch inside every endpoint.
- Accessing HttpContext throughout the application.
- Creating Correlation IDs in multiple places.
- Logging plain strings without structured data.

Avoiding these issues keeps the architecture clean and predictable.

---

# Best Practices

- Create request metadata once.
- Share Request Context using Dependency Injection.
- Keep controllers lightweight.
- Handle exceptions globally.
- Standardise all API responses.
- Log structured information.
- Measure request execution time.
- Separate infrastructure concerns from business logic.

---

# Interview Questions

1. What is a Request Context?
2. Why do we use Correlation IDs?
3. What is the difference between a Correlation ID and a Message ID?
4. Why should services avoid accessing HttpContext directly?
5. What is the purpose of an ApiResponseFactory?
6. Why is global exception handling preferred over try/catch in controllers?
7. What is structured logging?
8. Why do we measure request duration?
9. How does middleware participate in the ASP.NET Core request pipeline?
10. What are the benefits of centralising response generation?

---

# Key Takeaways

- Request Context stores metadata shared throughout a request.
- Correlation IDs allow requests to be traced across multiple services.
- Message IDs uniquely identify individual messages.
- Controllers should focus on business operations rather than infrastructure concerns.
- ApiResponseFactory ensures every endpoint returns a consistent response.
- Global exception handling eliminates duplicated try/catch blocks.
- Structured logging makes production diagnostics significantly easier.
- Measuring request duration lays the groundwork for future monitoring and observability.

---

# Next Chapter

**10 – OAuth 2.0, Identity Server & Authentication**

In the next chapter we will begin securing the platform by introducing an Identity Server responsible for authenticating clients, issuing JWT access tokens, validating scopes, and protecting the B2B API. This marks the transition from a functional Web API to a production-style banking integration platform with enterprise-grade authentication and authorisation.