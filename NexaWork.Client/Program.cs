using NexaWork.Domain.Constants;
using NexaWork.Infrastructure;
using NexaWork.Application;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi;
using OpenIddict.Validation.AspNetCore;
using MassTransit;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Client.Consumers;
using NexaWork.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();





// Configure OpenIddict Validation
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        // Tell the API where your Auth Server lives
        options.SetIssuer("https://localhost:7036");

        // Tell the API its own name (must match the principal.SetResources in Auth Server)
        options.AddAudiences("nexawork_client_api");

        // Allow downloading the public keys from the Auth Server automatically
        options.UseSystemNetHttp();

        // Register the ASP.NET Core integration
        options.UseAspNetCore();

        // For introspection, tell the API how to authenticate to the Auth Server instead of instead of local JWT validation
        options.UseIntrospection()
               .SetClientId("nexawork_client_api")
               .SetClientSecret("v_IRV1;OPbz(*OhepHrh!6KYwM1o!!4pVO&MiLFjxJX");
    });

// Set OpenIddict as the default Authentication Scheme
builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();


// MassTransit with RabbitMQ (The Consumer)
var rabbitMqSettings = builder.Configuration
    .GetSection("RabbitMQ");
builder.Services.AddMassTransit(x =>
{
    // Tell MassTransit about your consumer
    x.AddConsumer<UserRegisteredEventConsumer>();
    x.AddMediator();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            rabbitMqSettings["Host"],
            rabbitMqSettings["VirtualHost"],
            h =>
            {
                // NOTE: Do NOT store production secrets directly in appsettings.json.
                h.Username(rabbitMqSettings["Username"]!);
                h.Password(rabbitMqSettings["Password"]!);
            });

        // Configure the specific queue this API will listen to
        cfg.ReceiveEndpoint("customer-creation-queue", e =>
        {
            e.ConfigureConsumer<UserRegisteredEventConsumer>(context);
        });
    });
});



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
                AuthorizationUrl = new Uri("https://localhost:7036/connect/authorize"),
                TokenUrl = new Uri("https://localhost:7036/connect/token"),
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
        // Notice how we pass the "document" parameter here now
        // [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()

        [new OpenApiSecuritySchemeReference("oauth2", document)] = new List<string> { "api" }
    });
});


// Required to read HTTP data inside a service
builder.Services.AddHttpContextAccessor(); 
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();


// Register infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Register application services
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
        options.OAuthClientId("nexawork_client_api_swagger");

        // Enable PKCE (Crucial for security, just like in React!)
        options.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();


var sharedStoragePath = Path.GetFullPath(builder.Configuration.GetValue<string>("Storage:SharedFolderPath") ?? "../SharedStorage");

// Ensure it exists when the API starts up
if (!Directory.Exists(sharedStoragePath))
{
    Directory.CreateDirectory(sharedStoragePath);
}

// CRITICAL: Map the physical folder to a web URL
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(sharedStoragePath),
    RequestPath = "/uploads" // This matches the string returned in our Service!
});





app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// app.MapGet("/", () => "NexaWork Identity Provider is running.");
// app.MapGet("/swagger/index.html", () => "Redirecting to Swagger UI...").ExcludeFromDescription();

app.MapControllers();

app.Run();
