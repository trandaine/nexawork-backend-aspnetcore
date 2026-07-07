# NexaWork Full-Stack AI Coding Agent Guidelines

Welcome, AI Assistant! This document is your ultimate architectural master guide for the **NexaWork** ecosystem. Review this document thoroughly to understand the design patterns, microservice boundaries, event-driven messaging, authentication flows, and frontend integrations before modifying or generating any code.

---

## 1. Project Overview & Ecosystem
**NexaWork** is a state-of-the-art, enterprise-grade professional networking platform (a modern LinkedIn clone) built on ASP.NET Core Clean Architecture and a Next.js (Turbopack) frontend.

### Core Business Capabilities
- **Identity & Security (`NexaWork.Authentication`)**: Centralized OAuth2/OIDC Identity Provider (IDP) utilizing OpenIddict, ASP.NET Core Identity, and advanced passwordless FIDO2/WebAuthn multi-factor authentication.
- **Protected Resource API (`NexaWork.Client`)**: The core business engine serving professional profiles, networking connections (self-referencing M2M), feeds, job boards, resumes, and static storage.
- **Frontend (`nexawork-frontend`)**: A Next.js application strictly implementing Feature-Sliced Design (FSD) architecture and utilizing the Backend-For-Frontend (BFF) pattern via Auth.js (NextAuth v5).

---

## 2. System Architecture & Data Flows

```
+-----------------------------------------------------------------------+
|                       Next.js Frontend (BFF & FSD)                    |
|       (http://localhost:3000 | Auth.js | TanStack Query | Axios)      |
+-----------------------------------------------------------------------+
          |                                            |
          | 1. OAuth2 / PKCE Login                     | 3. Bearer Token / API Requests
          v                                            v
+-------------------------------+             +-------------------------------+
|    NexaWork.Authentication    |             |        NexaWork.Client        |
|    (IDP: https://localhost:7036) |             | (Resource: https://localhost:7172) |
+-------------------------------+             +-------------------------------+
          |                                            ^
          | 2. Publishes UserRegisteredEvent           | 4. Consumes Event via MassTransit
          +-------------> [ RabbitMQ Broker ] ---------+
                                                       | 5. Token Introspection
                                                       +---> (Calls Auth Server on 7036)
```

### Key Architectural Pillars
1. **Decoupled Identity:** `NexaWork.Authentication` is the sole authority for user identity (`NexaWorkUser`, `NexaWorkRole`). The `NexaWork.Client` API never accesses identity tables directly; instead, it performs active token introspection against the Auth Server using its own client credentials (`nexawork_client_api`).
2. **Event-Driven Onboarding:** When a new user registers in `NexaWork.Authentication`, MassTransit publishes a `UserRegisteredEvent` to RabbitMQ. `NexaWork.Client` consumes this event via `UserRegisteredEventConsumer` and automatically seeds the user's `Customer` profile in the business database.
3. **CQRS via MediatR:** All business operations in `NexaWork.Client` are strictly separated into Commands (modifying state) and Queries (reading state). Controllers remain ultra-lean, merely dispatching requests via `_mediator.Send()`.
4. **Validation Pipeline:** FluentValidation operates as a MediatR pipeline behavior (`ValidationBehavior`). Requests are automatically validated before any command/query handler executes.

---

## 3. Microservice Deep Dive

### A. `NexaWork.Authentication` (Port 7036)
- **Database:** `NexaWorkIdentityDbContext` (SQL Server).
- **OAuth2 Server:** Powered by OpenIddict. Configured for Authorization Code Flow with PKCE (`/connect/authorize`, `/connect/token`, `/connect/logout`, `/connect/userinfo`, `/connect/introspect`).
- **Background Pruning:** Quartz.NET runs as a hosted background service to aggressively clean up expired tokens every 15 minutes.
- **FIDO2 / WebAuthn:** Integrates passwordless authentication (`WebAuthnController.cs`), storing challenge data in distributed memory session caches.
- **CORS Policy (`AllowAppsAccess`):** Whitelists `http://localhost:5173` (React) and `https://localhost:7172` (Client API).

### B. `NexaWork.Client` (Port 7172)
- **Database:** `NexaWorkDbContext` (SQL Server).
- **Controllers & Routing:** Adheres strictly to `[Route("api/[controller]")]`. For example, `CustomersController` endpoints start with `/api/Customers`.
- **Static Storage:** Configured with `PhysicalFileProvider` to serve uploaded profile banners and avatars directly from `../SharedStorage` onto the `/uploads` request path.
- **User Context:** `ICurrentUserService` extracts the authenticated user's ID directly from the introspected JWT claims (`HttpContext.User`).
- **CORS Policy:** Explicitly allows `BaseURLConstants.REACT_APP_URL` (`http://localhost:3000`) with `.AllowCredentials()`.

---

## 4. Frontend Integration & FSD Architecture

