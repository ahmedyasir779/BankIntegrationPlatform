# 10 – Authentication & JWT Identity Server

---

# Overview

Authentication is one of the most important components of any modern application.

Without authentication, an API has no way of knowing **who is making a request**.

In enterprise banking systems, every request must be verified before any business logic is executed.

Before a client can request:

- Balance Inquiry
- MT940 Statement
- Account Verification
- Payments
- Transfers

the platform must first answer one question:

> **Who are you?**

That is the responsibility of an **Identity Server**.

In this chapter we will build our own Identity Server using **ASP.NET Core**, **JWT (JSON Web Tokens)** and the **OAuth 2.0 Client Credentials Flow**, then integrate it with our Bank Integration Platform.

Although our implementation is simplified for learning purposes, it closely follows how authentication works in enterprise banking environments.

---

# Why Authentication Exists

Imagine exposing your Balance API publicly:

```text
POST /api/v1/balance
```

Anyone who discovers your endpoint could call:

```http
POST /api/v1/balance

{
    "bankCode": "SNB",
    "accountNumber": "123456789"
}
```

Without authentication, the server has no idea who sent the request.

It cannot determine whether the caller is:

- a trusted company
- one of your customers
- another bank
- an attacker

This creates a major security risk.

Authentication solves this problem.

---

# What is Authentication?

Authentication is the process of verifying the identity of a caller.

It answers the question:

> **Who are you?**

Examples include:

- Username and password
- API Key
- Client Certificate
- OAuth Client Credentials
- JWT
- OpenID Connect

Our platform uses:

```text
OAuth 2.0

↓

JWT Access Token
```

---

# Authentication vs Authorization

These two concepts are often confused.

They are closely related but perform different jobs.

| Authentication | Authorization |
|---------------|--------------|
| Verifies identity | Verifies permissions |
| Who are you? | What are you allowed to do? |
| Happens first | Happens after authentication |
| Produces an identity | Produces access decisions |

Example:

```text
Authenticate

↓

portal-client

↓

Authorize

↓

Allowed to call Balance API
```

Authentication answers:

> "You are portal-client."

Authorization answers:

> "You may access Balance but not Payments."

---

# Example

Imagine entering a secure office building.

At reception:

You show your employee badge.

The receptionist verifies your identity.

That is authentication.

Now you walk to a restricted server room.

Your badge is scanned again.

The door checks whether you have permission.

That is authorization.

Exactly the same process occurs inside APIs.

```text
Authentication

↓

Who are you?

↓

Authorization

↓

What can you access?
```

---

# Why Banking Systems Require Authentication

Banks expose extremely sensitive services.

Examples include:

- Account balances
- Statements
- Transfers
- Payroll
- Direct debit
- Beneficiary management

Every request must be authenticated.

Otherwise anyone could attempt to access customer accounts.

For this reason banks almost never expose APIs anonymously.

Instead they require one or more of:

- OAuth 2.0
- Mutual TLS (mTLS)
- JWT
- Digital certificates
- API Gateway authentication
- IP whitelisting

Large banks often combine several of these technologies together.

---

# Where Authentication Happens

Many beginners assume every API performs authentication itself.

Large systems rarely work this way.

Instead authentication is delegated to a dedicated service.

```text
Client

↓

Identity Server

↓

Access Token

↓

Business API
```

This separation has several advantages:

- Single authentication service
- Easier maintenance
- Centralized security
- Reusable tokens
- Consistent identity management

---

# What is an Identity Server?

An Identity Server is a dedicated application responsible for:

- authenticating clients
- issuing tokens
- validating credentials
- managing identities
- controlling scopes
- defining permissions

Notice what it does **not** do.

It does not:

- retrieve balances
- generate MT940 statements
- communicate with banks
- perform business logic

Its only responsibility is identity.

---

# Identity Server Responsibilities

Our Identity Server currently performs:

```text
Receive Client Credentials

↓

Validate Client

↓

Generate JWT

↓

Return Access Token
```

Later we can extend it with:

- Refresh Tokens
- OpenID Connect
- Certificate authentication
- Multi-factor authentication
- Database-backed clients
- Token revocation
- User authentication

---

# Our Architecture

Our platform now contains three independent applications.

```text
                +----------------------+
                |     Identity.Api     |
                |----------------------|
                | Authenticate Client  |
                | Generate JWT         |
                +----------+-----------+
                           |
                           | Access Token
                           |
                           v
+------------+      +-----------------------+      +--------------------+
|   Client   | ---> | BankIntegration.Api   | ---> | BankMockServer.Api |
+------------+      +-----------------------+      +--------------------+
```

Each application has one responsibility.

| Application | Responsibility |
|-------------|---------------|
| Identity.Api | Authentication |
| BankIntegration.Api | Business Logic |
| BankMockServer.Api | Simulated Bank |

This separation follows the **Single Responsibility Principle**.

---

# Why Separate Identity From Business Logic?

Imagine placing authentication inside every API.

```text
Balance API

↓

Validate Password

↓

Generate Token

↓

Check Client

↓

Return Balance
```

Now imagine adding:

- Payment API
- Statement API
- Beneficiary API
- FX API

