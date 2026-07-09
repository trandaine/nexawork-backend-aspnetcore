# NexaWork Developer & AI Guide (GEMINI.md)

This document serves as the comprehensive architectural and operational guide for the NexaWork ecosystem. It is intended for both human developers and AI coding assistants to ensure consistency, scalability, and adherence to our established patterns.

## 🏗️ 1. Architecture Patterns

NexaWork is built on a **Clean Architecture** utilizing ASP.NET Core for the backend and Next.js (FSD) for the frontend. The system is distributed into specific microservices communicating via event-driven messaging.

### 1.1 Microservices & Ecosystem
- **Identity Provider (`NexaWork.Authentication` / `NexaWork.AuthServer`)**: Centralized OAuth2/OIDC IDP powered by OpenIddict. It is the sole authority for user identity (`NexaWorkUser`), utilizing ASP.NET Core Identity and FIDO2/WebAuthn.
- **Resource API (`NexaWork.Client`)**: The core business engine serving professional networking features (profiles, feeds, job boards).
- **Frontend (`nexawork-frontend`)**: A Next.js application implementing Feature-Sliced Design (FSD) and utilizing the Backend-For-Frontend (BFF) pattern via Auth.js (NextAuth v5).

### 1.2 Event-Driven Data Flow
- **RabbitMQ & MassTransit**: Used for cross-service asynchronous communication. For example, upon user registration in the Auth Server, a `UserRegisteredEvent` is published and consumed by the Client API to seed the `Customer` profile automatically.

### 1.3 Backend Design Patterns
- **CQRS via MediatR**: Strict separation of Commands (state modification) and Queries (data retrieval). Controllers only dispatch requests via `_mediator.Send()`.
- **Validation Pipeline**: FluentValidation is integrated as a MediatR pipeline behavior, ensuring all requests are validated before handler execution.
- **Repository Pattern**: Abstracts database access within the Infrastructure layer.
- **Token Introspection**: The `NexaWork.Client` never accesses identity tables. It performs active token introspection against the Auth Server using client credentials.

## 🧑‍💻 2. Coding Conventions

### 2.1 Backend (.NET)
- **Entities (Domain)**: Navigation properties **must** be marked as `virtual` to support Entity Framework Core's proxy-based lazy loading.
- **Controllers (Client)**: 
  - Controllers must be ultra-lean. **Never** inject `NexaWorkDbContext` directly into controllers.
  - Adhere strictly to the `[Route("api/[controller]")]` prefix (e.g., `/api/Customers`).
- **Feature Workflow (CQRS)**:
  1. Define Entity in `NexaWork.Domain/Entities/`.
  2. Configure Entity via `IEntityTypeConfiguration` in `NexaWork.Infrastructure`.
  3. Register `DbSet` in `NexaWorkDbContext`.
  4. Create Command/Query as C# `record` under `NexaWork.Application/Features/Client/{Entity}/...`.
  5. Create Handler (`IRequestHandler`) and Validator (`AbstractValidator`) in the same feature folder.
  6. Dispatch from the Controller.

### 2.2 Frontend (Next.js)
- **Feature-Sliced Design (FSD)**: Strictly adhere to layers: `app`, `views`, `widgets`, `features`, `entities`, and `shared`.
- **API Communication**: Use the global Axios instance in `src/shared/api/apiClient.ts`. Requests must include the controller name (e.g., `/Customers/profile-me`).
- **Environment Variables**: Always use the `NEXT_PUBLIC_` prefix for client-exposed variables (e.g., `NEXT_PUBLIC_SERVER_RESOURCE_PUBLIC_API_URL`).

## 🛠️ 3. Local Build Commands

The startup sequence is critical to ensure proper messaging and token validation.

### 3.1 Start Infrastructure
Spin up the RabbitMQ broker via Docker:
```bash
docker run --detach --hostname my-rabbit --name nexawork-rabbitmq \
    --env RABBITMQ_DEFAULT_USER=admin \
    --env RABBITMQ_DEFAULT_PASS=Admin@123456 \
    --publish 15672:15672 --publish 5672:5672 \
    rabbitmq:3-management
```

### 3.2 Database Migrations
Always specify the explicit Startup and Project flags when managing migrations for the Resource API:
```bash
# Add a migration
dotnet ef migrations add <MigrationName> -s NexaWork.Client -p NexaWork.Infrastructure -c NexaWorkDbContext

# Update the database
dotnet ef database update -s NexaWork.Client -p NexaWork.Infrastructure -c NexaWorkDbContext
```

### 3.3 Start the Services (Strict Order)
1. **Start Auth Server (Must be first for introspection)**
   ```bash
   cd NexaWork.Authentication && dotnet run
   ```
2. **Start Client API**
   ```bash
   cd NexaWork.Client && dotnet run
   ```
3. **Start Frontend**
   ```bash
   # Navigate to frontend project (e.g. nexawork-frontend)
   npm run dev
   ```

## 🧪 4. Testing Guidelines

- **Unit Testing**: 
  - Use **xUnit** as the primary testing framework.
  - Use **Moq** or **NSubstitute** for mocking interfaces (e.g., `IRepository`, `IMediator`).
  - Test Application logic (Handlers, Validators) in isolation without database dependencies.
- **Integration Testing**:
  - Use `WebApplicationFactory` to spin up in-memory instances of the APIs.
  - Utilize **Testcontainers** for spinning up ephemeral SQL Server and RabbitMQ instances during integration tests to ensure true fidelity.
- **Frontend Testing**:
  - Use **Jest** and **React Testing Library** for component testing within the FSD structure (targeting `widgets` and `features`).
  - E2E testing should leverage **Cypress** or **Playwright**, mapping flows from login (Auth.js) to resource interaction.