The Next.js frontend is structured around strict Feature-Sliced Design (FSD) layers:
```
src/
  ├── app/                  # App Router (routing, layout.tsx, globals.css, providers)
  ├── views/                # FSD Pages layer (full page components: dashboard, home, login)
  ├── widgets/              # Standalone UI blocks (app-sidebar, site-header, nav-user)
  ├── features/             # User interactions (auth-actions, login-form, dashboard charts)
  ├── entities/             # Domain representations (Customer types, useCustomer hook)
  ├── shared/               # Reusable core (shadcn UI, Axios apiClient, auth config)
  └── proxy.ts              # Next.js Middleware (resides at project root)
```

### Essential Frontend Rules
1. **API Client & Routes:** The global Axios instance (`src/shared/api/apiClient.ts`) utilizes `NEXT_PUBLIC_SERVER_RESOURCE_PUBLIC_API_URL` (`https://localhost:7172/api`). Always ensure client-side API requests include the controller name (e.g., `/Customers/profile-me`, **not** `/profile-me`).
2. **Error Interception:** The Axios response interceptor actively traps `401 Unauthorized` and `404 Not Found` API responses, instantly redirecting the browser to `/login`.
3. **Federated Logout:** The logout flow in `NavUser` executes an RP-Initiated Federated Logout. It destroys the local NextAuth session and redirects the browser to `https://localhost:7036/connect/logout?post_logout_redirect_uri=...&id_token_hint=...` to completely terminate the IDP session.

---

## 5. Critical Developer Workflows

### Adding a New Domain Feature
1. **Define Entity:** Create `NexaWork.Domain/Entities/{Entity}.cs`. Ensure all navigation properties are marked `virtual` to satisfy Entity Framework Core's proxy-based lazy loading requirements.
2. **Configure Entity:** Create `NexaWork.Infrastructure/Persistence/Configurations/{Entity}Configuration.cs` implementing `IEntityTypeConfiguration<{Entity}>` to define table names, foreign keys, and column constraints.
3. **Register DbSet:** Add `DbSet<{Entity}>` to `NexaWorkDbContext.cs`.
4. **Create Command/Query:** Define under `NexaWork.Application/Features/Client/{Entity}/Commands/{Action}/{Action}{Entity}Command.cs` as a C# `record` implementing `IRequest<T>`.
5. **Create Handler & Validator:** Define the `IRequestHandler` and `AbstractValidator` within the exact same feature folder.
6. **Create Controller Endpoint:** Add to `NexaWork.Client/Controllers/{Entity}sController.cs`, inject `ISender _mediator`, and execute `_mediator.Send(command)`. **Never inject `NexaWorkDbContext` directly into controllers.**

### Executing Database Migrations
Always specify the correct Startup (`-s`) and Project (`-p`) flags when generating migrations:

```bash
# Adding a migration for NexaWork.Client (from NexaWork.Client directory)
dotnet ef migrations add {MigrationName} -s NexaWork.Client -p NexaWork.Infrastructure -c NexaWorkDbContext

# Updating the database
dotnet ef database update -s NexaWork.Client -p NexaWork.Infrastructure -c NexaWorkDbContext
```

### Spinning Up Local Development
Execute the startup sequence in this exact order to ensure successful messaging and token validation:
```bash
# 1. Start RabbitMQ container
docker run --detach --hostname my-rabbit --name nexawork-rabbitmq \
    --env RABBITMQ_DEFAULT_USER=admin \
    --env RABBITMQ_DEFAULT_PASS=Admin@123456 \
    --publish 15672:15672 --publish 5672:5672 \
    rabbitmq:3-management

# 2. Start NexaWork.Authentication (Auth Server MUST be running first)
cd NexaWork.Authentication && dotnet run

# 3. Start NexaWork.Client (in a separate terminal)
cd NexaWork.Client && dotnet run

# 4. Start Next.js Frontend (in the frontend project directory)
npm run dev
```

---

## 6. Common Pitfalls & Anti-Patterns to Avoid

1. **Missing `NEXT_PUBLIC_` on Frontend Env Vars:** Client-side Axios will fail or hit Next.js 404s if `NEXT_PUBLIC_` is omitted from environment variable declarations.
2. **Omitting `virtual` on Navigation Properties:** Doing so completely breaks EF Core lazy loading, causing null reference exceptions when accessing child collections.
3. **Bypassing MediatR:** Never query or save to `DbContext` directly within API controllers. Always maintain strict CQRS separation.
4. **Ignoring Controller Route Prefixes:** Remember that C# controllers use `api/[controller]`. A frontend fetch to `/profile-me` will 404; it must be `/Customers/profile-me`.
5. **Direct Identity Table Access:** Never attempt to join `NexaWorkUser` tables directly from `NexaWork.Client`. Use OpenIddict token introspection and `ICurrentUserService`.
6. **Unregistered Validators:** Ensure every command has an accompanying FluentValidation class registered in the Dependency Injection container.