Each API would duplicate the same authentication logic.

Instead we centralize authentication.

```text
Identity Server

↓

Token

↓

Business APIs
```

Every API trusts the Identity Server.

---

# OAuth 2.0

OAuth 2.0 is the industry standard authorization framework used by:

- Microsoft
- Google
- Amazon
- PayPal
- Visa
- Mastercard
- Open Banking providers

It defines **how applications obtain access tokens** without exposing credentials to every service.

Notice an important point:

OAuth is **not** a token format.

OAuth is a protocol.

JWT is one possible token format used by OAuth.

---

# Why OAuth Exists

Imagine a client needing access to five APIs.

Without OAuth:

```text
Client

↓

Username

↓

API 1

↓

Username

↓

API 2

↓

Username

↓

API 3
```

Credentials are repeatedly transmitted.

OAuth replaces this with:

```text
Authenticate Once

↓

Receive Token

↓

Reuse Token
```

The password (or client secret) is exchanged only once.

---

# OAuth Roles

OAuth defines several roles.

| Role | Responsibility |
|------|---------------|
| Resource Owner | Owns the data |
| Client | Application requesting access |
| Authorization Server | Authenticates clients |
| Resource Server | Hosts protected APIs |

Our project currently contains:

```text
Client

↓

Identity.Api

↓

BankIntegration.Api
```

Which maps to:

| OAuth Role | Our Project |
|------------|------------|
| Client | Portal Client |
| Authorization Server | Identity.Api |
| Resource Server | BankIntegration.Api |

---

# OAuth Grant Types

OAuth supports multiple authentication flows.

The most common are:

| Flow | Typical Use |
|-------|-------------|
| Authorization Code | Web applications |
| Client Credentials | Server-to-server |
| Device Code | Smart TVs |
| Refresh Token | Renew expired tokens |
| PKCE | Mobile & SPA applications |

For our integration platform we use:

```text
Client Credentials Flow
```

because there is **no human user** logging in.

---

# Why Client Credentials?

Our platform integrates companies and banks.

Example:

```text
ERP System

↓

Bank Integration Platform

↓

Bank
```

There is no person entering a username and password.

Instead:

One application authenticates another application.

This is exactly what the Client Credentials Flow was designed for.

---

# Client Credentials Flow

The complete flow is:

```text
Client

↓

POST /connect/token

↓

ClientId
ClientSecret

↓

Identity Server

↓

Validate Client

↓

Generate JWT

↓

Return Access Token

↓

Client Stores Token

↓

Authorization: Bearer <token>

↓

BankIntegration.Api

↓

Validate JWT

↓

Business Logic
```

Only the Identity Server ever sees the client secret.

Business APIs only receive the access token.

---

# Why JWT?

The Identity Server needs a way to represent an authenticated client.

JWT provides a compact, digitally signed token containing identity information.

Instead of storing sessions on the server, the server simply signs a token.

Every API can validate that signature.

This makes JWT:

- Stateless
- Fast
- Lightweight
- Scalable

---

# What is a JWT?

JWT stands for:

**JSON Web Token**

It is a compact string that securely represents identity information.

Example:

```text
eyJhbGciOiJIUzI1Ni...

...

...

K8dlL4r....
```

Although it appears random, it actually contains three separate sections.

---

# JWT Structure

A JWT contains:

```text
Header

.

Payload

.

Signature
```

Example:

```text
xxxxx.yyyyy.zzzzz
```

Each section is Base64Url encoded.

---

# JWT Header

The header describes the token.

Example:

```json
{
    "alg": "HS256",
    "typ": "JWT"
}
```

Meaning:

- Algorithm = HMAC SHA-256
- Type = JWT

---

# JWT Payload

The payload contains claims.

Example:

```json
{
    "sub": "portal-client",
    "client_id": "portal-client",
    "scope": [
        "balance.read",
        "statement.read"
    ]
}
```

These values describe the authenticated client.

They are **not encrypted**.

They are only encoded.

Never store secrets inside JWT payloads.

---

# JWT Signature

The signature proves the token has not been modified.

It is generated using:

- Header
- Payload
- Secret Key

If someone changes even one character inside the payload:

```text
balance.read

↓

payments.write
```

the signature immediately becomes invalid.

The API rejects the token.

This guarantees integrity.

---

# Stateless Authentication

Traditional web applications stored sessions in memory.

```text
Client

↓

Login

↓

Server stores session
```

JWT removes the need for server-side sessions.

Instead:

```text
Client

↓

Stores Token

↓

Sends Token

↓

API Validates Signature
```

The server does not need to remember each client.

This makes horizontal scaling much easier.

---
---

# Project Structure

After learning the theory behind OAuth and JWT, we can now examine how our Identity Server is organized.

Unlike a small demo application where everything is placed inside a single file, we have structured our project using a layered architecture similar to enterprise ASP.NET Core applications.

Current structure:

```text
Identity.Api
│
├── Authentication
│   ├── Models
│   │   ├── Client.cs
│   │   ├── TokenRequest.cs
│   │   └── TokenResponse.cs
│   │
│   └── Services
│       ├── IClientRegistry.cs
│       ├── InMemoryClientRegistry.cs
│       ├── IClientValidationService.cs
│       └── ClientValidationService.cs
│
├── Infrastructure
│   └── Security
│       ├── JwtSettings.cs
│       ├── IJwtTokenService.cs
│       └── JwtTokenService.cs
│
├── Controllers
│       TokenController.cs
│
└── Program.cs
```

Each folder has a single responsibility.

| Folder | Responsibility |
|---------|---------------|
| Authentication | Authentication-related models and services |
| Infrastructure | Security implementation |
| Controllers | API endpoints |
| Program.cs | Dependency Injection and middleware configuration |

This separation keeps the application maintainable as it grows.

---

# Authentication Models

Authentication revolves around three simple models.

```text
Client

↓

TokenRequest

↓

TokenResponse
```

Each model represents one stage of the authentication process.

---

# Client Model

The `Client` model represents an application that is allowed to authenticate against the Identity Server.

Unlike traditional login systems where a user enters a username and password, here an **application** authenticates itself.

Example:

```csharp
public class Client
{
    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public List<string> AllowedScopes { get; set; } = [];
}
```

---

# Why Do We Need a Client Model?

Every application consuming our platform must first be registered.

For example:

```text
ERP System

↓

portal-client

↓

Identity Server
```

The Identity Server needs to know:

- Which client is calling?
- What is its secret?
- Which scopes is it allowed to receive?

The `Client` model stores exactly this information.

---

# ClientId

Every client has a unique identifier.

Example:

```text
portal-client
```

Think of this as the application's username.

It identifies the calling application.

Multiple applications may exist:

```text
portal-client

mobile-app

payment-gateway

reporting-service

internal-api
```

Each receives its own unique Client ID.

---

# ClientSecret

Every client also owns a secret.

Example:

```text
SuperSecret123
```

This acts like the application's password.

During authentication:

```text
ClientId

+

ClientSecret

↓

Identity Server
```

If either value is incorrect, authentication fails.

In production systems, client secrets are:

- long
- randomly generated
- securely stored
- rotated regularly

They should never be hardcoded in client applications.

---

# AllowedScopes

A client should not automatically gain access to every API.

Instead we define exactly which operations it may perform.

Example:

```csharp
AllowedScopes =
[
    "balance.read",
    "statement.read"
];
```

This follows the Principle of Least Privilege.

Clients receive only the permissions they require.

---

# Why Store Scopes With the Client?

Imagine two clients.

```text
portal-client

↓

Balance
Statement
```

and

```text
payment-client

↓

Payments
Transfers
```

Each client has different permissions.

Embedding scopes within the client configuration allows the Identity Server to issue tokens containing only the authorised scopes.

---

# TokenRequest Model

The client authenticates by sending a token request.

```json
{
    "clientId": "portal-client",
    "clientSecret": "SuperSecret123"
}
```

This request maps to:

```csharp
public class TokenRequest
{
    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;
}
```

This object represents the login request sent to `/connect/token`.

---

# Why Separate TokenRequest From Client?

Although they appear similar, they serve different purposes.

`Client`

Represents a registered application stored by the Identity Server.

`TokenRequest`

Represents incoming data received from the client.

Keeping them separate avoids exposing internal configuration to external callers.

---

# TokenResponse Model

If authentication succeeds, the Identity Server returns a response.

Example:

```json
{
    "accessToken": "...",
    "tokenType": "Bearer",
    "expiresIn": 3600,
    "scope": "balance.read statement.read"
}
```

Represented by:

```csharp
public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = "Bearer";

    public int ExpiresIn { get; set; }

    public string Scope { get; set; } = string.Empty;
}
```

---

# Why Doesn't the Response Return the Client Secret?

Once authentication succeeds, the client secret is no longer needed.

Returning it would create a major security risk.

Instead the Identity Server returns only:

- Access Token
- Token Type
- Expiration
- Granted Scopes

The secret never leaves the Identity Server.

---

# Client Registry

The Identity Server must know which clients are allowed to authenticate.

This responsibility belongs to the Client Registry.

```text
Token Request

↓

Client Registry

↓

Find Client

↓

Return Client
```

---

# IClientRegistry

Rather than depending directly on a specific implementation, the Identity Server depends on an interface.

Example:

```csharp
public interface IClientRegistry
{
    Client? FindByClientId(string clientId);
}
```

Using an interface follows the Dependency Inversion Principle.

It allows the implementation to change without affecting the rest of the application.

---

# InMemoryClientRegistry

For this project we store clients in memory.

Example:

```text
Identity Server Starts

↓

Create Client List

↓

Store In Memory
```

Our implementation currently contains:

```text
portal-client

↓

ClientSecret

↓

AllowedScopes
```

This is sufficient for development.

---

# Why In-Memory?

Using an in-memory registry keeps the example simple.

It allows us to focus on authentication rather than database design.

In production, this registry would typically be replaced by:

- SQL Server
- PostgreSQL
- Azure Key Vault
- HashiCorp Vault
- Identity Provider
- Configuration Service

The rest of the application would remain unchanged because it depends only on `IClientRegistry`.

---

