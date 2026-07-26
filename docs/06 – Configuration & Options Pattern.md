# 06 – Configuration & Options Pattern

---

# Overview

One of the first lessons every backend developer learns is:

> **Never hardcode values that may change.**

Things such as:

* API URLs
* Database connection strings
* Authentication methods
* Timeouts
* Certificates
* API Keys
* Feature flags

change between environments and over time.

ASP.NET Core solves this problem through its **Configuration System** and the **Options Pattern**, allowing applications to keep configuration separate from application logic.

In this chapter, we'll examine how configuration works in ASP.NET Core and how we've already applied these concepts in our Bank Integration Platform.

---

# What is Configuration?

Configuration is any value that controls how an application behaves **without requiring the code to be changed**.

Examples include:

* Which database to connect to.
* Which bank API URL to call.
* Whether Swagger is enabled.
* How long to wait before timing out.
* Which authentication method a bank uses.

These values are not business logic—they are operational settings.

---

# Why We Don't Hardcode Values

Imagine writing this inside an adapter:

```csharp
string baseUrl = "https://api.snb.com.sa";
```

Initially, it works.

But what happens when:

* The bank changes its URL?
* We move from Test to Production?
* A new developer needs a local environment?
* Another customer uses a different endpoint?

Now we must search through the codebase and rebuild the application.

Hardcoded values make software difficult to maintain and deploy.

---

# The Better Approach

Instead, we move configuration outside the code.

```text
Application Code

↓

Reads Configuration

↓

Configuration File
```

Now changing a URL requires updating configuration rather than modifying source code.

---

# appsettings.json

The primary configuration file in ASP.NET Core is:

```text
appsettings.json
```

This file contains application settings that ASP.NET Core loads automatically when the application starts.

Our project currently contains:

```json
{
  "BankOptions": {
    "SNB": {
      "BaseUrl": "https://api.snb.com.sa",
      "Timeout": 30,
      "Authentication": "OAuth"
    },
    "AlRajhi": {
      "BaseUrl": "https://api.alrajhibank.com.sa",
      "Timeout": 30,
      "Authentication": "Certificate"
    },
    "Riyad": {
      "BaseUrl": "https://api.riyadbank.com",
      "Timeout": 30,
      "Authentication": "JWT"
    }
  }
}
```

Notice that there is **no business logic** here.

Only configuration.

---

# Why JSON?

Microsoft chose JSON because it is:

* Easy to read.
* Easy to edit.
* Widely supported.
* Structured.
* Human-readable.

Nested configuration maps naturally to C# objects.

---

# Environment-Specific Configuration

Enterprise applications rarely have a single configuration file.

Typical structure:

```text
appsettings.json

appsettings.Development.json

appsettings.Test.json

appsettings.Staging.json

appsettings.Production.json
```

Each environment overrides only the values that differ.

Example:

Development:

```text
https://localhost:7001
```

Production:

```text
https://api.company.com
```

The application code remains identical.

Only the configuration changes.

---

# Strongly Typed Configuration

Instead of reading raw strings everywhere:

```csharp
builder.Configuration["BankOptions:SNB:BaseUrl"]
```

ASP.NET Core allows configuration to be mapped directly into C# classes.

This is called **Strongly Typed Configuration**.

---

# Our Configuration Classes

We created two classes.

## BankConfiguration

```csharp
public class BankConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;

    public int Timeout { get; set; }

    public string Authentication { get; set; } = string.Empty;
}
```

This represents a **single bank's configuration**.

---

## BankOptions

```csharp
public class BankOptions
{
    public Dictionary<string, BankConfiguration> Banks { get; set; } = [];
}
```

This represents the entire `BankOptions` section.

Each dictionary key is a bank code.

Example:

```text
Banks

├── SNB

├── Riyad

└── AlRajhi
```

---

# Why Use a Dictionary?

Originally we could have written:

```csharp
public class BankOptions
{
    public BankConfiguration SNB { get; set; }

    public BankConfiguration Riyad { get; set; }

    public BankConfiguration AlRajhi { get; set; }
}
```

But imagine adding:

* HSBC
* QNB
* FAB
* Emirates NBD

Every new bank would require modifying the class.

Using:

```csharp
Dictionary<string, BankConfiguration>
```

allows the application to support any number of banks without changing the model.

This follows the **Open/Closed Principle (OCP)**:

* **Open for extension**
* **Closed for modification**

---

# Registering Configuration

Configuration is registered in `Program.cs`.

```csharp
builder.Services.Configure<BankOptions>(
    builder.Configuration.GetSection("BankOptions"));
```

Let's break this down.

---

## builder.Configuration

