# 03 – HTTP Request Pipeline

---

# Overview

Every request sent to an ASP.NET Core application follows a predictable journey before a response is returned to the client.

Understanding this journey is one of the most important skills for an ASP.NET Core developer because every feature we build—controllers, middleware, dependency injection, authentication, logging, exception handling, and even database access—depends on this pipeline.

Throughout this chapter we will follow a single request as it travels through our Bank Integration Platform.

---

# What is an HTTP Request Pipeline?

An HTTP Request Pipeline is the sequence of components that process an incoming HTTP request.

Each component receives the request, performs some work, and then decides whether to:

* Continue processing the request.
* Modify the request.
* Modify the response.
* Stop processing entirely.

This design makes ASP.NET Core highly modular and extensible.

---

# Our Current Pipeline

As of Day 3, our application processes requests in the following order.

```text
                Client
                   │
                   ▼
          HTTP Request
                   │
                   ▼
             Kestrel Server
                   │
                   ▼
      CorrelationMiddleware
                   │
                   ▼
       ExceptionMiddleware
                   │
                   ▼
               Routing
                   │
                   ▼
        BalanceController
                   │
                   ▼
            IBankService
                   │
                   ▼
             BankService
                   │
                   ▼
          AdapterRegistry
                   │
                   ▼
              IBankAdapter
                   │
                   ▼
             SNBAdapter
                   │
                   ▼
         BalanceResponse
                   │
                   ▼
 ApiResponse<BalanceResponse>
                   │
                   ▼
          HTTP Response
                   │
                   ▼
                Client
```

Every successful request currently follows this exact path.

---

# Step 1 – Client Sends a Request

Everything begins with a client.

The client could be:

* Swagger UI
* Postman
* A mobile application
* A banking system
* Another microservice
* A web application

Example:

```http
POST /api/v1/balance
```

with the request body:

```json
{
    "accountNumber": "123456789",
    "bankCode": "SNB"
}
```

The request is sent to our API.

---

# Step 2 – Kestrel Receives the Request

The first component inside ASP.NET Core is Kestrel.

Kestrel:

* Listens on the configured port.
* Accepts the TCP connection.
* Parses the HTTP request.
* Creates an `HttpContext`.
* Starts the middleware pipeline.

At this point none of our own code has executed yet.

---

# Step 3 – Middleware Begins

The request now enters our middleware pipeline.

Current order:

```text
CorrelationMiddleware

↓

ExceptionMiddleware
```

Every request passes through these middleware components before reaching a controller.

---

# CorrelationMiddleware

Responsibilities:

* Check whether the client supplied an `X-Correlation-Id`.
* Generate one if it does not exist.
* Store it in `HttpContext.Items`.
* Return the same ID in the response header.

This allows every log message and every service involved in the request to be linked together.

---

# ExceptionMiddleware

This middleware wraps the rest of the pipeline inside a `try...catch`.

If an exception occurs anywhere downstream, it:

* Catches the exception.
* Prevents the application from crashing.
* Creates a standard API error response.
* Includes the same Correlation ID.
* Returns HTTP 500.

Without this middleware every controller would need repetitive exception handling.

---

# Step 4 – Routing

If middleware allows the request to continue:

```csharp
await _next(context);
```

ASP.NET Core reaches the routing system.

Routing determines which endpoint matches the incoming URL.

Example:

```text
POST /api/v1/balance
```

matches:

```text
BalanceController

↓

GetBalance()
```

---

# Step 5 – Model Binding

ASP.NET Core automatically converts JSON into C# objects.

Incoming JSON:

```json
{
    "accountNumber":"123456789",
    "bankCode":"SNB"
}
```

becomes:

```csharp
BalanceRequest request
```

This process is called **Model Binding**.

No manual parsing is required.

---

# Step 6 – Controller Execution

Our controller now executes.

Responsibilities:

* Receive the request.
* Call the service.
* Return an HTTP response.

Notice what the controller does **not** do:

* No database access.
* No adapter selection.
* No business rules.
* No exception handling.
* No logging.

