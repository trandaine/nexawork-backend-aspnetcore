using Microsoft.AspNetCore.SignalR;
using NexaWork.Client.Hubs;
using NexaWork.Domain.Constants;
using NexaWork.Infrastructure;
using NexaWork.Application;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi;
using OpenIddict.Validation.AspNetCore;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Client.Consumers;
using NexaWork.Client.Services;
using NexaWork.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var messageConnectionString = builder.Configuration.GetConnectionString("MessageConnection") ?? connectionString;
var openIdictSettings = builder.Configuration.GetSection("OpenIddict");
var rabbitMqSettings = builder.Configuration.GetSection("RabbitMQ");
// var urlSettings = builder.Configuration.GetSection("Url");
var swaggerSettings = builder.Configuration.GetSection("Swagger");


builder.Services.AddDbContext<NexaWorkDbContext>(options => { options.UseSqlServer(connectionString); });
builder.Services.AddDbContext<MessageDbContext>(options => { options.UseSqlServer(messageConnectionString); });

// Configure OpenIddict Validation

#region OpenIdict Services

string GetRequiredOpenIdSetting(string key)
{
    var value = openIdictSettings[key];

    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException(
            $"Configuration Error: The OpenIddict setting '{key}' is missing or empty. " +
            $"Please check your appsettings.Development.json.");
    }

    return value;
}

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        // Tell the API where your Auth Server lives
        options.SetIssuer(GetRequiredOpenIdSetting("Issuer"));

        // Tell the API its own name
        options.AddAudiences(GetRequiredOpenIdSetting("Audience"));

        // Allow downloading the public keys from the Auth Server automatically
        options.UseSystemNetHttp();

        // Register the ASP.NET Core integration
        options.UseAspNetCore();

        // For introspection, tell the API how to authenticate to the Auth Server
        options.UseIntrospection()
            .SetClientId(GetRequiredOpenIdSetting("ClientId"))
            .SetClientSecret(GetRequiredOpenIdSetting("ClientSecret"));
    });

// Set OpenIddict as the default Authentication Scheme
builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

#endregion

// MassTransit with RabbitMQ (The Consumer)

#region RabbitMQ Services

string GetRequiredRabbitMqSettings(string key)
{
    var value = rabbitMqSettings[key];

    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException(
            $"Configuration Error: The RabbitMQ setting '{key}' is missing or empty. " +
            $"Please check your appsettings.Development.json.");
    }

    return value;
}

builder.Services.AddMassTransit(x =>
{
    // Tell MassTransit about your consumer
    x.AddConsumer<UserRegisteredEventConsumer>();
    x.AddMediator();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            GetRequiredRabbitMqSettings("Host"),
            GetRequiredRabbitMqSettings("VirtualHost"),
            h =>
            {
                h.Username(GetRequiredRabbitMqSettings("Username"));
                h.Password(GetRequiredRabbitMqSettings("Password"));
            });

        // Configure the specific queue this API will listen to
        cfg.ReceiveEndpoint(GetRequiredRabbitMqSettings("QueueName"),
            e => { e.ConfigureConsumer<UserRegisteredEventConsumer>(context); });
    });
});

#endregion


#region Swagger Services

string GetRequiredSwaggerSettings(string key)
{
    var value = swaggerSettings[key];

    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException(
            $"Configuration Error: The Swagger setting '{key}' is missing or empty. " +
            $"Please check your appsettings.Development.json.");
    }

    return value;
}
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NexaWork Client API", Version = "v1" });

    // Modern OpenAPI 3.0 definition
    // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    // {
    //     Name = "Authorization",
    //     Type = SecuritySchemeType.Http,
    //     Scheme = "bearer",
    //     BearerFormat = "JWT",
    //     In = ParameterLocation.Header,
    //     Description = "Paste your raw JWT Token here. Swagger will automatically add 'Bearer ' to the request."
    // });


    // Define the OAuth2 Security Scheme using Authorization Code Flow
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                // Point these to your Auth Server
                // AuthorizationUrl = new Uri("https://localhost:7036/connect/authorize"),
                AuthorizationUrl = new Uri(GetRequiredSwaggerSettings("AuthorizationCallback")),
                TokenUrl = new Uri(GetRequiredSwaggerSettings("TokenEndpoint")),
                Scopes = new Dictionary<string, string>
                {
                    // This must match the scope your API requires
                    { "api", "Access to NexaWork API" }
                }
            }
        }
    });

    // Global Security Requirement
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        // [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()

        [new OpenApiSecuritySchemeReference("oauth2", document)] = new List<string> { "api" }
    });
});
#endregion



// Required to read HTTP data inside a service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// SignalR Real-Time Services
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddScoped<IMessageNotificationService, MessageNotificationService>();


// builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();


#region Client API Services

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // policy.WithOrigins("http://localhost:5173") // Replace with your React app's URL
        policy.WithOrigins(BaseURLConstants.REACT_APP_URL) // Replace with your React app's URL
            .AllowAnyHeader()
            .AllowAnyMethod();
        //   .AllowCredentials(); // Crucial for accepting the authentication cookie
    });
});


// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = builder.Configuration["Jwt:Issuer"],
//         ValidAudience = builder.Configuration["Jwt:Audience"],
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//     };
// });

#endregion


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    // Enable middleware for serving the generated JSON document
    app.UseSwagger();
    // Enable middleware for Swagger UI
    // Truy cập SwaggerUI tại URL: https://localhost:{port}/swagger/index.html
    app.UseSwaggerUI(options =>
    {
        // Tell Swagger which Client ID to use
        options.OAuthClientId(swaggerSettings["OAuthClientId"]!);
        // options.OAuthClientId("nexawork_client_api_swagger");

        // Enable PKCE (Crucial for security, just like in React!)
        options.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();


var sharedStoragePath =
    Path.GetFullPath(builder.Configuration.GetValue<string>("Storage:SharedFolderPath")
                     ?? throw new InvalidOperationException("SharedFolderPath path is not configured in appsettings.json."));

// Ensure it exists when the API starts up
if (!Directory.Exists(sharedStoragePath))
{
    Directory.CreateDirectory(sharedStoragePath);
}

// CRITICAL: Map the physical folder to a web URL
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(sharedStoragePath),
    // RequestPath = "/uploads" // This matches the string returned in our Service!
    RequestPath = builder.Configuration.GetValue<string>("Storage:RequestPath")
                  ?? throw new InvalidOperationException("RequestPath of Storage is not configured in appsettings.json.") 
});


app.UseCors();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();