Represents the application's configuration system.

It combines values from:

* appsettings.json
* appsettings.Environment.json
* Environment Variables
* Command Line Arguments
* User Secrets (development)
* Azure Key Vault (if configured)

Everything is merged into one configuration source.

---

## GetSection()

```csharp
builder.Configuration.GetSection("BankOptions")
```

Selects only the `BankOptions` section from the configuration.

Instead of loading the entire configuration tree, we bind only what we need.

---

## Configure<T>()

```csharp
Configure<BankOptions>()
```

Tells ASP.NET Core:

> "Create a `BankOptions` object and populate it using the values from this configuration section."

The framework performs the mapping automatically.

---

# The Options Pattern

The **Options Pattern** is the recommended way to access configuration in ASP.NET Core.

Rather than injecting configuration directly, we inject:

```csharp
IOptions<BankOptions>
```

This provides:

* Type safety.
* IntelliSense.
* Validation support.
* Easy unit testing.
* Loose coupling.

---

# Why Not Inject IConfiguration Everywhere?

We could inject:

```csharp
IConfiguration
```

and manually retrieve values.

```csharp
_configuration["BankOptions:SNB:BaseUrl"]
```

Problems:

* Magic strings.
* No compile-time checking.
* Easy to mistype keys.
* Harder to refactor.
* Difficult to validate.

Strongly typed options avoid these issues.

---

# Using IOptions

Our controller receives configuration like this:

```csharp
public BalanceController(
    IBankService bankService,
    IOptions<BankOptions> options)
{
    _bankService = bankService;
    _bankOptions = options.Value;
}
```

`options.Value` contains the populated `BankOptions` object.

No manual JSON parsing is required.

---

# Our Configuration Endpoint

To verify everything worked correctly, we created:

```text
GET /api/v1/balance/config
```

This endpoint returns the loaded configuration.

Example response:

```json
{
  "banks": {
    "SNB": {
      "baseUrl": "https://api.snb.com.sa",
      "timeout": 30,
      "authentication": "OAuth"
    }
  }
}
```

This confirmed that:

* The configuration file was loaded.
* The section was bound correctly.
* Dependency Injection was working.

---

# How We'll Use This Later

Right now, adapters return mock responses.

Later, each adapter will read its own configuration.

Example:

```text
SNBAdapter

↓

Read SNB Configuration

↓

BaseUrl

↓

Authentication

↓

Timeout

↓

Call Bank API
```

Notice that the adapter doesn't know where the configuration came from.

It simply receives it through Dependency Injection.

---

# Benefits of the Options Pattern

Using the Options Pattern provides several advantages:

* Strong typing.
* Centralised configuration.
* Cleaner code.
* Better IntelliSense.
* Easier testing.
* Reduced duplication.
* Easier validation.
* Environment flexibility.

---

# Common Mistakes

* Hardcoding URLs.
* Hardcoding API Keys.
* Injecting `IConfiguration` into every class.
* Repeating configuration lookups.
* Mixing business logic with configuration.

---

# Best Practices

* Store operational settings in `appsettings.json`.
* Group related settings into option classes.
* Register option classes in `Program.cs`.
* Inject `IOptions<T>` instead of `IConfiguration` where possible.
* Keep configuration separate from business logic.
* Use dictionaries when configuration items are dynamic.

---

# Interview Questions

1. What is application configuration?
2. Why shouldn't API URLs be hardcoded?
3. What is `appsettings.json`?
4. What is the Options Pattern?
5. What does `builder.Services.Configure<T>()` do?
6. Why is `IOptions<T>` preferred over `IConfiguration`?
7. Why did we use a `Dictionary<string, BankConfiguration>`?
8. How does strongly typed configuration improve maintainability?
9. What is the benefit of environment-specific configuration files?
10. How does our `SNBAdapter` benefit from the Options Pattern?

---

# Key Takeaways

* Configuration defines how an application behaves without changing its code.
* `appsettings.json` is ASP.NET Core's primary configuration source.
* Strongly typed configuration maps JSON directly to C# classes.
* The Options Pattern provides a clean, type-safe way to access configuration.
* Dependency Injection supplies configuration objects where they are needed.
* Using a dictionary allows our banking platform to support new banks without modifying the configuration model.
* Separating configuration from business logic makes the application easier to maintain, test, and deploy.

---

# Next Chapter

**07 – Middleware**

Now that we understand how requests enter the application and how services receive configuration, we are ready to explore **Middleware** in depth. We'll examine how the middleware pipeline works, why middleware is ideal for cross-cutting concerns, and how we implemented `CorrelationMiddleware` and `ExceptionMiddleware` in our project.
