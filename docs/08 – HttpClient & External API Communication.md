# HTTP Client & External API Communication

## Objective

Today we replaced our fake in-memory bank responses with real HTTP communication.

Instead of the `SNBAdapter` manually creating a `BalanceResponse`, it now communicates with a separate API (`BankMockServer.Api`) exactly like it would communicate with a real bank.

This is the first time our integration platform actually performs an external API call.

---

## Why HttpClient?

A bank integration platform rarely contains the business data itself.

Instead it communicates with external systems.

Examples include:

- Banks
- Government APIs
- Payment gateways
- Identity providers
- Fraud detection services

.NET provides **HttpClient** for sending HTTP requests efficiently.

Using HttpClientFactory also solves common problems like:

- Socket exhaustion
- DNS refresh issues
- Lifetime management
- Centralized configuration
- Logging support

---

## Overall Architecture

```
                    Client
                      │
                      ▼
        BankIntegration.Api
                      │
                      ▼
                BankService
                      │
                      ▼
              AdapterRegistry
                      │
                      ▼
                 SNBAdapter
                      │
                      ▼
              BankHttpClient
                      │
             HTTP POST Request
                      │
                      ▼
          BankMockServer.Api
                      │
                      ▼
             JSON Response
                      │
                      ▼
              BalanceResponse
```

---

## Why Use an Adapter?

Each bank speaks differently.

For example:

SNB

```
POST /balance
```

Al Rajhi

```
POST /accounts/balance
```

Riyad

```
POST /v2/customer/balance
```

Authentication may also differ.

- OAuth
- JWT
- Certificate
- API Key

The adapter hides all of these differences.

BankService never needs to know how an individual bank works.

---

## HttpClient Lifecycle

Instead of creating:

```csharp
new HttpClient()
```

for every request, ASP.NET Core uses **HttpClientFactory**.

Registration:

```csharp
builder.Services.AddHttpClient<
    IBankHttpClient,
    BankHttpClient>();
```

Whenever a BankHttpClient is requested through Dependency Injection, .NET automatically provides:

- managed HttpClient
- connection pooling
- DNS updates
- logging support

---

## Typed HttpClient

Our reusable HTTP client is:

```
IBankHttpClient
```

Implementation:

```
BankHttpClient
```

Current method:

```csharp
Task<TResponse> PostAsync<TRequest, TResponse>()
```

This allows any adapter to send requests without duplicating HTTP code.

Example:

```csharp
return await _httpClient.PostAsync<
    BalanceRequest,
    BalanceResponse>(
        url,
        request);
```

Tomorrow this same client can be reused for:

- Balance
- MT940
- Transactions
- Beneficiaries
- Payments

---

## Bank Configuration

Instead of hardcoding URLs inside adapters, each bank has its own configuration.

Example:

```json
"SNB": {
    "BaseUrl": "http://localhost:5200",
    "BalanceEndpoint": "/api/v1/balance",
    "Timeout": 30,
    "Authentication": "OAuth"
}
```

The adapter only reads the configuration.

```csharp
var url =
    $"{_configuration.BaseUrl}{_configuration.BalanceEndpoint}";
```

Changing environments now only requires changing configuration.

No source code changes.

---

## BankHttpClient Responsibilities

Our BankHttpClient is responsible for:

- Sending HTTP requests
- Serializing request objects
- Deserializing responses
- Logging requests
- Returning strongly typed models

It should **NOT** contain business logic.

---

## Adapter Responsibilities

Each adapter is responsible for:

- Selecting endpoints
- Authentication
- Request mapping
- Response mapping
- Bank-specific behavior

Example:

```csharp
public async Task<BalanceResponse> GetBalanceAsync(
    BalanceRequest request)
{
    var url =
        $"{_configuration.BaseUrl}{_configuration.BalanceEndpoint}";

    return await _httpClient.PostAsync<
        BalanceRequest,
        BalanceResponse>(
            url,
            request);
}
```

Notice there is almost no HTTP code.

Everything is delegated to BankHttpClient.

---

## Building the Mock Bank

Instead of calling a real bank, we built our own API.

Project:

```
BankMockServer.Api
```

Endpoint:

```
POST /api/v1/balance
```

Example request:

```json
{
    "accountNumber": "11222"
}
```

Example response:

```json
{
    "accountNumber": "11222",
    "balance": 9999.99,
    "currency": "SAR"
}
```

This simulates a real banking integration without requiring external access.

---

## Request Flow

```
Client

↓

BankIntegration.Api

↓

BankService

↓

AdapterRegistry

↓

SNBAdapter

↓

BankHttpClient

↓

HTTP POST

↓

BankMockServer.Api
```

---

## Response Flow

```
BankMockServer.Api

↓

JSON Response

↓

BankHttpClient

↓

BalanceResponse

↓

SNBAdapter

↓

BankService

↓

Controller

↓

Client
```

---

## Logging

Because HttpClientFactory is used, .NET automatically logs:

```
Start processing HTTP request

↓

Sending HTTP request

↓

Receiving response

↓

Status Code

↓

Request completed
```

We also added our own application logs.

Example:

```
Processing balance request

Selected adapter: SNBAdapter

POST http://localhost:5200/api/v1/balance

Balance request completed successfully
```

This separation between framework logs and application logs makes debugging much easier.

---

## Configuration

Current architecture:

```
appsettings.json

↓

BankOptions

↓

BankConfiguration

↓

SNBAdapter

↓

BankHttpClient
```

Future improvements include:

- Retry policies (Polly)
- Circuit breakers
- Per-bank timeout configuration
- Authentication handlers
- Client certificates

---

## Common Pitfalls

Avoid:

❌ Creating a new HttpClient for every request

❌ Hardcoding URLs

❌ Putting HTTP code inside controllers

❌ Mixing business logic with HTTP logic

❌ Copying request code into every adapter

Prefer:

✔ HttpClientFactory

✔ Typed HttpClient

✔ Configuration

✔ Adapter pattern

✔ Dependency Injection

---

## What We Built Today

✅ Replaced fake responses with real HTTP communication

✅ Introduced a reusable BankHttpClient

✅ Configured Typed HttpClient

✅ Built a standalone BankMockServer.Api

✅ Added Swagger to the mock server

✅ Connected SNBAdapter to the mock server

✅ Moved URLs into configuration

✅ Successfully completed an end-to-end HTTP call between two APIs

---

## Key Takeaways

Today marked the transition from a simple Web API to a true integration platform.

The architecture now resembles a real enterprise banking solution:

- Controllers no longer communicate directly with banks.
- Business logic is isolated in services.
- Bank-specific behavior is isolated in adapters.
- HTTP communication is centralized in a reusable client.
- External endpoints are configuration-driven.
- A mock banking API allows realistic integration testing without relying on real banking systems.

This foundation makes it straightforward to add additional banks, authentication mechanisms, resilience policies, and production-grade features in the following stages of the project.