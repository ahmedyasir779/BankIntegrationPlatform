# 05 – Dependency Injection & Services

---

# Overview

Dependency Injection (DI) is one of the core design principles of ASP.NET Core and one of the most frequently discussed topics in software engineering interviews.

Although it may seem like a framework feature, Dependency Injection is actually a software design pattern that promotes **loose coupling**, improves maintainability, and makes applications easier to test.

In this chapter we will learn what Dependency Injection is, why we use it, how ASP.NET Core implements it, and how it is used throughout our Bank Integration Platform.

---

# What is Dependency Injection?

Dependency Injection is a technique where an object receives the components it depends on from an external source rather than creating them itself.

Instead of this:

```csharp
public class BalanceController : ControllerBase
{
    private readonly BankService _bankService = new BankService();
}
```

we do this:

```csharp
public class BalanceController : ControllerBase
{
    private readonly IBankService _bankService;

    public BalanceController(IBankService bankService)
    {
        _bankService = bankService;
    }
}
```

The controller does not create a `BankService`.

Instead, ASP.NET Core provides one automatically.

---

# What is a Dependency?

A dependency is simply another object that a class needs to perform its job.

For example:

```text
BalanceController

↓

IBankService

↓

BankService
```

`BalanceController` depends on `IBankService`.

Similarly:

```text
BankService

↓

AdapterRegistry
```

`BankService` depends on `AdapterRegistry`.

Every class in a well-designed application has dependencies.

Dependency Injection is simply the mechanism used to provide them.

---

# What Problem Does Dependency Injection Solve?

Imagine writing:

```csharp
var service = new BankService();
```

inside every controller.

Problems quickly appear:

* The controller is tightly coupled to one implementation.
* Replacing the implementation becomes difficult.
* Unit testing becomes harder.
* Every class becomes responsible for creating its own dependencies.

As applications grow, this approach becomes difficult to maintain.

Dependency Injection solves these problems by moving object creation into a central container.

---

# Inversion of Control (IoC)

Dependency Injection is one way of implementing a broader principle known as **Inversion of Control (IoC)**.

Normally a class controls the creation of its dependencies.

With IoC, that responsibility is inverted.

Instead of the class creating its own dependencies:

```text
Controller

↓

new BankService()
```

the framework creates them:

```text
Controller

↑

ASP.NET Core DI Container

↓

BankService
```

The controller focuses only on its own responsibility.

---

# The Dependency Injection Container

ASP.NET Core includes a built-in Dependency Injection container.

This container is responsible for:

* Creating objects.
* Managing object lifetimes.
* Resolving dependencies.
* Injecting dependencies into constructors.

We configure the container in `Program.cs`.

Example:

```csharp
builder.Services.AddScoped<IBankService, BankService>();
```

This tells ASP.NET Core:

> Whenever a class requests an `IBankService`, create and inject a `BankService`.

---

# Service Registration

Every service must be registered before ASP.NET Core can inject it.

Our project currently registers:

```csharp
builder.Services.AddScoped<IBankService, BankService>();

builder.Services.AddScoped<AdapterRegistry>();

builder.Services.AddScoped<IBankAdapter, SNBAdapter>();
builder.Services.AddScoped<IBankAdapter, RiyadAdapter>();
builder.Services.AddScoped<IBankAdapter, AlRajhiAdapter>();
builder.Services.AddScoped<IBankAdapter, MockBankAdapter>();
```

Without registration, ASP.NET Core does not know how to create the object.

---

# Constructor Injection

ASP.NET Core supports several forms of injection, but constructor injection is the preferred approach.

Example:

```csharp
public BalanceController(
    IBankService bankService,
    IOptions<BankOptions> options)
{
    _bankService = bankService;
    _bankOptions = options.Value;
}
```

When the controller is created, ASP.NET Core automatically resolves each dependency.

The controller never needs to call `new`.

---

# Dependency Graph

When a request reaches our controller, ASP.NET Core resolves the complete dependency graph.

```text
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
IEnumerable<IBankAdapter>
        │
        ├── SNBAdapter
        ├── RiyadAdapter
        ├── AlRajhiAdapter
        └── MockBankAdapter
```

The framework builds this graph automatically.

---

# Why We Use Interfaces

Notice that our controller depends on:

```csharp
IBankService
```

rather than:

```csharp
BankService
```

This is deliberate.

Interfaces provide an abstraction.

The controller only knows:

> "I need something capable of performing banking operations."

It does not need to know the implementation.

This follows the **Dependency Inversion Principle (DIP)**.

---

# IBankService

Our service contract defines the application's business capability.

```csharp
public interface IBankService
{
    Task<BalanceResponse> GetBalanceAsync(BalanceRequest request);
}
```

Every implementation must satisfy this contract.

---

# BankService

`BankService` contains the application's workflow.

Current responsibilities include:

* Receiving requests from controllers.
* Selecting the correct adapter.
* Delegating work.
* Returning results.

Notice what it does **not** do:

