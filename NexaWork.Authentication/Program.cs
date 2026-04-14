using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NexaWork.Domain.IdentityEntites;
using NexaWork.Infrastructure;
using NexaWork.Infrastructure.Data.Seedings;
using NexaWork.Infrastructure.Data.Seedings.Authentications;


var builder = WebApplication.CreateBuilder(args);
// var connectionString = builder.Configuration.GetConnectionString("NexaWorkDbIdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'NexaWorkDbIdentityContextConnection' not found.");;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<NexaWorkDbIdentityContext>(options =>
{
    options.UseOpenIddict();
});

// builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<NexaWorkDbIdentityContext>();


builder.Services.AddIdentity<NexaWorkUser, NexaWorkRole>(options => {
        // Có thể cấu hình thêm rules cho password tại đây
        options.SignIn.RequireConfirmedAccount = false; 
    })
    .AddEntityFrameworkStores<NexaWorkDbIdentityContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); // Cực kỳ quan trọng để các trang Scaffold UI hoạt động




builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});


builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<NexaWorkDbIdentityContext>();
    })
    .AddServer(options =>
    {
        // Cấu hình các Endpoints chuẩn
        options.SetAuthorizationEndpointUris("connect/authorize")
               .SetTokenEndpointUris("connect/token")
               .SetEndSessionEndpointUris("connect/logout");

        // Bật luồng Authorization Code (dành cho React & React Native)
        options.AllowAuthorizationCodeFlow();
        // Bắt buộc mọi Client phải dùng PKCE cho an toàn
        options.RequireProofKeyForCodeExchange();

        // Key dùng cho môi trường dev
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        // Đăng ký cho ASP.NET Core
        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough()
               .EnableTokenEndpointPassthrough();
            //    .EnableLogoutEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });



// //=== Cấu hình CORS để React App có thể gọi API mà không bị chặn bởi trình duyệt ===
// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(policy =>
//     {
//         policy.WithOrigins("http://localhost:5173") // Domain của React App
//               .AllowAnyHeader()
//               .AllowAnyMethod();
//     });
// });








var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<NexaWorkDbIdentityContext>();
    // Tự động apply migration và tạo DB nếu chưa có
    // context.Database.Migrate(); 

    try
    {
        await IdentityRoleDataSeeder.SeedRoleAsync(services);
        await IdentityUserDataSeeder.SeedAdminAsync(services); // Bỏ comment nếu bạn có file này
        await OpenIddictDataSeeder.SeedClientAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi Seeding dữ liệu.");
    }
}









// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // Cho phép phục vụ file tĩnh từ wwwroot


app.UseRouting();
// app.UseCors(); // Kích hoạt CORS nếu bạn đã cấu hình ở trên

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages(); // Bắt buộc để chạy các trang Identity UI

app.Run();
