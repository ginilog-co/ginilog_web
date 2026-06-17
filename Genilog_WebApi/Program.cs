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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);


var allowedHosts = new[]
{
    "localhost",
    "127.0.0.1",
    "ginilog.org",
    "ginilog.com",
    "www.ginilog.org",
    "www.ginilog.com",
    "api.ginilog.org",
    "api-data.ginilog.org",
};

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                return false;

            return allowedHosts.Contains(
                uri.Host,
                StringComparer.OrdinalIgnoreCase)
                || uri.Host.EndsWith(".ginilog.org", StringComparison.OrdinalIgnoreCase)
                || uri.Host.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase)
                || uri.Host.EndsWith(".onrender.com", StringComparison.OrdinalIgnoreCase);
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});


// Add services to the container.
// ================= CONTROLLERS =================
builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// Initialize Firebase with error handling

var credential = CredentialFactory
    .FromFile<ServiceAccountCredential>(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ginilog-e3c8a-firebase-adminsdk-28ax3-07783858d2.json")
    )
    .ToGoogleCredential();

FirebaseApp.Create(new AppOptions()
{
    Credential = credential,
});



//builder.Services.AddDbContext<Genilog_Data_Context>(options =>
//{
//    options.UseNpgsql(builder.Configuration.GetConnectionString("Genilog_Data_Context"));
//});

builder.Services.AddDbContext<Genilog_Data_Context>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString("Genilog_Data_Context"));
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

// Repository Here
builder.Services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicPermissionPolicyProvider>();
builder.Services.AddScoped<IUserPermissionService, UserPermissionService>();
builder.Services.AddScoped<IBlacklistedTokenRepository, BlacklistedTokenRepository>();
builder.Services.AddHostedService<TokenCleanupService>();

// ================= TOKEN =================
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ginilog API",
        Version = "v1",
        Description = "Ginilog Backend API Documentation"
    });

    // 🔐 JWT AUTH SUPPORT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    // ✅ Show enums as strings (VERY IMPORTANT)
    options.UseInlineDefinitionsForEnums();
});

// optional configuration here
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
      )
  };
  options.Events = new JwtBearerEvents
  {
      OnTokenValidated = async context =>
      {
          var blacklistService = context.HttpContext.RequestServices
              .GetRequiredService<IBlacklistedTokenRepository>();

          // ✅ Correct: read JTI from claim
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
      }
  };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("OrderAny", policy =>
        policy.AddRequirements(
            new PermissionRequirement("order.view", "order.edit", "order.approve")));
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("emailLimiter", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(40);
        opt.PermitLimit = 3; // max 3 requests per minute per IP
        opt.QueueLimit = 0;
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

var keysPath = Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys");

Directory.CreateDirectory(keysPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("GinilogApp");
// Add SignalR (no additional package required)
builder.Services.AddSignalR();
var app = builder.Build();

// ================= PIPELINE =================
app.UseForwardedHeaders();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //  app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<IRolesRepository>();

    await RolePermissionSeeder.SeedRoles(repo);
    await RolePermissionSeeder.SeedPermissions(repo);
    await RolePermissionSeeder.SeedRolePermissions(repo);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseWebSockets();
app.UseRateLimiter();
app.Map("/ws", async (HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await WebSocketHandler.HandleConnection(webSocket, context);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();