# Client Validation Service

The registry retrieves clients.

The validation service decides whether they are allowed to authenticate.

Workflow:

```text
Receive Request

↓

Find Client

↓

Compare Secret

↓

Return Valid / Invalid
```

Separating retrieval from validation follows the Single Responsibility Principle.

---

# IClientValidationService

The interface defines the authentication contract.

```csharp
Task<Client?> ValidateAsync(
    TokenRequest request);
```

Every validation service must implement this behaviour regardless of where clients are stored.

---

# ClientValidationService

The implementation performs three simple steps.

```text
Receive TokenRequest

↓

Find Client

↓

Compare ClientSecret

↓

Return Client

or

Return Null
```

Returning the complete client object allows the JWT service to access the client's scopes when generating the access token.

---

# JwtSettings

JWT generation requires configuration.

Instead of hardcoding values, we store them in `appsettings.json`.

```json
"JwtSettings": {
    "Issuer": "Identity.Api",
    "Audience": "BankIntegrationPlatform",
    "SecretKey": "...long secret..."
}
```

These settings are mapped into:

```csharp
public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;
}
```

---

# Why Use the Options Pattern?

Reading configuration directly throughout the application creates tight coupling.

Instead we bind configuration once:

```csharp
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
```

Later we inject:

```csharp
IOptions<JwtSettings>
```

This provides:

- Strong typing
- Centralised configuration
- Better testing
- Easier maintenance

---

# Current Authentication Flow

At this stage, our Identity Server performs the following steps:

```text
Client

↓

POST /connect/token

↓

TokenRequest

↓

ClientValidationService

↓

IClientRegistry

↓

Client Found?

↓

Yes

↓

Return Client

↓

Ready To Generate JWT
```

The client has now been authenticated.

The next step is to generate a secure JWT containing the client's identity and scopes.

---

---

# JWT Generation

Now that the client has been successfully authenticated, the Identity Server must issue an Access Token.

This is the heart of the Identity Server.

Without JWT generation, the client would successfully authenticate but would have nothing to present to protected APIs.

The responsibility for creating tokens belongs to the `JwtTokenService`.

---

# Why Do We Need JWT?

Imagine our platform without JWT.

```text
Client

↓

Username + Password

↓

BankIntegration.Api
```

Every API request would require the client to send its credentials.

Problems:

- Credentials travel over the network repeatedly.
- Authentication must happen on every request.
- Every service must know how to validate clients.
- Poor scalability.

JWT solves this problem.

Instead, authentication happens once.

```text
Client

↓

Identity Server

↓

JWT

↓

BankIntegration.Api

↓

Request Accepted
```

The client exchanges credentials for a token and then uses the token for future requests.

---

# What is Inside a JWT?

A JWT is simply a digitally signed JSON document.

Our generated token contains information such as:

```json
{
    "sub": "portal-client",
    "client_id": "portal-client",
    "scope": [
        "balance.read",
        "statement.read"
    ],
    "iss": "Identity.Api",
    "aud": "BankIntegrationPlatform",
    "exp": 1785232405
}
```

Notice that no password or client secret is included.

The token only contains identity and permissions.

---

# JWT Structure

Every JWT consists of three parts.

```text
Header

.

Payload

.

Signature
```

Example:

```text
xxxxx.yyyyy.zzzzz
```

Each section is Base64Url encoded.

---

# JWT Header

The header describes how the token is signed.

Example:

```json
{
    "alg": "HS256",
    "typ": "JWT"
}
```

Meaning:

| Field | Description |
|--------|-------------|
| alg | Hashing algorithm used |
| typ | Token type |

Our project currently uses:

```text
HS256
```

which means:

> HMAC SHA-256

---

# JWT Payload

The payload contains the claims.

Example:

```json
{
    "sub": "portal-client",
    "client_id": "portal-client",
    "scope": [
        "balance.read",
        "statement.read"
    ]
}
```

Claims describe the authenticated client.

---

# JWT Signature

The signature protects the token from modification.

It is created using:

```text
Header

+

Payload

+

Secret Key

↓

SHA256

↓

Digital Signature
```

If anyone changes the payload, the signature immediately becomes invalid.

This guarantees the integrity of the token.

---

# Why We Need a Secret Key

The secret key signs every token.

```text
Identity Server

↓

Secret Key

↓

Generate Signature
```

Later:

```text
BankIntegration.Api

↓

Same Secret Key

↓

Validate Signature
```

If the signatures match, the token is trusted.

If not, the request is rejected.

---

# JwtSettings

Instead of hardcoding values, JWT configuration is stored in `appsettings.json`.

Example:

```json
"JwtSettings": {
    "Issuer": "Identity.Api",
    "Audience": "BankIntegrationPlatform",
    "SecretKey": "ThisIsMySuperSecretKeyThatShouldBeAtLeast32CharactersLong!"
}
```

These settings are injected into the JWT service using the Options Pattern.

---

# IJwtTokenService

Rather than allowing controllers to generate tokens directly, we define a service contract.

```csharp
public interface IJwtTokenService
{
    string GenerateAccessToken(Client client);
}
```

This follows Dependency Injection principles.

