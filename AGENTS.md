# NexaWork AI Coding Agent Guidelines

## Project Overview
**NexaWork** is a LinkedIn Clone platform built with ASP.NET Core. It uses Clean Architecture with an event-driven messaging system to coordinate authentication, customer onboarding, and content sharing across multiple microservices.

## Architecture: The Big Picture

### Service Separation & Data Flows
```
[AuthServer (OpenIddict OAuth2)]
         ↓
    Publishes: UserRegisteredEvent
         ↓
[NexaWork.Client API (Main Business Logic)]  ← Consumes event via RabbitMQ
                                              ← Uses repositories & MediatR
                                              
[NexaWork.Authentication/Admin]              ← MVC projects
         ↓ (Tokens)
   Validates against AuthServer
```

**Critical Decision**: Authentication is intentionally decoupled via OpenIddict. The AuthServer (`NexaWork.AuthServer`) is the ONLY source for user identity. NexaWork.Client consumes this via OpenIddict validation, not direct db access.

### Core Patterns (Why They Matter)
1. **MediatR CQRS**: Commands/Queries organized by domain entity under `NexaWork.Application/Features/Client/{Entity}/{Commands|Queries}`
2. **Repository Pattern**: All db access through scoped repositories defined in `NexaWork.Infrastructure/Persistence/Repositories`
3. **Validation Pipeline**: `ValidationBehavior<TRequest, TResponse>` runs FluentValidation before handlers execute
4. **Event Publishing**: AuthServer publishes `UserRegisteredEvent` → RabbitMQ → NexaWork.Client consumes to create Customer profile

## Critical Developer Workflows

### Adding a New Feature (e.g., Skills Management)
1. **Define entity** in `NexaWork.Domain/Entities/Skill.cs` with `virtual` collection properties (required for EF lazy loading)
2. **Create DbSet** in `NexaWork.Infrastructure/Persistence/NexaWorkDbContext.cs`
3. **Add configuration** in `NexaWork.Infrastructure/Persistence/Configurations/SkillConfiguration.cs` (all entities require this)
4. **Create command**: `NexaWork.Application/Features/Client/Skill/Commands/Create/CreateSkillCommand.cs` as `record`, implement `IRequest<Guid>`
5. **Create handler**: `CreateSkillCommandHandler.cs` implementing `IRequestHandler<CreateSkillCommand, Guid>`
6. **Create validator**: `CreateSkillCommandValidator.cs` extending `AbstractValidator<CreateSkillCommand>`
7. **Create controller endpoint**: `NexaWork.Client/Controllers/SkillController.cs`, inject `IMediator` and call `mediator.Send(command)`

**Never** query DbContext directly in controllers—always use MediatR.

### Database Migrations
```bash
# Add migration (from NexaWork.Client project directory)
dotnet ef migrations add {MigrationName} -s NexaWork.Client -p NexaWork.Infrastructure -c NexaWorkDbContext

# Update database
dotnet ef database update -s NexaWork.Client -p NexaWork.Infrastructure -c NexaWorkDbContext
```

**Key Requirement**: Use `-s` for startup project (the project with Program.cs) and `-p` for the project containing DbContext.

### Running Local Development
```bash
# 1. Start RabbitMQ (required for messaging)
docker run --detach --hostname my-rabbit --name nexawork-rabbitmq \
    --env RABBITMQ_DEFAULT_USER=admin \
    --env RABBITMQ_DEFAULT_PASS=Admin@123456 \
    --publish 15672:15672 --publish 5672:5672 \
    rabbitmq:3-management

# 2. Ensure databases are migrated
dotnet ef database update -s NexaWork.Client -p NexaWork.Infrastructure

# 3. Run AuthServer first (it publishes events)
cd NexaWork.AuthServer && dotnet run

# 4. Run NexaWork.Client API (in separate terminal)
cd NexaWork.Client && dotnet run
```

**Why this order**: AuthServer must be running for NexaWork.Client's OpenIddict validation to work.

## Project-Specific Conventions

### Repository Implementations (Infrastructure Layer)
- Located in `NexaWork.Infrastructure/Persistence/Repositories`
- Always implement corresponding interface from `NexaWork.Application/Common/Interfaces/Repositories`
- Example: `IOrganizationRepository` → `OrganizationRepository`, registered as scoped in `DependencyInjection.cs`
- All repositories depend on `INexaWorkDbContext` injected via constructor