* Parse HTTP requests.
* Return HTTP responses.
* Communicate directly with clients.

Those responsibilities belong elsewhere.

---

# Why Controllers Should Not Call Adapters Directly

Imagine this controller:

```text
BalanceController

↓

SNBAdapter
```

Now the controller knows how banks work.

If tomorrow we add:

* QNB
* Emirates NBD
* HSBC
* Barclays

the controller becomes increasingly complex.

Instead, our architecture is:

```text
BalanceController

↓

IBankService

↓

AdapterRegistry

↓

IBankAdapter
```

The controller remains unchanged regardless of how many banks are added.

---

# Service Lifetimes

Every registered service has a lifetime.

ASP.NET Core provides three built-in lifetimes.

---

## Singleton

```csharp
builder.Services.AddSingleton<T>();
```

One instance is created for the entire lifetime of the application.

Every request receives the same object.

```text
Application Starts

↓

Create Object

↓

Reuse Forever
```

Suitable for:

* Configuration providers.
* Caches.
* Stateless utilities.
* Shared application services.

Avoid storing request-specific information in Singleton services.

---

## Scoped

```csharp
builder.Services.AddScoped<T>();
```

One instance is created per HTTP request.

```text
Request 1

↓

Create Object

↓

Dispose


Request 2

↓

Create New Object
```

This is the lifetime we currently use.

Suitable for:

* Business services.
* Repositories.
* Entity Framework DbContext.
* Adapter Registry.

Each request receives its own instance.

---

## Transient

```csharp
builder.Services.AddTransient<T>();
```

A new instance is created every time it is requested.

```text
Need Object

↓

Create New Object

↓

Discard
```

Suitable for:

* Lightweight stateless services.
* Small helper components.

---

# Which Lifetime Should We Use?

For our current project:

| Service         | Lifetime                             | Reason                                                            |
| --------------- | ------------------------------------ | ----------------------------------------------------------------- |
| BankService     | Scoped                               | One business service per request.                                 |
| AdapterRegistry | Scoped                               | Built for each request using registered adapters.                 |
| Bank Adapters   | Scoped                               | Future adapters will make HTTP calls using request-specific data. |
| Logger          | Singleton (framework-managed)        | Shared logging infrastructure.                                    |
| HttpClient      | Managed through `IHttpClientFactory` | Prevents socket exhaustion and manages connections efficiently.   |

---

# Why BankService is Scoped

Every request has its own:

* Correlation ID.
* Request context.
* Business operation.

A scoped lifetime ensures request-specific information is never shared between users.

---

# Options Pattern and Dependency Injection

Configuration also uses Dependency Injection.

Instead of reading:

```json
appsettings.json
```

manually, we register:

```csharp
builder.Services.Configure<BankOptions>(
    builder.Configuration.GetSection("BankOptions"));
```

Later we inject:

```csharp
IOptions<BankOptions>
```

This keeps configuration strongly typed and testable.

---

# Dependency Injection in Our Project

The current flow is:

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

IBankAdapter

↓

SNBAdapter
```

Each component only knows about the layer directly beneath it.

This creates a loosely coupled architecture.

---

# Benefits of Dependency Injection

Dependency Injection provides many advantages.

* Loose coupling.
* Easier maintenance.
* Easier unit testing.
* Better separation of concerns.
* Improved readability.
* Easier replacement of implementations.
* Better scalability.

---

# Common Mistakes

* Creating services with `new`.
* Registering every service as Singleton.
* Depending on concrete implementations instead of interfaces.
* Mixing business logic with dependency registration.
* Registering services that are never used.

---

# Best Practices

* Prefer constructor injection.
* Depend on interfaces rather than implementations.
* Register services with appropriate lifetimes.
* Keep services focused on one responsibility.
* Allow the DI container to create objects.
* Avoid service locator patterns.

---

# Interview Questions

1. What is Dependency Injection?
2. What problem does Dependency Injection solve?
3. What is Inversion of Control?
4. What is the ASP.NET Core DI container?
5. Why do we inject interfaces instead of concrete classes?
6. What is the difference between Singleton, Scoped, and Transient?
7. Why is `BankService` registered as Scoped?
8. Why shouldn't controllers create services using `new`?
9. How does constructor injection work?
10. What are the advantages of loose coupling?

---

# Key Takeaways

* Dependency Injection separates object creation from object usage.
* ASP.NET Core includes a built-in DI container.
* Constructor injection is the preferred way to receive dependencies.
* Interfaces reduce coupling and improve flexibility.
* `BankService` coordinates business operations rather than handling HTTP concerns.
* Scoped services are ideal for request-based business logic.
* A well-designed dependency graph makes applications easier to extend and maintain.

---

# Next Chapter

**06 – Configuration & Options Pattern**

In the next chapter we will examine how ASP.NET Core manages configuration using `appsettings.json`, strongly typed options, and the Options Pattern. We will also explain why enterprise applications never hardcode URLs, credentials, or environment-specific settings.
