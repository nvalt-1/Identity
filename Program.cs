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

#region IDENTITY
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
    Debug.Assert(builder.Configuration.GetSection("Identity:PasswordConfig").Get<PasswordOptions>() != null);
    var passwordConfig = builder.Configuration.GetSection("Identity:PasswordConfig").Get<PasswordOptions>() ?? new PasswordOptions();

    // Password Requirements
    options.Password.RequireDigit = passwordConfig.RequireDigit;
    options.Password.RequiredLength = passwordConfig.RequiredLength;
    options.Password.RequireLowercase = passwordConfig.RequireLowercase;
    options.Password.RequireUppercase = passwordConfig.RequireUppercase;
    options.Password.RequireNonAlphanumeric = passwordConfig.RequireNonAlphanumeric;
    options.Password.RequiredUniqueChars = passwordConfig.RequiredUniqueChars;

    Debug.Assert(builder.Configuration.GetSection("Identity:LockoutConfig").Get<LockoutConfig>() != null);
    var lockoutConfig = builder.Configuration.GetSection("Identity:LockoutConfig").Get<LockoutConfig>() ?? new LockoutConfig();

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutConfig.DefaultLockoutMinutes);
    options.Lockout.MaxFailedAccessAttempts = lockoutConfig.MaxFailedAccessAttempts;
    options.Lockout.AllowedForNewUsers = lockoutConfig.AllowedForNewUsers;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    Debug.Assert(builder.Configuration.GetSection("Identity:CookieConfig").Get<CookieConfig>() != null);
    var cookieConfig = builder.Configuration.GetSection("Identity:CookieConfig").Get<CookieConfig>() ?? new CookieConfig();

    options.ExpireTimeSpan = TimeSpan.FromMinutes(cookieConfig.ExpireTimeMinutes);
    options.SlidingExpiration = cookieConfig.SlidingExpiration;
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    Debug.Assert(builder.Configuration.GetSection("Identity:SecurityStampConfig").Get<SecurityStampConfig>() != null);
    var securityStampConfig = builder.Configuration.GetSection("Identity:SecurityStampConfig").Get<SecurityStampConfig>() ?? new SecurityStampConfig();

    options.ValidationInterval = TimeSpan.FromMinutes(securityStampConfig.ValidationIntervalMinutes);
});

#endregion IDENTITY
#region SESSION

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    Debug.Assert(builder.Configuration.GetSection("SessionConfig").Get<SessionConfig>() != null);
    var sessionConfig = builder.Configuration.GetSection("SessionConfig").Get<SessionConfig>() ?? new SessionConfig();

    options.IdleTimeout = TimeSpan.FromMinutes(sessionConfig.IdleTimeoutMinutes);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

#endregion SESSION

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

app.UseSession();

app.MapControllers();

app.Run();