The controller only knows that it can request a token.

It does not know how the token is generated.

---

# JwtTokenService

The implementation performs the complete JWT generation process.

Its responsibilities include:

- Reading JWT configuration.
- Creating claims.
- Creating signing credentials.
- Creating the JWT.
- Returning the serialized token.

This keeps authentication logic isolated inside a dedicated service.

---

# Creating Claims

Claims describe the authenticated client.

Our service creates claims similar to:

```csharp
new Claim(JwtRegisteredClaimNames.Sub, client.ClientId),

new Claim("client_id", client.ClientId),

new Claim(JwtRegisteredClaimNames.Jti,
          Guid.NewGuid().ToString())
```

These identify:

- Who authenticated.
- Which client authenticated.
- A unique identifier for the token.

---

# Adding Scopes

Each allowed scope becomes an individual claim.

Example:

```csharp
foreach (var scope in client.AllowedScopes)
{
    claims.Add(new Claim("scope", scope));
}
```

Result:

```text
balance.read

statement.read
```

Later, APIs use these claims for authorization.

---

# What is JTI?

Every generated token receives a unique identifier.

Example:

```text
d3c39355-4721-4e5b-9de3-2702d14b0f02
```

This is called the **JWT ID (JTI)**.

Reasons:

- Audit logging.
- Token revocation.
- Detect replay attacks.
- Token tracking.

Every generated JWT has a unique JTI.

---

# Signing Credentials

The next step is creating signing credentials.

```text
Secret Key

↓

SymmetricSecurityKey

↓

SigningCredentials

↓

HS256
```

These credentials digitally sign every JWT.

---

# Creating the JwtSecurityToken

Once everything has been prepared, we create the JWT.

Information included:

- Issuer
- Audience
- Claims
- Expiration
- Signing Credentials

The framework packages these into a `JwtSecurityToken`.

---

# Serializing the Token

The final step converts the token into a string.

```text
JwtSecurityToken

↓

JwtSecurityTokenHandler

↓

Compact JWT String
```

Example:

```text
eyJhbGciOiJIUzI1NiIsInR5...
```

This string is returned to the client.

---

# TokenController

The controller exposes the authentication endpoint.

```text
POST

/connect/token
```

Its responsibilities are intentionally small.

It simply:

1. Receives the request.
2. Validates the client.
3. Generates a JWT.
4. Returns the response.

Business logic remains inside services.

---

# Authentication Flow Inside TokenController

The controller follows this sequence.

```text
Receive TokenRequest

↓

Validate Client

↓

Client Exists?

↓

No

↓

401 Unauthorized

↓

Yes

↓

Generate JWT

↓

Return TokenResponse
```

This keeps the controller clean and focused.

---

# Successful Response

A successful authentication returns:

```json
{
    "accessToken": "...",
    "tokenType": "Bearer",
    "expiresIn": 3600,
    "scope": "balance.read statement.read"
}
```

The client stores this token and includes it in future requests.

---

# Invalid Client

If the Client ID or Client Secret is incorrect, the Identity Server returns:

```http
401 Unauthorized
```

No JWT is generated.

This prevents unauthorised applications from accessing protected services.

---

# Registering the Services

Everything is wired together using Dependency Injection.

```text
IClientRegistry

↓

InMemoryClientRegistry

↓

IClientValidationService

↓

ClientValidationService

↓

IJwtTokenService

↓

JwtTokenService
```

The controller depends only on interfaces.

---

# End-to-End Authentication Flow

Our Identity Server now performs the complete authentication process.

```text
Client

↓

POST /connect/token

↓

TokenController

↓

ClientValidationService

↓

InMemoryClientRegistry

↓

Client Found

↓

JwtTokenService

↓

Generate Claims

↓

Sign JWT

↓

Return Access Token
```

The client is now authenticated and receives a signed JWT that can be presented to protected APIs.

---

# What We Achieved

At this stage we have built a functioning Identity Server capable of:

- Registering clients.
- Validating client credentials.
- Generating signed JWT access tokens.
- Returning OAuth-style token responses.
- Using Dependency Injection throughout the authentication pipeline.
- Issuing tokens containing client identity and scopes.

This forms the authentication foundation for the remainder of the Bank Integration Platform.

---

---

# Securing BankIntegration.Api with JWT

Our Identity Server can now issue JWT access tokens.

The next step is ensuring that **BankIntegration.Api** only accepts requests containing valid tokens.

Without this step, anyone could call our APIs simply by knowing the endpoint URL.

Authentication moves from:

```text
Client

↓

BankIntegration.Api
```

to

```text
Client

↓

Identity.Api

↓

JWT

↓

BankIntegration.Api

↓

Protected Endpoint
```

---

# Why Protect the API?

Imagine our API exposed publicly.

```text
POST /api/v1/balance
```

Anyone could send:

```json
{
    "accountNumber": "123456",
    "bankCode": "SNB"
}
```

Even if they are not an authorised client.

This is unacceptable for banking systems.

Instead, every request must first prove its identity.

---

# Bearer Authentication

JWTs are normally transmitted using the HTTP **Authorization** header.

Example:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

