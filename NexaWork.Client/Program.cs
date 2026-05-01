using NexaWork.Domain.Constants;
using NexaWork.Infrastructure.Data.Seedings;
using NexaWork.Infrastructure.Data.Seedings.Authentications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NexaWork.Infrastructure;
using NexaWork.Application;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();


// builder.Services.AddAuthentication().AddJwtBearer();
// builder.Services.AddAuthorization();








builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NexaWork API", Version = "v1" });

    // 1. Modern OpenAPI 3.0 definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste your raw JWT Token here. Swagger will automatically add 'Bearer ' to the request."
    });

    // 2. Global Security Requirement
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        // Notice how we pass the "document" parameter here now
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});



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
#endregion


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


var sharedStoragePath = Path.GetFullPath(builder.Configuration.GetValue<string>("Storage:SharedFolderPath") ?? "../SharedStorage");

// Ensure it exists when the API starts up
if (!Directory.Exists(sharedStoragePath))
{
    Directory.CreateDirectory(sharedStoragePath);
}

// 2. ⚡ CRITICAL: Map the physical folder to a web URL
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
