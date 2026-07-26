# 01 – Project Structure

---

# Overview

A well-organised project structure is one of the foundations of maintainable software.

As applications grow, poor organisation quickly leads to tightly coupled code, duplicated logic, and difficulty introducing new features.

From the beginning of this project, we intentionally organised the solution into logical areas instead of placing all files in a single folder. Although some folders are currently empty, they were created early to establish a scalable architecture that can support future enterprise requirements.

This project follows a simplified version of **Clean Architecture**, allowing the application to evolve without major restructuring.

---

# Current Solution Structure

```text
BankIntegrationPlatform
│
├── Application
│
├── Controllers
│
├── Domain
│
├── Infrastructure
│
├── Middleware
│
├── Common
│
├── Configuration
│
├── Extensions
│
├── Contracts
│
├── Properties
│
├── appsettings.json
│
├── Program.cs
│
└── BankIntegration.Api.csproj
```

Each folder has a specific responsibility.

Understanding these responsibilities is more important than memorising where files are stored.

---

# Why We Created the Folder Structure Early

One common mistake in new projects is creating folders only when they become necessary.

Although this works for small applications, it often results in large refactoring efforts as the project grows.

Instead, this project establishes the intended architecture from the beginning.

Benefits include:

* Consistent organisation.
* Easier navigation.
* Clear separation of responsibilities.
* Reduced coupling between components.
* Easier onboarding for new developers.
* Simpler migration to microservices.

---

# Controllers

## Purpose

Controllers expose the application's public HTTP endpoints.

They receive incoming HTTP requests, validate input (when applicable), call the appropriate application service, and return an HTTP response.

Controllers should remain as thin as possible.

---

## Responsibilities

Controllers should:

* Receive HTTP requests.
* Read route parameters.
* Read request bodies.
* Call application services.
* Return HTTP responses.
* Return appropriate HTTP status codes.

---

## Controllers Should NOT

Controllers should not:

* Contain business logic.
* Access databases directly.
* Call external banking systems.
* Perform calculations.
* Decide which bank adapter to use.

All business decisions belong elsewhere.

---

## Our Current Controller

Current implementation:

```text
BalanceController
```

Responsibilities:

* Receives balance requests.
* Calls `IBankService`.
* Returns an `ApiResponse<BalanceResponse>`.

The controller has no knowledge of how individual banks are implemented.

---

# Application

## Purpose

The Application layer contains the application's business behaviour.

It orchestrates operations but does not know implementation details such as databases or HTTP communication.

---

## Current Contents

Current folders include:

* Interfaces
* Services
* DTOs
* Common

---

## Services

Services contain business workflows.

Example:

```text
BankService
```

Responsibilities:

* Receive requests from controllers.
* Select the correct bank adapter.
* Coordinate the overall operation.
* Return business results.

---

## Interfaces

Interfaces define contracts.

Example:

```text
IBankService

IBankAdapter
```

The rest of the application depends on these abstractions rather than concrete implementations.

This follows the **Dependency Inversion Principle (DIP)**.

---

## DTOs

DTO (Data Transfer Object) stands for **Data Transfer Object**.

DTOs are used to transfer data between application layers without exposing internal implementation details.

Although currently empty, this folder will later contain request and response models used specifically inside the application layer.

---

## Common

This folder stores application-wide objects that do not belong to a single feature.

Examples include:

* RequestContext
* Shared constants
* Shared helper types

These objects are reused throughout the application.

---

# Domain

## Purpose

The Domain layer represents the core business model.

It contains the business entities and message contracts shared across the application.

The Domain should remain independent of ASP.NET Core, databases, and external services whenever possible.

---

## Current Structure

Current folders include:

* Models
* Messages
* Enums
* Exceptions

---

## Models

Models represent business data.

Examples:

* BalanceRequest
* BalanceResponse

These classes describe the information exchanged during business operations.

---

## Messages

Messages represent communication contracts.

Examples:

* ApiResponse<T>
* ResponseHeader
* RequestHeader
* ResponseStatus

These classes standardise communication between systems.

Separating them from business models makes versioning and future API evolution easier.

---

## Enums

Enums store fixed sets of values.

Using enums instead of strings reduces errors and improves readability.

