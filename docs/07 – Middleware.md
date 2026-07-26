# 07 – Middleware

---

# Overview

Middleware is one of the most powerful and distinctive features of ASP.NET Core.

Every HTTP request entering an ASP.NET Core application passes through a sequence of middleware components before reaching a controller. Likewise, every HTTP response travels back through the same middleware before being sent to the client.

Middleware enables us to implement functionality that applies to every request, such as:

* Logging
* Authentication
* Authorization
* Exception handling
* Correlation IDs
* Rate limiting
* Response compression
* Caching
* Request validation

Rather than duplicating this logic in every controller, we write it once in middleware.

Our Bank Integration Platform already uses middleware for Correlation IDs and global exception handling.

---

# What is Middleware?

A middleware component is a class that sits inside the ASP.NET Core request pipeline.

It receives an incoming request, performs some work, and then decides whether to:

* Continue to the next middleware.
* Modify the request.
* Modify the response.
* End the request early.

Each middleware is responsible for **one specific concern**.

---

# The Middleware Pipeline

The middleware pipeline is configured in `Program.cs`.

Our current pipeline is:

```csharp
app.UseHttpsRedirection();

app.UseMiddleware<CorrelationMiddleware>();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();
```

The order of these statements determines the order in which requests are processed.

Middleware order is critical.

---

# Request Flow

Every request currently follows this path:

```text
Client

↓

Kestrel

↓

HttpsRedirection

↓

CorrelationMiddleware

↓

ExceptionMiddleware

↓

Routing

↓

Controller

↓

Application Service

↓

Adapter

↓

Controller

↑

ExceptionMiddleware

↑

CorrelationMiddleware

↑

Client
```

Notice that middleware executes **twice**:

* Once on the way into the application.
* Once on the way back out.

This is why middleware is sometimes described as wrapping the rest of the pipeline.

---

# Anatomy of a Middleware Class

Every middleware has the same basic structure.

```csharp
public class SampleMiddleware
{
    private readonly RequestDelegate _next;

    public SampleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Before

        await _next(context);

        // After
    }
}
```

Let's examine each part.

---

# RequestDelegate

```csharp
private readonly RequestDelegate _next;
```

`RequestDelegate` represents the **next component** in the pipeline.

Calling:

```csharp
await _next(context);
```

passes control to the next middleware or, eventually, the controller.

If `_next(context)` is never called, the pipeline stops immediately and the controller is never reached.

---

# HttpContext

Every middleware receives an `HttpContext`.

This object contains everything related to the current request.

Examples include:

* Request headers
* Response headers
* Request body
* Response body
* User identity
* Query string
* Route values
* Cookies
* Services
* `Items` collection

Because the same `HttpContext` instance is shared throughout the request, middleware and controllers can exchange information using it.

---

# Before and After the Request

One of middleware's most important characteristics is that it executes code both **before** and **after** the next component.

Example:

```csharp
public async Task InvokeAsync(HttpContext context)
{
    Console.WriteLine("Before");

    await _next(context);

    Console.WriteLine("After");
}
```

Execution order:

```text
Middleware A (Before)

↓

Middleware B (Before)

↓

Controller

↑

Middleware B (After)

↑

Middleware A (After)
```

This behaviour is similar to pushing and popping items from a stack.

---

# CorrelationMiddleware

The first custom middleware we implemented was:

```text
CorrelationMiddleware
```

Its responsibilities are:

* Read the `X-Correlation-Id` header.
* Generate one if the client did not provide it.
* Store it in `HttpContext.Items`.
* Return the same value in the response header.

This ensures that every request has a unique identifier.

---

# Why We Need Correlation IDs

Imagine a request travelling through five microservices.

```text
Gateway

↓

Integration

↓

Payments

↓

Notification

↓

Audit
```

Without a shared Correlation ID, each service writes unrelated log entries.

Finding all logs for one request becomes extremely difficult.

With a Correlation ID:

```text
7d6d...

Gateway

↓

Integration

↓

Payments

↓

Notification

↓

Audit
```

Every log entry contains the same identifier, making it possible to trace the entire request.

---

# Storing Data in HttpContext.Items

Inside our middleware we use:

```csharp
context.Items[HttpContextKeys.CorrelationId] = correlationId;
```

`HttpContext.Items` is a temporary dictionary that exists only for the lifetime of the current request.

It is ideal for passing information between middleware, controllers, and services.

At the end of the request, it is automatically discarded.

---

# Why We Created HttpContextKeys

Instead of repeatedly writing:

```csharp
context.Items["CorrelationId"]
```

we created:

```csharp
public static class HttpContextKeys
{
    public const string CorrelationId = "CorrelationId";
}
```

Benefits:

* Avoids magic strings.
* Reduces typing mistakes.
* Easier refactoring.
* Centralises key names.

---

# Response.OnStarting()

One of the most important concepts we encountered was:

```csharp
context.Response.OnStarting(() =>
{
    context.Response.Headers["X-Correlation-Id"] = correlationId;

    return Task.CompletedTask;
});
```