Controllers should coordinate, not implement business logic.

---

# Step 7 – BankService

The controller delegates the work to:

```text
IBankService

↓

BankService
```

Responsibilities:

* Receive business requests.
* Coordinate the workflow.
* Ask the Adapter Registry for the correct adapter.
* Return the result.

This keeps controllers small and focused.

---

# Step 8 – Adapter Registry

The registry receives the bank code.

Example:

```text
SNB
```

It searches its dictionary:

```text
Dictionary<string, IBankAdapter>
```

and returns:

```text
SNBAdapter
```

Notice that no `switch` statement is required.

Adding a new bank becomes much easier.

---

# Step 9 – Bank Adapter

Each bank has its own adapter.

Current adapters:

* SNBAdapter
* RiyadAdapter
* AlRajhiAdapter
* MockBankAdapter

Currently these adapters return mock data.

Later they will:

* Call external APIs.
* Authenticate.
* Parse responses.
* Handle certificates.
* Transform data.

The rest of the application will remain unchanged.

---

# Step 10 – Response Creation

The adapter returns:

```text
BalanceResponse
```

The controller wraps it inside:

```text
ApiResponse<T>
```

containing:

* ResponseHeader
* ResponseStatus
* Data

Every endpoint will eventually follow this standard response format.

---

# Step 11 – Returning Through the Pipeline

After the controller finishes, execution travels backwards through the middleware pipeline.

```text
Controller

↑

ExceptionMiddleware

↑

CorrelationMiddleware

↑

Kestrel

↑

Client
```

This is why middleware can execute code **after** the controller has finished.

One example is:

```csharp
context.Response.OnStarting(...)
```

which allows us to add response headers immediately before they are sent.

---

# Understanding `await _next(context)`

This is one of the most important concepts in ASP.NET Core.

When middleware executes:

```csharp
await _next(context);
```

it temporarily hands control to the next component.

When that component finishes, execution resumes immediately after the `await`.

This behaviour makes middleware act like a stack rather than a simple sequence.

---

# Visualising the Request

Think of the request like a parcel travelling through a warehouse.

Each department:

* inspects it,
* may attach information,
* may reject it,
* or passes it to the next department.

Eventually the parcel reaches its destination.

The completed parcel then travels back through every department before leaving the warehouse.

Middleware behaves in exactly the same way.

---

# Why This Pipeline Matters

Every feature we build later will fit into this pipeline.

Examples include:

* Authentication
* Authorisation
* Logging
* Metrics
* Rate limiting
* Caching
* Compression
* Monitoring
* Distributed tracing

Understanding the request pipeline makes these features much easier to reason about.

---

# Common Mistakes

* Putting business logic inside middleware.
* Accessing databases directly from controllers.
* Using middleware when a service is more appropriate.
* Assuming middleware only runs before the controller.
* Ignoring the response path after `await _next(context)`.

---

# Best Practices

* Keep middleware focused on one responsibility.
* Keep controllers thin.
* Delegate business logic to services.
* Return standard response contracts.
* Use middleware for cross-cutting concerns.

---

# Interview Questions

1. What is the ASP.NET Core request pipeline?
2. Why does every request pass through middleware?
3. What does `await _next(context)` do?
4. Why does execution continue after `await`?
5. What is model binding?
6. Why shouldn't controllers contain business logic?
7. What is the responsibility of the Adapter Registry?
8. At which point is the HTTP response created?
9. Why do we use `Response.OnStarting()`?
10. How would authentication fit into this pipeline?

---

# Key Takeaways

* Every request follows a predictable sequence of components.
* Middleware executes both before and after downstream components.
* Controllers coordinate requests rather than implementing business logic.
* Services contain application behaviour.
* Adapters isolate bank-specific implementations.
* The response travels back through the middleware pipeline before reaching the client.
* Understanding this pipeline is essential for building enterprise-grade ASP.NET Core applications.

---

# Next Chapter

**04 – Controllers**

The next chapter explains why controllers exist, what responsibilities they should have, what they should avoid, and how our `BalanceController` is designed to remain thin and maintainable.
