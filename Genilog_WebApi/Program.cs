using FirebaseAdmin;
using Genilog_WebApi.DataContext;
using Genilog_WebApi.Key;
using Genilog_WebApi.Repository;
using Genilog_WebApi.Repository.AdminRepo;
using Genilog_WebApi.Repository.AuthRepo;
using Genilog_WebApi.Repository.AuthRepo.PolicyBased;
using Genilog_WebApi.Repository.BookingsRepo;
using Genilog_WebApi.Repository.GeneralRepo;
using Genilog_WebApi.Repository.InfoRepo;
using Genilog_WebApi.Repository.LogisticsRepo;
using Genilog_WebApi.Repository.NotificationRepo;
using Genilog_WebApi.Repository.UploadRepo;
using Genilog_WebApi.Repository.UserRepo;
using Genilog_WebApi.Repository.WalletRepo;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using System.IO.Compression;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// ================= DEFINED ALLOWED ORIGINS =================
// Explicitly define allowed origins instead of using dynamic checking
var allowedOrigins = new[]
{
    // Local development
    "http://localhost:3000",
    "http://localhost:3001",
    "http://localhost:5173",
    "http://localhost:8080",
    "https://localhost:3000",
    "https://localhost:3001",
    "https://localhost:5173",
    "https://localhost:8080",
    
    // Your production domain
    "https://www.ginilog.com",
    "https://ginilog.com",
    
    // Vercel deployments
    "https://ginilog.vercel.app",
    "https://ginilog-git-main.vercel.app",
    "https://ginilog-git-*.vercel.app",
    "https://*.vercel.app",
    
    // Render backend (for testing)
    "https://*.onrender.com",
    
    // Your other domains
    "https://api-data.ginilog.org",
    "https://*.ginilog.org"
};

// Add response compression for better performance
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

// ================= CORS FIX =================
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        // Use specific origins instead of dynamic checking
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10)); // Cache preflight requests
    });
});

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// Initialize Firebase with error handling
try
{
    var firebaseJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ginilog-e3c8a-firebase-adminsdk-28ax3-07783858d2.json");
    
    if (File.Exists(firebaseJsonPath))
    {
        var credential = CredentialFactory
            .FromFile<ServiceAccountCredential>(firebaseJsonPath)
            .ToGoogleCredential();

        FirebaseApp.Create(new AppOptions()
        {
            Credential = credential,
        });
        Console.WriteLine("Firebase initialized successfully");
    }
    else
    {
        Console.WriteLine($"Firebase credentials not found at: {firebaseJsonPath}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to initialize Firebase: {ex.Message}");
    // Don't throw, allow app to start but log error
}

// Database Context
builder.Services.AddDbContext<Genilog_Data_Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Genilog_Data_Context"));
    // Add these for better debugging in production
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
});

// ================= CONFIG =================
builder.Services.Configure<PaymentConfig>(builder.Configuration.GetSection("Payment"));
builder.Services.Configure<FirebaseConfig>(builder.Configuration.GetSection("Firebase"));
builder.Services.Configure<ServerConfig>(builder.Configuration.GetSection("Server"));

// Register Cls_Keys as singleton
builder.Services.AddSingleton<Cls_Keys>();

// ================= REPOSITORIES =================
builder.Services.AddScoped<IGeneralUserRepository, GeneralUserRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRolesRepository, RoleRepository>();
builder.Services.AddScoped<IUser_RoleRepository, User_RoleRepository>();
builder.Services.AddScoped<ITokenHandler, Genilog_WebApi.Repository.AuthRepo.TokenHandler>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IUploadRepository, UploadRepository>();

builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IRidersRepository, RidersRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

builder.Services.AddScoped<IAccomodationRepository, AccomodationRepository>();
builder.Services.AddScoped<IAirlineRepository, AirlineRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();

builder.Services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicPermissionPolicyProvider>();
builder.Services.AddScoped<IUserPermissionService, UserPermissionService>();
builder.Services.AddScoped<IBlacklistedTokenRepository, BlacklistedTokenRepository>();
builder.Services.AddHostedService<TokenCleanupService>();

// ================= TOKEN =================
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();

// ================= SWAGGER =================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ginilog API",
        Version = "v1",
        Description = "Ginilog Backend API Documentation"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.UseInlineDefinitionsForEnums();
});

// AutoMapper
builder.Services.AddAutoMapper(config =>
{
    config.LicenseKey = builder.Configuration["Jwt:AutoMapperToken"];
}, typeof(Program).Assembly);

// ================= AUTH =================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        ),
        ClockSkew = TimeSpan.FromSeconds(30) // Allow 30 seconds clock skew
    };
    
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Handle token from query string for SignalR/WebSockets
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/ws"))
            {
                context.Token = accessToken;
            }
            
            return Task.CompletedTask;
        },
        
        OnTokenValidated = async context =>
        {
            var blacklistService = context.HttpContext.RequestServices
                .GetRequiredService<IBlacklistedTokenRepository>();

            var jti = context.Principal?.Claims
                        .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (!string.IsNullOrEmpty(jti))
            {
                var isBlacklisted = await blacklistService.IsTokenBlacklistedAsync(jti);
                if (isBlacklisted)
                {
                    context.Fail("Token has been revoked");
                }
            }
        },
        
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("OrderAny", policy =>
        policy.AddRequirements(
            new PermissionRequirement("order.view", "order.edit", "order.approve")));

// ================= RATE LIMITING =================
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("emailLimiter", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(40);
        opt.PermitLimit = 3;
        opt.QueueLimit = 0;
    });
    
    options.AddFixedWindowLimiter("apiLimiter", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
        opt.QueueLimit = 10;
    });
    
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
    };
});

// ================= FORWARDED HEADERS =================
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// ================= DATA PROTECTION =================
var keysPath = Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys");
Directory.CreateDirectory(keysPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("GinilogApp");

// ================= SIGNALR =================
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.MaximumReceiveMessageSize = 102400;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// ================= PIPELINE =================

// Forwarded headers must be first
app.UseForwardedHeaders();

// Response compression
app.UseResponseCompression();

// Configure pipeline based on environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        options.DisplayRequestDuration();
    });
}
else
{
    app.UseHsts();
}

// Seed roles and permissions
using (var scope = app.Services.CreateScope())
{
    try
    {
        var repo = scope.ServiceProvider.GetRequiredService<IRolesRepository>();
        await RolePermissionSeeder.SeedRoles(repo);
        await RolePermissionSeeder.SeedPermissions(repo);
        await RolePermissionSeeder.SeedRolePermissions(repo);
        Console.WriteLine("Roles and permissions seeded successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding roles and permissions: {ex.Message}");
    }
}

// Health check endpoint
app.MapHealthChecks("/health");

// HTTPS Redirection (skip in development if needed)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

// CORS must be between UseRouting and UseAuthentication
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// Map controllers
app.MapControllers();

// WebSocket support
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(30),
    ReceiveBufferSize = 4 * 1024
});

// WebSocket endpoint with better error handling
app.Map("/ws", async (HttpContext context) =>
{
    try
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("WebSocket connection established");
            await WebSocketHandler.HandleConnection(webSocket, context);
        }
        else
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("WebSocket request expected");
        }
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "WebSocket connection error");
        
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("WebSocket connection failed");
        }
    }
});

// Optional: Add a test endpoint for CORS
app.MapGet("/test-cors", () => Results.Ok(new { message = "CORS is working!" }));

app.Run();
