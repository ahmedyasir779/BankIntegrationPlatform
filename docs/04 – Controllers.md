# 04 – Controllers

---

# Overview

Controllers are the entry point into our application's business functionality.

Every HTTP request that reaches our API is eventually handled by a controller. However, contrary to what many new developers believe, a controller should **not** contain the application's business logic.

Its primary responsibility is to act as a coordinator between the HTTP world (requests and responses) and the Application layer (business logic).

Keeping controllers small and focused is one of the key principles of building maintainable ASP.NET Core applications.

---

# What is a Controller?

A controller is a C# class that exposes one or more HTTP endpoints.

When ASP.NET Core's routing system matches an incoming URL, it invokes the appropriate controller action.

For example:

```text
POST /api/v1/balance
```

is routed to:

```text
BalanceController

↓

GetBalance()
```

The controller becomes the bridge between the client and the rest of the application.

---

# Where Controllers Fit in the Architecture

Controllers belong to the presentation layer.

They communicate with:

* HTTP clients
* ASP.NET Core
* Application services

They should **not** communicate directly with:

* Databases
* External banking APIs
* SQL Server
* File systems
* Business rules

Our architecture looks like this:

```text
Client

↓

Controller

↓

Application Service

↓

Infrastructure

↓

External Bank
```

Notice that the controller never communicates with the bank directly.

---

# Responsibilities of a Controller

A controller has a small number of responsibilities.

It should:

* Receive HTTP requests.
* Read route values.
* Read query parameters.
* Read request bodies.
* Validate incoming data (using Model Validation).
* Call the appropriate service.
* Return an HTTP response.

Everything else belongs somewhere else.

---

# Responsibilities the Controller Should Avoid

A controller should **never**:

* Contain business rules.
* Select which bank adapter to use.
* Open database connections.
* Execute SQL queries.
* Perform calculations.
* Authenticate with external systems.
* Catch every exception manually.
* Write logs directly.
* Build complex workflows.

If a controller starts becoming large, it is usually a sign that responsibilities should be moved into services or middleware.

---

# Our Current Controller

Our project currently contains a single controller.

```text
Controllers
└── BalanceController.cs
```

It exposes two endpoints:

```text
POST /api/v1/balance

GET /api/v1/balance/config
```

Each endpoint has a single responsibility.

---

# Controller Attributes

Our controller begins with:

```csharp
[ApiController]
[Route("api/v1/balance")]
public class BalanceController : ControllerBase
```

Each attribute has a purpose.

---

## `[ApiController]`

This tells ASP.NET Core that the class is an API controller.

It enables several useful features automatically, including:

* Automatic model validation.
* Automatic HTTP 400 responses for invalid models.
* Improved parameter binding.
* Better API conventions.

Almost every REST API controller should use this attribute.

---

## `[Route]`

The `Route` attribute defines the base URL for the controller.

Example:

```csharp
[Route("api/v1/balance")]
```

means every endpoint inside the controller starts with:

```text
/api/v1/balance
```

Individual actions can extend this route.

Example:

```csharp
[HttpGet("config")]
```

becomes:

```text
/api/v1/balance/config
```

---

## Why Inherit from `ControllerBase`?

Our controller inherits from:

```csharp
ControllerBase
```

instead of:

```csharp
Controller
```

`ControllerBase` is designed specifically for APIs.

It includes helper methods such as:

* `Ok()`
* `BadRequest()`
* `NotFound()`
* `Created()`
* `NoContent()`

The full `Controller` class includes MVC features such as Razor Views, which our REST API does not need.

---

# Constructor Injection

Our controller receives its dependencies through its constructor.

Example:

```csharp
public BalanceController(
    IBankService bankService,
    IOptions<BankOptions> option)
```

ASP.NET Core automatically creates these objects using Dependency Injection.

This approach has several advantages:

* Loose coupling.
* Easier testing.
* Easier maintenance.
* Clear dependencies.

The controller does not create its own objects using `new`.

---

# Why We Inject `IBankService`

Instead of writing:

```csharp
var service = new BankService();
```

we request:

```csharp
IBankService
```

This follows the **Dependency Inversion Principle (DIP)**.

The controller depends on an abstraction rather than a concrete implementation.

If the implementation changes in the future, the controller remains unchanged.

---

# Why We Inject `IOptions<BankOptions>`

Our configuration is stored inside `appsettings.json`.

Rather than reading configuration manually, ASP.NET Core binds it to a strongly typed object.

```csharp
IOptions<BankOptions>
```

gives us access to:

```csharp
_bankOptions
```

without hardcoding configuration values.