### Feature Organization
```
NexaWork.Application/Features/Client/
├── Organization/
│   ├── Commands/
│   │   └── Create/
│   │       ├── CreateOrganizationCommand.cs    (record IRequest<Guid>)
│   │       ├── CreateOrganizationHandler.cs    (IRequestHandler)
│   │       └── CreateOrganizationValidator.cs  (AbstractValidator)
│   └── Queries/
│       └── GetOrganization/
│           └── GetOrganizationQuery.cs
└── Customers/
    └── Commands/
        └── Create/
            └── CreateCustomerCommand.cs
```

**Naming Rule**: Match folder structure to command name (CreateOrganizationCommand → Create folder → CreateOrganization* files)

### Command Records Pattern
Use C# `record` type for commands (immutable, auto-generated equality):
```csharp
public record CreateOrganizationCommand(
    string Name,
    string? Industry,
    DateTime? FoundedDate
) : IRequest<Guid>;
```

### Entity Lazy Loading Requirements
All navigation properties must be `virtual` (required for EF's proxy-based lazy loading):
```csharp
public class Customer
{
    public Guid CustomerId { get; set; }
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual Organization? Organization { get; set; }
}
```

## Integration Points & Cross-Project Communication

### AuthServer ↔ Client Integration
- **NexaWork.AuthServer** (port 7036): Generates JWT tokens via OpenIddict
- **NexaWork.Client** (port 5000+): Validates tokens by calling AuthServer's introspection endpoint
- **Configuration** in `NexaWork.Client/Program.cs`:
  - Issuer: `https://localhost:7036`
  - Audience: `nexawork_client_api`
  - Introspection endpoint uses client credentials (see `SetClientId`/`SetClientSecret`)

### Event-Driven User Onboarding (RabbitMQ)
1. AuthServer registers user → publishes `UserRegisteredEvent` to RabbitMQ
2. Event contract: `NexaWork.Contracts/UserRegisteredEvent.cs`
3. NexaWork.Client listens via `UserRegisteredEventConsumer` (in Consumers folder)
4. Consumer uses MediatR to send `CreateCustomerCommand` + related commands
5. **Configuration**: `RabbitMQ` settings in `appsettings.json` (Host, VirtualHost, Username, Password)

### Current User Context (For Authorization)
- Inject `ICurrentUserService` in controllers to get current user ID from JWT claims
- Implementation: `NexaWork.Client/Services/CurrentUserService.cs`
- Automatically extracts from `HttpContext.User` claims

## Database & Migrations Context

### Key Entities (Extend these, don't create parallel structures)
- **Customer**: User profile (ForeignKey to AuthServer's NexaWorkUser via UserId)
- **Post**: Content shared by customers
- **Skill**: Catalog of available skills
- **Organization**: Company profiles
- **Connection**: Self-referencing M2M for customer networks
- **Education**, **Experience**: Resume data
- **Comment**, **Reaction**: Engagement on posts
- **JobListing**, **JobApplication**: Job board features

### Configuration Example Pattern
Each entity has a configuration class (e.g., `SkillConfiguration.cs` implementing `IEntityTypeConfiguration<Skill>`). These apply constraints like `HasMaxLength`, relationships, and table names in `OnModelCreating`.

## External Dependencies You'll Encounter

| Package | Purpose | Location |
|---------|---------|----------|
| **MediatR** | CQRS command/query dispatch | Application layer |
| **FluentValidation** | Request validation | Application validators |
| **OpenIddict** | OAuth2/OIDC server & validation | AuthServer & Client |
| **MassTransit** | Event publish/consume with RabbitMQ | Client Consumers |
| **Entity Framework Core** | ORM & migrations | Infrastructure |
| **Quartz** | Background job scheduling (cleanup) | AuthServer |

## Common Mistakes to Avoid

1. **Querying DbContext directly in handlers**: Use repositories instead
2. **Forgetting `virtual` on navigation properties**: Breaks lazy loading
3. **Mixing authentication concerns**: AuthServer owns user identity; Client uses tokens
4. **Hardcoding connection strings**: Use `appsettings.json` and `ConnectionStringConstants`
5. **Creating new DbContext instances**: Always inject and use the registered `INexaWorkDbContext`
6. **Skipping validation validators**: Every command must have a corresponding validator registered in Dependency Injection
7. **Wrong migration project order**: Use correct `-s` (startup) and `-p` (context) flags