Although currently empty, this folder is reserved for future domain enumerations.

---

## Exceptions

Contains custom exception types related to business behaviour.

Using dedicated exception classes provides more meaningful error handling than throwing generic exceptions.

---

# Infrastructure

## Purpose

Infrastructure contains technical implementations.

This layer is responsible for communicating with external systems and providing concrete implementations for abstractions defined elsewhere.

---

## Current Structure

Current folders include:

* Configurations
* External
* Logging
* Persistence
* Repositories

---

## Configurations

Stores strongly typed configuration classes.

Examples:

* BankOptions
* BankConfiguration

These classes map configuration values from `appsettings.json`.

---

## External

Contains integrations with external systems.

Current implementation:

* Bank adapters.
* AdapterRegistry.

Each adapter knows how to communicate with one specific bank.

The rest of the application remains unaware of bank-specific details.

---

## Logging

Reserved for future logging implementations.

Later this folder may contain:

* Console logging.
* File logging.
* Serilog.
* SQL logging.
* ElasticSearch integration.

---

## Persistence

Reserved for database-related components.

Future examples:

* Entity Framework DbContext.
* Database migrations.
* SQL Server configuration.

---

## Repositories

Repositories encapsulate data access.

Controllers and services should never query databases directly.

Instead, they communicate with repository interfaces.

---

# Middleware

## Purpose

Middleware executes before and after requests travel through the ASP.NET Core pipeline.

Middleware provides cross-cutting functionality shared by every endpoint.

---

## Current Middleware

Current middleware includes:

* CorrelationMiddleware
* ExceptionMiddleware

These execute for every incoming request.

---

## Why Middleware Exists

Without middleware, every controller would need to duplicate logic such as:

* Logging.
* Exception handling.
* Correlation IDs.
* Authentication.
* Authorisation.

Middleware centralises these concerns.

---

# Common

## Purpose

Contains shared objects used by multiple areas of the application.

Examples include:

* HttpContextKeys
* RequestContext

These objects support infrastructure rather than business logic.

---

# Configuration

Reserved for future application configuration components.

As the platform grows, configuration classes and startup extensions may be organised here.

---

# Extensions

Extension methods improve readability and keep Program.cs clean.

Future examples include:

* Service registration.
* Middleware registration.
* Swagger configuration.

Rather than placing large amounts of configuration inside `Program.cs`, extension methods allow related configuration to be grouped logically.

---

# Contracts

Reserved for shared interfaces and communication contracts that may eventually be reused by multiple projects.

Although currently empty, the folder exists because enterprise projects frequently separate contracts from implementations.

---

# Program.cs

## Purpose

`Program.cs` is the application's entry point.

Responsibilities include:

* Creating the web application.
* Registering services.
* Registering middleware.
* Configuring Swagger.
* Mapping controllers.
* Starting Kestrel.

It is responsible for composing the application rather than implementing business logic.

---

# appsettings.json

## Purpose

Stores application configuration.

Examples:

* Bank URLs.
* Authentication methods.
* Timeouts.
* Logging settings.
* Connection strings.

Keeping configuration outside the source code allows behaviour to change without recompilation.

---

# Why This Structure Supports Future Growth

Although the current project exposes only a single balance endpoint, the architecture has been designed with future expansion in mind.

Planned additions include:

* Identity Service.
* B2B API.
* Business Logic Service.
* Integration Service.
* Gateway Service.
* SQL Server.
* Distributed logging.
* Authentication.
* External HTTP integrations.

Because responsibilities are already separated, these features can be introduced without major restructuring.

---

# Key Takeaways

* Every folder has a single responsibility.
* Controllers should remain thin.
* Business logic belongs in the Application layer.
* Business models belong in the Domain layer.
* Technical implementations belong in Infrastructure.
* Middleware handles cross-cutting concerns.
* Configuration should never be hardcoded.
* A clear project structure makes future growth significantly easier.

---

# Next Chapter

The next document explains the foundation of the entire project:

**02 – ASP.NET Core Fundamentals**

Before understanding controllers, middleware, or dependency injection, it is essential to understand what ASP.NET Core is, how it hosts applications, and how an HTTP request reaches our code.