The word **Bearer** tells ASP.NET Core that the following value is a bearer token.

Whoever possesses the token is treated as the authenticated client.

---

# Authentication vs Authorization

These two concepts are often confused.

Authentication answers:

> Who are you?

Authorization answers:

> What are you allowed to do?

Example:

```text
JWT Valid?

↓

Yes

↓

Does Client Have balance.read?

↓

Yes

↓

Access Granted
```

Authentication always happens before authorization.

---

# JWT Bearer Middleware

ASP.NET Core already includes middleware capable of validating JWT tokens.

Instead of writing token validation ourselves, we simply configure the built-in authentication handler.

```text
Incoming Request

↓

Authentication Middleware

↓

Validate JWT

↓

Continue Request
```

If validation fails, the request never reaches the controller.

---

# Installing JWT Authentication

Our project uses the official package:

```text
Microsoft.AspNetCore.Authentication.JwtBearer
```

This package provides:

- JWT validation
- Signature verification
- Lifetime validation
- Issuer validation
- Audience validation

---

# JwtSettings

Both APIs must agree on the same JWT configuration.

```json
"JwtSettings": {
    "Issuer": "Identity.Api",
    "Audience": "BankIntegrationPlatform",
    "SecretKey": "ThisIsMySuperSecretKeyThatShouldBeAtLeast32CharactersLong!"
}
```

The Identity Server signs tokens using these values.

BankIntegration.Api validates tokens using the same values.

---

# Registering JwtSettings

Configuration is registered using the Options Pattern.

```csharp
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
```

This allows the authentication middleware to access the JWT configuration.

---

# Configuring Authentication

Authentication is registered during application startup.

```csharp
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(...);
```

This tells ASP.NET Core:

> Every incoming Bearer token should be validated using the JWT Bearer handler.

---

# Token Validation Parameters

Several validation rules are configured.

```text
ValidateIssuer

ValidateAudience

ValidateLifetime

ValidateIssuerSigningKey
```

Each protects the API against a different type of attack.

---

# ValidateIssuer

The issuer identifies who created the token.

Expected value:

```text
Identity.Api
```

If another server generates the token:

```text
FakeIdentityServer
```

validation immediately fails.

---

# ValidateAudience

The audience identifies who the token was created for.

Expected value:

```text
BankIntegrationPlatform
```

If a token intended for another application is presented, it is rejected.

---

# ValidateLifetime

Every JWT expires.

Example:

```text
Now

↓

Token Valid

↓

1 Hour

↓

Expired

↓

Rejected
```

Expired tokens cannot be reused.

---

# ValidateIssuerSigningKey

The signature proves the token has not been modified.

ASP.NET Core recreates the signature using the configured secret key.

```text
Incoming Token

↓

Recalculate Signature

↓

Compare

↓

Valid?

↓

Continue
```

If even one character of the token changes, validation fails.

---

# Authentication Middleware

Once authentication is registered, it must be added to the request pipeline.

```csharp
app.UseAuthentication();

app.UseAuthorization();
```

Order matters.

Authentication must always execute before authorization.

---

# Request Pipeline

Our request pipeline now looks like this.

```text
HTTP Request

↓

Correlation Middleware

↓

Exception Middleware

↓

Authentication

↓

Authorization

↓

Controller

↓

BankService

↓

Adapter

↓

Bank
```

Only authenticated requests reach the controller.

---

# Protecting Controllers

Protecting an endpoint requires a single attribute.

```csharp
[Authorize]
```

Example:

```csharp
[Authorize]

[ApiController]

[Route("api/v1")]
public class BalanceController : ControllerBase
{
}
```

Now every endpoint inside this controller requires a valid JWT.

---

# What Happens Without a Token?

Request:

```http
POST /api/v1/balance
```

No Authorization header.

Result:

```http
401 Unauthorized
```

The controller is never executed.

---

# What Happens With an Invalid Token?

Example:

```http
Authorization: Bearer InvalidToken
```

Result:

```http
401 Unauthorized
```

The authentication middleware rejects the request before it reaches the business layer.

---

# What Happens With a Valid Token?

Example:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

Validation succeeds.

The request continues.

```text
Authentication

↓

Controller

↓

BankService

↓

Adapter

↓

Bank
```

The client now has access to protected resources.

---

# Testing with Postman

The complete authentication flow now consists of two requests.

## Step 1

Obtain a token.

```http
POST

/connect/token
```

Response:

```json
{
    "accessToken": "...",
    "tokenType": "Bearer"
}
```

---

## Step 2

Call the protected API.

```http
POST

/api/v1/balance
```

Headers:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

Body:

```json
{
    "accountNumber": "2334554",
    "bankCode": "SNB"
}
```

The API validates the JWT and returns the balance.

---

# Complete Authentication Flow

Our platform now performs a full authentication cycle.

```text
Client

↓

POST /connect/token

↓

Identity.Api

↓

Validate Client

↓

Generate JWT

↓

Access Token

↓

POST /api/v1/balance

↓

Authorization Header

↓

BankIntegration.Api

↓

Validate JWT

↓

Authorize Client

↓

BankService

↓

Adapter

↓

Bank

↓

Response
```