---

# Action Methods

Each public method decorated with an HTTP attribute becomes an endpoint.

Example:

```csharp
[HttpPost]
public async Task<ActionResult<ApiResponse<BalanceResponse>>> GetBalance(...)
```

This method is called an **Action**.

Each action should perform one task only.

---

# Why Our Actions Are Asynchronous

Our actions return:

```csharp
Task<ActionResult<ApiResponse<BalanceResponse>>>
```

instead of:

```csharp
ActionResult<ApiResponse<BalanceResponse>>
```

Using asynchronous methods allows ASP.NET Core to process other requests while waiting for I/O operations such as:

* Database queries.
* HTTP calls.
* File operations.

Although our adapters currently return mock data, they will later perform HTTP requests to real banking systems.

By writing asynchronous code now, our design is already prepared for that transition.

---

# IActionResult vs ActionResult<T>

There are several ways to return responses from a controller.

### IActionResult

```csharp
public IActionResult Get()
```

Use this when the action may return different types of responses and there is no single response model.

Example:

* `Ok()`
* `NotFound()`
* `BadRequest()`
* `NoContent()`

---

### ActionResult<T>

```csharp
public ActionResult<BalanceResponse> Get()
```

Use this when the action normally returns a specific model but may also return HTTP error responses.

This provides better Swagger documentation and stronger typing.

---

### Task<ActionResult<T>>

```csharp
public async Task<ActionResult<ApiResponse<BalanceResponse>>> GetBalance(...)
```

This combines:

* Asynchronous execution.
* Strong typing.
* Flexible HTTP responses.

It is the preferred approach for most modern REST APIs.

---

# Building a Standard Response

Our controller wraps every successful result inside:

```text
ApiResponse<T>
```

Example:

```text
ApiResponse
│
├── Header
│   ├── MessageId
│   ├── CorrelationId
│   ├── TimestampUtc
│   └── Status
│
└── Data
```

This provides a consistent contract across every endpoint.

Clients always know where to find:

* Metadata.
* Status information.
* Business data.

---

# Correlation ID in the Controller

Originally, the controller generated a new Correlation ID.

```csharp
CorrelationId = Guid.NewGuid()
```

We later improved this design.

The Correlation ID is now created once inside `CorrelationMiddleware`.

The controller simply reads the existing value from `HttpContext.Items`.

This ensures the same identifier is used throughout the entire request lifecycle.

---

# Thin Controllers

Our goal is to keep controllers extremely small.

A controller should answer one question:

> "Who should perform this work?"

It should never answer:

> "How should this work be performed?"

That responsibility belongs to services.

---

# Example Request Flow

Our current balance request follows this path:

```text
Client

↓

BalanceController

↓

IBankService

↓

BankService

↓

AdapterRegistry

↓

SNBAdapter

↓

ApiResponse

↓

Client
```

Notice that the controller simply coordinates the process.

---

# Best Practices

* Keep controllers thin.
* Inject dependencies through constructors.
* Depend on interfaces, not implementations.
* Return consistent response models.
* Use asynchronous actions.
* Avoid business logic inside controllers.
* Let middleware handle cross-cutting concerns.
* Keep each action focused on a single responsibility.

---

# Common Mistakes

* Writing SQL inside controllers.
* Calling external APIs directly.
* Creating services with `new`.
* Catching exceptions in every action.
* Returning different response formats for different endpoints.
* Adding business calculations to controller methods.

---

# Interview Questions

1. What is the responsibility of a controller?
2. Why should controllers remain thin?
3. What is the purpose of the `[ApiController]` attribute?
4. Why do API controllers inherit from `ControllerBase`?
5. What is constructor injection?
6. Why do we inject `IBankService` instead of `BankService`?
7. What is the difference between `IActionResult` and `ActionResult<T>`?
8. Why should controller actions be asynchronous?
9. Why do we use `ApiResponse<T>`?
10. Why should the Correlation ID come from middleware rather than the controller?

---

# Key Takeaways

* Controllers are the entry point for HTTP requests.
* Their role is to coordinate, not implement business logic.
* Dependencies should be injected rather than created manually.
* Actions should be asynchronous and strongly typed.
* Consistent response contracts simplify client integration.
* Middleware handles shared concerns, allowing controllers to remain clean and focused.
* Thin controllers are easier to test, maintain, and extend.

---

# Next Chapter

**05 – Dependency Injection & Services**

The next chapter explains one of the most important concepts in modern ASP.NET Core development: Dependency Injection. We will examine service lifetimes, interfaces, inversion of control, and how our `BankService` fits into the overall architecture.
