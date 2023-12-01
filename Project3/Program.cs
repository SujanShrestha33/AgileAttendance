using Microsoft.AspNetCore.Identity;
using BiometricAttendanceSystem.Infrastrucuture.Identity;
using Core.Entities;
using Core.Entities.Identity;
using Core.Interface;
using Infrastructure.Identity;
using Infrastructure.Services;
//using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BiometricAttendanceSystem.Helper;
using Core;
using Hangfire;
using Hangfire.SqlServer;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Configuration.AddJsonFile("appsettings.json", optional: false);
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/dist";
});
builder.Services.AddDbContext<BiometricAttendanceReaderDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

GlobalConfiguration.Configuration.UseSqlServerStorage(@"Data Source=DESKTOP-29HDJAG\SQLEXPRESS; Database=HangfireBiometric; Integrated Security=true;");

var connectionString = builder.Configuration.GetConnectionString("HangfireConnection");

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

builder.Services.AddHangfireServer();

//register custom services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IDeviceConfigRepository, DeviceConfigRepository>();
builder.Services.AddScoped<IAttendanceLogRepository, AttendanceLogRepository>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Identity options here
})
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AppIdentityServices(builder.Configuration);

builder.Services.AddScoped<AttendanceRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStaticFiles();
if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles(); 
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//Scheduling
app.UseHangfireDashboard();

//Schedule the get-device-config function to run minutely
//RecurringJob.AddOrUpdate<IDeviceConfigRepository>("get-device-config", service => service.GetDeviceConfigLIVE(), Cron.Minutely);

//Schedule the get-attendance-log function to run minutely
RecurringJob.AddOrUpdate<IAttendanceLogRepository>("get-attendance-log", service => service.GetAttendanceLogListLIVE(), "03 25 * * *", TimeZoneInfo.Local);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var loggerFactory = services.GetRequiredService<ILoggerFactory>();
try
{
    // Seeding identity data to identity database
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var identityContext = services.GetRequiredService<AppIdentityDbContext>();
    await identityContext.Database.MigrateAsync();
    await AppIdentityDbContextSeed.SeedUserAsync(userManager);
}
catch (Exception ex)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(ex, "An error occurred seeding the DB.");
}

app.Run();
