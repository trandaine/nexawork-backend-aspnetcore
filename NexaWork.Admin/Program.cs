using Microsoft.AspNetCore.Identity;
using NexaWork.Domain.IdentityEntites;
using NexaWork.Infrastructure;
using NexaWork.Infrastructure.Persistence;


var builder = WebApplication.CreateBuilder(args);
// var connectionString = builder.Configuration.GetConnectionString("NexaWorkDbIdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'NexaWorkDbIdentityContextConnection' not found.");;

// Add services to the container.
builder.Services.AddControllersWithViews();
// builder.Services.AddDbContext<NexaWorkDbContext>();

// builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<NexaWorkDbIdentityContext>();
// builder.Services.AddDbContext<NexaWorkDbIdentityContext>();

builder.Services.AddInfrastructureServices(builder.Configuration);




#region Cấu hình Identity
builder.Services.AddIdentity<NexaWorkUser, NexaWorkRole>(options =>
{
    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = true;
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(10);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    // User settings
    options.User.RequireUniqueEmail = true;
    // Sign-in settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;


})
    .AddEntityFrameworkStores<NexaWorkDbIdentityContext>()
    .AddDefaultTokenProviders();

// Cau hinh cookie va sessions
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Authentications/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);

    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

#endregion


builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


var app = builder.Build();






// Khởi chạy hàm Seeding khi app khởi động
// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     try
//     {
//         // Chạy hàm tạo Roles và Admin
//         await IdentityRoleDataSeeder.SeedRoleAsync(services);
//         await IdentityUserDataSeeder.SeedAdminAsync(services);
//     }
//     catch (Exception ex)
//     {
//         var logger = services.GetRequiredService<ILogger<Program>>();
//         logger.LogError(ex, "Đã xảy ra lỗi trong quá trình Seeding dữ liệu Identity.");
//     }
// }








// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
