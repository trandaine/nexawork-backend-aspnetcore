using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NexaWork.Domain.Constants;
using NexaWork.Domain.IdentityEntites;
using NexaWork.Infrastructure;
using NexaWork.Infrastructure.Data.Seedings;
using NexaWork.Infrastructure.Data.Seedings.Authentications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<NexaWorkDbContext>();

builder.Services.AddDbContext<NexaWorkDbIdentityContext>(options =>
{
    // Configure Entity Framework Core to use SQL Server.
    // options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Register the entity sets needed by OpenIddict.
    options.UseOpenIddict();
});


builder.Services.AddIdentity<NexaWorkUser, NexaWorkRole>(options =>
{
    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = false; // Cho phép đăng nhập mà không cần xác nhận email
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    // User settings
    options.User.RequireUniqueEmail = true;
    // Sign-in settings
    options.SignIn.RequireConfirmedEmail = false;  // Sign in không cần confirm email
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<NexaWorkDbIdentityContext>()
    .AddDefaultTokenProviders();



// Cấu hình OpenIddict
builder.Services.AddOpenIddict()
    // Register the OpenIddict core components.
    .AddCore(options =>
    {
        // Configure OpenIddict to use the Entity Framework Core stores and models.
        options.UseEntityFrameworkCore()
               .UseDbContext<NexaWorkDbIdentityContext>();
    })
    // Register the OpenIddict server components.
    .AddServer(options =>
    {
        // Enable the authorization and token endpoints.
        options.SetAuthorizationEndpointUris("connect/authorize")
               .SetTokenEndpointUris("connect/token");

        // Enable the Authorization Code Flow with PKCE (The most secure standard)
        options.AllowAuthorizationCodeFlow();

        // 
        options.AllowPasswordFlow();

        // Register the signing and encryption credentials.
        // NOTE: For development, we use ephemeral keys. For production, you need a real certificate.
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        // Register the ASP.NET Core host and configure the options.
        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough()
               .EnableTokenEndpointPassthrough();
    })
    // Register the OpenIddict validation components.
    .AddValidation(options =>
    {
        // Import the configuration from the local OpenIddict server instance.
        options.UseLocalServer();
        options.UseAspNetCore();
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // policy.WithOrigins("http://localhost:5173") // Replace with your React app's URL
        policy.WithOrigins(BaseURLConstants.REACT_APP_URL) // Replace with your React app's URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Crucial for accepting the authentication cookie
    });
});




builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});






var app = builder.Build();




// Khởi chạy hàm Seeding khi app khởi động
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Chạy hàm tạo Roles và Admin
        await IdentityRoleDataSeeder.SeedRoleAsync(services);
        await IdentityUserDataSeeder.SeedAdminAsync(services);

        // Chạy hàm tạo Client cho OpenIddict
        await OpenIddictDataSeeder.SeedClientAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi trong quá trình Seeding dữ liệu Identity.");
    }
}




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    // Enable middleware for serving the generated JSON document
    app.UseSwagger();
    // Enable middleware for Swagger UI
    // Truy cập SwaggerUI tại URL: https://localhost:{port}/swagger/index.html
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "NexaWork Identity Provider is running.");

app.MapControllers();

app.Run();
