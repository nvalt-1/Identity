using System.Data.Common;
using System.Diagnostics;
using Identity.Classes.Identity;
using Identity.Models.Config;
using Identity.Models.Identity;
using Identity.Services;
using Identity.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddUserStore<UserStore>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.Configure<IdentityOptions>(options =>
{
    Debug.Assert(builder.Configuration.GetSection("PasswordConfig").Get<PasswordOptions>() != null);
    var passwordConfig = builder.Configuration.GetSection("PasswordConfig").Get<PasswordOptions>() ?? new PasswordOptions();

    // Password Requirements
    options.Password.RequireDigit = passwordConfig.RequireDigit;
    options.Password.RequiredLength = passwordConfig.RequiredLength;
    options.Password.RequireLowercase = passwordConfig.RequireLowercase;
    options.Password.RequireUppercase = passwordConfig.RequireUppercase;
    options.Password.RequireNonAlphanumeric = passwordConfig.RequireNonAlphanumeric;
    options.Password.RequiredUniqueChars = passwordConfig.RequiredUniqueChars;

    Debug.Assert(builder.Configuration.GetSection("LockoutConfig").Get<LockoutConfig>() != null);
    var lockoutConfig = builder.Configuration.GetSection("LockoutConfig").Get<LockoutConfig>() ?? new LockoutConfig();

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutConfig.DefaultLockoutMinutes);
    options.Lockout.MaxFailedAccessAttempts = lockoutConfig.MaxFailedAccessAttempts;
    options.Lockout.AllowedForNewUsers = lockoutConfig.AllowedForNewUsers;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    Debug.Assert(builder.Configuration.GetSection("CookieConfig").Get<CookieConfig>() != null);
    var cookieConfig = builder.Configuration.GetSection("CookieConfig").Get<CookieConfig>() ?? new CookieConfig();

    options.ExpireTimeSpan = TimeSpan.FromMinutes(cookieConfig.ExpireTimeMinutes);
    options.SlidingExpiration = cookieConfig.SlidingExpiration;
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    Debug.Assert(builder.Configuration.GetSection("SecurityStampConfig").Get<SecurityStampConfig>() != null);
    var securityStampConfig = builder.Configuration.GetSection("SecurityStampConfig").Get<SecurityStampConfig>() ?? new SecurityStampConfig();

    options.ValidationInterval = TimeSpan.FromMinutes(securityStampConfig.ValidationIntervalMinutes);
});

// Register app services
var connectionString = builder.Configuration.GetSection("ConnectionStrings:Default").Get<string>();
builder.Services.AddScoped<DbConnection, SqliteConnection>(_ => new SqliteConnection(connectionString));

builder.Services.AddScoped<IDatabaseService, SqlLiteDatabaseService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();

// Serilog
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