This is the same high-level architecture used by modern banking APIs.

---

# Current Security Features

At this stage, our platform provides:

- Client authentication.
- JWT generation.
- JWT signature validation.
- Issuer validation.
- Audience validation.
- Token expiration validation.
- Protected API endpoints.
- Standard Bearer authentication.

Although simplified, this closely mirrors how enterprise APIs secure communication.

---

# What's Still Missing?

Our API now knows **who** the client is.

The next improvement is determining **what** the client is allowed to do.

Currently, every authenticated client can access every protected endpoint.

In enterprise systems, access is controlled using **scopes**.

---

---

# Scope-Based Authorization

Authentication tells the API **who** the client is.

Authorization determines **what** that client is allowed to do.

Without authorization, every authenticated client would have unrestricted access to every endpoint.

In a banking platform, this would be a serious security risk.

---

# Authentication vs Authorization

These two concepts serve different purposes.

```text
Authentication

↓

"Who are you?"

↓

JWT Validation

↓

Authorization

↓

"What are you allowed to do?"
```

Authentication always happens first.

Only after a client is authenticated can permissions be evaluated.

---

# Why Do We Need Scopes?

Imagine we have three different clients.

```text
Portal Client

↓

Can View Balance

↓

Can View Statements


----------------------------------

Mobile App

↓

Can View Balance


----------------------------------

Payment Gateway

↓

Can Create Payments
```

Although all three are authenticated, each should have different permissions.

Scopes solve this problem.

---

# What is a Scope?

A scope is simply a permission.

Examples:

```text
balance.read

statement.read

payment.create

payment.cancel

customer.read

customer.update
```

Each scope represents a single business capability.

---

# Scopes Inside Our Client Model

Every registered client defines the scopes it is allowed to request.

Example:

```csharp
new Client
{
    ClientId = "portal-client",

    AllowedScopes =
    [
        "balance.read",
        "statement.read"
    ]
}
```

These scopes are issued inside the JWT.

---

# Scopes Inside the JWT

When the token is generated, every allowed scope becomes a claim.

Example:

```json
{
    "scope": [
        "balance.read",
        "statement.read"
    ]
}
```

These claims travel with every request.

---

# Reading the JWT

When a request reaches `BankIntegration.Api`, ASP.NET Core validates the JWT.

After validation, the framework creates a `ClaimsPrincipal`.

Example:

```text
JWT

↓

Claims

↓

HttpContext.User
```

Every claim becomes available through the authenticated user.

---

# ClaimsPrincipal

ASP.NET Core represents the authenticated client using:

```csharp
HttpContext.User
```

This object contains every claim extracted from the JWT.

Example:

```text
sub

client_id

scope

exp

iss

aud
```

Controllers can access these claims whenever necessary.

---

# The Authorize Attribute

The simplest form of authorization is:

```csharp
[Authorize]
```

This only checks:

> Is the client authenticated?

It does **not** check permissions.

---

# Authorization Policies

To enforce permissions, ASP.NET Core uses authorization policies.

Example registration:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "BalanceRead",
        policy =>
            policy.RequireClaim(
                "scope",
                "balance.read"));
});
```

This creates a reusable authorization rule.

---

# Applying a Policy

Once registered, the policy can protect a controller or action.

```csharp
[Authorize(Policy = "BalanceRead")]
```

Now the endpoint requires:

- A valid JWT.
- The `balance.read` scope.

---

# What Happens During Authorization?

The request flow now becomes:

```text
Incoming Request

↓

Validate JWT

↓

Extract Claims

↓

Does scope contain

balance.read?

↓

Yes

↓

Controller Executes

↓

No

↓

403 Forbidden
```

Authentication succeeds first.

Authorization then checks the required permission.

---

# 401 vs 403

These status codes are often confused.

### 401 Unauthorized

Meaning:

> The client is not authenticated.

Examples:

- No JWT.
- Invalid JWT.
- Expired JWT.

---

### 403 Forbidden

Meaning:

> The client is authenticated but lacks permission.

Example:

```text
JWT Valid

↓

Missing balance.read

↓

403 Forbidden
```

The client is recognised but not authorised.

---

# Multiple Scopes

Clients may have multiple permissions.

Example:

```json
{
    "scope": [
        "balance.read",
        "statement.read",
        "payment.create"
    ]
}
```

The API evaluates only the scope required for the requested endpoint.

---

# Different Endpoints, Different Policies

Our future APIs may look like this.

```text
/api/v1/balance

↓

Requires

balance.read


--------------------------------

/api/v1/statements

↓

Requires

statement.read


--------------------------------

/api/v1/payments

↓

Requires

payment.create
```

Each endpoint declares its own security requirements.

---

# Why This Scales Well

Suppose we add a new banking feature.

```text
Standing Orders
```

We simply introduce a new scope.

```text
standingorder.read
```

No authentication code changes.

Only a new authorization policy is added.

---

# Enterprise Example

Real banking platforms commonly define hundreds of scopes.

Example:

```text
account.read

account.write

payment.create

payment.approve

payment.cancel

beneficiary.read

beneficiary.create

customer.read

customer.update

