using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NexaWork.Domain.Constants;
using NexaWork.Domain.IdentityEntites;
using NexaWork.Infrastructure.Data.Seedings;
using NexaWork.Infrastructure.Data.Seedings.Authentications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NexaWork.Infrastructure;
using NexaWork.Infrastructure.Persistence;
using NexaWork.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



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


app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "NexaWork Identity Provider is running.");

app.MapControllers();

app.Run();
