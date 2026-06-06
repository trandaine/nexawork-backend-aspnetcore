# NexaWork - Professional Networking Platform

## 📖 Project Overview
**NexaWork** is a comprehensive, microservices-oriented professional networking platform (a LinkedIn Clone). It facilitates social connections, content sharing, and recruitment. The platform utilizes an event-driven architecture that coordinates decoupled authentication, dynamic user portfolios, interactive social feeds, and a fully functional job board.

## ✨ Core Features
- **Profile & Networking:** Users can manage their personal profiles, including addresses and social links. It supports a custom connection system (self-referencing many-to-many) for users to build their professional networks.
- **Resumes & Portfolios:** Comprehensive management of user educational backgrounds, work experiences, and skill sets.
- **Content & Feeds:** Users can create posts (with media), comment on content, and react using a variety of diverse reactions (Like, Love, Insightful, etc.).
- **Job Board:** Organizations can post job listings. Users can discover jobs and submit applications, complete with their resumes and cover letters.

## 🛠 Tech Stack & Architecture
- **Frameworks:** ASP.NET Core (.NET Web API & MVC), React (Frontend).
- **Architecture:** **Clean Architecture** with a strict separation into Domain, Application, Infrastructure, and Presentation/Client layers.
- **Design Patterns:** 
  - **CQRS** implementation via **MediatR**.
  - **Repository Pattern** for database access abstraction.
  - **Pipeline Validation** leveraging **FluentValidation** within MediatR.
- **Database & ORM:** SQL Server managed with **Entity Framework Core**. Uses proxy-based lazy loading (requiring `virtual` navigation properties).
- **Authentication & Security:** Decoupled Identity using **OpenIddict** (OAuth2/OIDC) for generating and validating JWT tokens. Incorporates custom identity (`NexaWorkUser`) and supports advanced features like Two-Factor Authentication (2FA).
- **Message Broker:** **RabbitMQ** integrated via **MassTransit** for asynchronous cross-service messaging (e.g., triggering onboarding via `UserRegisteredEvent`).
- **Background Jobs:** **Quartz.NET** for system cleanup and scheduled background tasks.

## 📁 Project Structure
The solution is broken down into several key projects emphasizing Clean Architecture:
- `NexaWork.Domain`: Contains enterprise logic, core entities, and custom exceptions.
- `NexaWork.Application`: Contains business logic, MediatR commands/queries, interfaces, and FluentValidation validators.
- `NexaWork.Infrastructure`: Implementation of external concerns (EF Core DbContext, Repositories, messaging setups).
- `NexaWork.Client`: The primary Web API project serving the frontend and consuming RabbitMQ events.
- `NexaWork.AuthServer`: Standalone OpenIddict-based authentication server responsible for user identity and token generation.
- `NexaWork.Admin` / `NexaWork.Authentication`: MVC projects for administrative workflows and identity UI.

## 🚀 Getting Started

### Prerequisites
- .NET 8+ SDK
- SQL Server
- Docker Desktop (for RabbitMQ)
- Node.js (for React frontend)

### 1. Start Infrastructure (RabbitMQ)
Run the following Docker command to spin up the RabbitMQ instance required for microservice communication:
```bash
docker run --detach --hostname my-rabbit --name nexawork-rabbitmq \
    --env RABBITMQ_DEFAULT_USER=admin \
    --env RABBITMQ_DEFAULT_PASS=Admin@123456 \
    --publish 15672:15672 \
    --publish 5672:5672 \
    rabbitmq:3-management
```

### 2. Database Migrations
Ensure the database is up-to-date. Run the following commands from the project root:
```bash
# Add a new migration if needed
dotnet ef migrations add InitialCreate -s NexaWork.Client -p NexaWork.Infrastructure -c NexaWorkDbContext

# Update the database schema
dotnet ef database update -s NexaWork.Client -p NexaWork.Infrastructure -c NexaWorkDbContext
```

### 3. Run the Backend Services
**Important Order:** The `AuthServer` must be running before the `Client` API because the Client validates JWT tokens against the AuthServer's introspection endpoint.

**Terminal 1 (AuthServer):**
```bash
cd NexaWork.AuthServer
dotnet run
```

**Terminal 2 (Client API):**
```bash
cd NexaWork.Client
dotnet run
```

## 💻 Developer Guidelines

### Adding a New Feature (CQRS Workflow)
When adding new domain features (e.g., `Skill`), follow this standardized flow:
1. **Domain:** Define the entity in `NexaWork.Domain/Entities/` (ensure navigation properties are `virtual`).
2. **Infrastructure:** Create the `DbSet`, configuration (`IEntityTypeConfiguration`), and Repository implementation. Register them in Dependency Injection.
3. **Application:** Create MediatR Command/Query records, Handlers, and FluentValidation classes under `NexaWork.Application/Features/Client/{Entity}/`.
4. **Client API:** Create endpoints in the Controllers that inject `IMediator` to dispatch commands/queries. **Never** use `DbContext` directly in controllers or handlers.