Initially we tried adding the response header **after**:

```csharp
await _next(context);
```

This caused the exception:

> Headers are read-only, response has already started.

Why?

Because once ASP.NET Core begins sending the response to the client, the headers become locked.

`OnStarting()` registers a callback that executes **immediately before** the response headers are sent.

This guarantees that our Correlation ID is included in every response.

---

# ExceptionMiddleware

Our second middleware is:

```text
ExceptionMiddleware
```

Its responsibilities are:

* Wrap the rest of the pipeline in a `try...catch`.
* Catch unhandled exceptions.
* Return a standard error response.
* Preserve the Correlation ID.
* Prevent the application from crashing.

Without this middleware, every controller would need repetitive exception handling.

---

# Standard Error Responses

Instead of allowing ASP.NET Core to return different error formats, we return:

```text
ApiResponse<T>
```

containing:

```text
Header

↓

ResponseStatus

↓

Data = null
```

This gives clients a predictable response structure for both success and failure.

---

# Middleware Order Matters

Consider this configuration:

```csharp
app.UseMiddleware<CorrelationMiddleware>();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();
```

This works because:

1. Correlation ID is created first.
2. Exception middleware runs second.
3. Controllers execute.

If an exception occurs, the Correlation ID already exists and can be included in the error response.

If the order were reversed, the Correlation ID might not be available when handling exceptions.

---

# Why Middleware Instead of Controllers?

Imagine implementing Correlation IDs inside every controller.

```text
BalanceController

↓

Generate Correlation ID

↓

TransferController

↓

Generate Correlation ID

↓

CustomerController

↓

Generate Correlation ID
```

The same code would be repeated throughout the application.

Middleware allows us to write it once.

This follows the **Don't Repeat Yourself (DRY)** principle.

---

# Middleware vs Services

Many developers confuse middleware with services.

| Middleware                      | Service                                 |
| ------------------------------- | --------------------------------------- |
| Processes every HTTP request.   | Performs business logic.                |
| Runs before controllers.        | Called by controllers.                  |
| Handles cross-cutting concerns. | Handles application-specific behaviour. |
| Uses `HttpContext`.             | Should not depend on HTTP.              |

Examples of middleware:

* Logging
* Authentication
* Exception handling

Examples of services:

* BankService
* PaymentService
* CustomerService

---

# Middleware vs Filters

ASP.NET Core also supports Filters.

A simplified comparison:

| Middleware                              | Filters                                        |
| --------------------------------------- | ---------------------------------------------- |
| Runs before routing or after responses. | Runs around controller actions only.           |
| Applies to all requests.                | Applies only to MVC or API actions.            |
| Can short-circuit before controllers.   | Cannot replace middleware for global concerns. |

For cross-cutting infrastructure concerns, middleware is usually the better choice.

---

# Future Middleware

As this project grows, we will introduce additional middleware such as:

* Authentication
* Authorization
* Request logging
* Performance timing
* Rate limiting
* Request validation
* Distributed tracing
* Security headers

Each middleware will have a single responsibility.

---

# Best Practices

* Keep middleware focused on one concern.
* Register middleware in the correct order.
* Avoid placing business logic inside middleware.
* Use `HttpContext.Items` for request-scoped data.
* Use `OnStarting()` when modifying response headers.
* Let middleware handle global concerns instead of controllers.

---

# Common Mistakes

* Forgetting to call `await _next(context)`.
* Registering middleware in the wrong order.
* Modifying response headers after the response has started.
* Using middleware for business logic.
* Storing request-specific data in Singleton services.

---

# Interview Questions

1. What is middleware?
2. Why does every request pass through middleware?
3. What is `RequestDelegate`?
4. What happens if `_next(context)` is never called?
5. Why does middleware execute both before and after controllers?
6. What is `HttpContext.Items` used for?
7. Why did we create `HttpContextKeys`?
8. Why did we need `Response.OnStarting()`?
9. Why is middleware better than duplicating logic in controllers?
10. Why does middleware order matter?

---

# Key Takeaways

* Middleware is a core building block of ASP.NET Core.
* Every request and response passes through the middleware pipeline.
* Middleware is designed for cross-cutting concerns rather than business logic.
* `RequestDelegate` forwards execution to the next component.
* `HttpContext` carries request-specific information throughout the pipeline.
* `HttpContext.Items` allows middleware and controllers to share request-scoped data.
* `Response.OnStarting()` safely modifies response headers before they are sent.
* Proper middleware ordering is essential for correct application behaviour.
* Our `CorrelationMiddleware` and `ExceptionMiddleware` demonstrate how enterprise applications centralise shared functionality.

---

# Next Chapter

**08 – HTTP Client & External API Communication**

Before we begin implementing real bank integrations, we will step back and study the HTTP protocol itself. We'll cover HTTP methods, status codes, headers, request and response bodies, REST principles, idempotency, and API versioning. Understanding these concepts will help explain why our controllers, contracts, and endpoints are designed the way they are.


