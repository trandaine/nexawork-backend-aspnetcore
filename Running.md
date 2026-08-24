Running the NexaWork Project
Prerequisites (Docker)
# 1. Start SQL Server (Azure SQL Edge)
docker run --detach --hostname mssql --name nexawork-sql \
--env ACCEPT_EULA=Y \
--env MSSQL_SA_PASSWORD="Dai@2018" \
--publish 1433:1433 \
mcr.microsoft.com/azure-sql-edge

# 2. Start RabbitMQ
docker run --detach --hostname my-rabbit --name nexawork-rabbitmq \
--env RABBITMQ_DEFAULT_USER=admin \
--env RABBITMQ_DEFAULT_PASS=Admin@123456 \
--publish 15672:15672 --publish 5672:5672 \
rabbitmq:3-management

# Wait ~30 seconds for SQL Server to fully initialize
sleep 30
Run Migrations
# Apply AuthServer migrations (Identity + OpenIddict)
cd /Users/dainetr/Projects/nexawork-backend-aspnetcore
dotnet ef database update -s NexaWork.AuthServer -p NexaWork.AuthServer -c NexaWorkIdentityDbContext

# Apply main database migrations (NexaWorkDatabase)
dotnet ef database update -s NexaWork.Client -p NexaWork.Infrastructure -c NexaWorkDbContext

# Apply Message database migrations (NexaMessageDB)
dotnet ef database update -s NexaWork.Client -p NexaWork.Infrastructure -c MessageDbContext
Run Both Projects Simultaneously
Terminal 1 - AuthServer (port 7036):
cd /Users/dainetr/Projects/nexawork-backend-aspnetcore/NexaWork.AuthServer
dotnet run
Terminal 2 - Client API (port 5000/5001):
cd /Users/dainetr/Projects/nexawork-backend-aspnetcore/NexaWork.Client
dotnet run
Verify Running
Service	URL
AuthServer (OpenIddict)	https://localhost:7036 (https://localhost:7036)
Client API (Swagger)	https://localhost:5001/swagger (https://localhost:5001/swagger)
RabbitMQ Management	http://localhost:15672 (http://localhost:15672) (admin / Admin@123456)
Single Command (using tmux or screen)
# Using tmux - creates split panes with mouse on:

```aiignore
tmux new-session -d -s nexawork 'cd NexaWork.Authentication && dotnet run --launch-profile https' \; \
  set-option -g mouse on \; \
  split-window -h 'cd NexaWork.Client && dotnet run --launch-profile https' \; \
  attach-session
```


Important Notes
1. Order matters: AuthServer must start first (it publishes UserRegisteredEvent to RabbitMQ which Client consumes)
2. OpenIddict validation: Client API calls AuthServer's introspection endpoint - AuthServer must be healthy
3. Database: Both NexaWorkDatabase and NexaMessageDB are created automatically via migrations
4. Configuration: appsettings.Development.json files contain local dev settings (already created)