admin.audit
```

Large systems rely on scopes to implement fine-grained security.

---

# Complete Security Flow

Our platform now performs the following sequence.

```text
Client

↓

Login

↓

Identity.Api

↓

JWT

↓

BankIntegration.Api

↓

Authentication

↓

Authorization

↓

Scope Check

↓

Business Logic

↓

Bank Adapter

↓

Bank
```

Every request is both authenticated and authorised before business logic executes.

---

# Benefits of Scope-Based Authorization

Using scopes provides several advantages.

- Fine-grained permissions.
- Easier permission management.
- Reusable authorization policies.
- Principle of least privilege.
- Better security.
- Easy expansion as new APIs are introduced.

---

# Best Practices

- Keep scopes small and focused.
- Use verb-noun naming (`balance.read`, `payment.create`).
- Protect every sensitive endpoint.
- Never rely on the client to enforce permissions.
- Always validate scopes on the server.
- Separate authentication from authorization.

---

# Interview Questions

1. What is the difference between authentication and authorization?
2. What is a JWT claim?
3. What is a scope?
4. Why are scopes included in JWTs?
5. What is the difference between 401 and 403?
6. How does `[Authorize]` differ from `[Authorize(Policy = "...")]`?
7. What is an authorization policy?
8. Why are scopes preferred over hard-coded role checks?
9. How does ASP.NET Core evaluate authorization policies?
10. Why should APIs enforce permissions instead of trusting the client?

---

# Key Takeaways

- Authentication identifies the client.
- Authorization determines the client's permissions.
- Scopes represent individual business capabilities.
- JWTs carry scopes as claims.
- ASP.NET Core policies validate scopes automatically.
- `401 Unauthorized` means the client is not authenticated.
- `403 Forbidden` means the client is authenticated but lacks the required permission.
- Scope-based authorization enables secure, scalable API design.

---

# Security Reminder

Swagger authentication is intended for development and testing.

In production:

- Use HTTPS.
- Protect Swagger with authentication or disable it entirely.
- Never expose development tokens publicly.
- Store secrets securely.
- Rotate signing keys regularly.

---

# Complete Platform Architecture

Our Bank Integration Platform now follows this authentication architecture.

```text
                 +----------------------+
                 |      Client          |
                 +----------+-----------+
                            |
                            |
                    POST /connect/token
                            |
                            v
                 +----------------------+
                 |    Identity.Api      |
                 |----------------------|
                 | Validate Client      |
                 | Generate JWT         |
                 +----------+-----------+
                            |
                     JWT Access Token
                            |
                            v
                 +----------------------+
                 |      Swagger         |
                 |   or Postman Client  |
                 +----------+-----------+
                            |
        Authorization: Bearer <JWT>
                            |
                            v
              +----------------------------+
              |   BankIntegration.Api      |
              |----------------------------|
              | Authenticate JWT           |
              | Authorize Scope            |
              | Execute Business Logic     |
              +-------------+--------------+
                            |
                            v
                 +----------------------+
                 |     Bank Adapter     |
                 +----------+-----------+
                            |
                            v
                 +----------------------+
                 | Mock / Real Bank API |
                 +----------------------+
```

This represents the complete authentication and authorisation pipeline implemented during this stage of the project.

---

# What We Have Built

By the end of this chapter, the platform includes:

- Identity Server.
- In-memory client registry.
- Client validation service.
- JWT generation.
- Token endpoint (`/connect/token`).
- JWT Bearer Authentication.
- Protected API endpoints.
- Scope-based authorization.
- Swagger JWT integration.
- End-to-end authentication flow between services.

Although simplified, this architecture closely resembles the authentication model used in modern enterprise APIs and banking integration platforms.

---

# Interview Questions

1. Why does Swagger require JWT authentication?
2. What is the purpose of `AddSecurityDefinition()`?
3. What is the difference between a Security Definition and a Security Requirement?
4. Why is the `Authorization` header used?
5. Why is the Bearer scheme preferred for JWTs?
6. How does Swagger automatically send the JWT with each request?
7. Why should Swagger be disabled or protected in production?
8. What happens if an expired token is used?
9. What happens if a valid token lacks the required scope?
10. Describe the complete authentication flow from client login to accessing a protected endpoint.

---

# Key Takeaways

- Swagger can be configured to authenticate using JWT Bearer tokens.
- `AddSecurityDefinition()` tells Swagger how authentication works.
- `AddSecurityRequirement()` applies that authentication scheme to API requests.
- After authorisation, Swagger automatically includes the `Authorization` header in every request.
- The platform now supports end-to-end authentication using Identity.Api, JWTs, protected endpoints, and scope-based authorization.
- This architecture provides a strong foundation for adding OAuth 2.0, OpenID Connect, mTLS, API Gateway security, refresh tokens, and role-based access control in future stages.

---

# Next Chapter

**11 – OAuth 2.0 & OpenID Connect**

In the next chapter, we will evolve our custom Identity Server into a standards-based authentication platform by exploring the OAuth 2.0 framework and OpenID Connect. We will learn the different grant types, understand access tokens versus ID tokens, and prepare the platform for enterprise-grade authentication and authorisation.