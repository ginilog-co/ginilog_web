using FirebaseAdmin;
using Genilog_WebApi.DataContext;
using Genilog_WebApi.Repository;
using Genilog_WebApi.Repository.AdminRepo;
using Genilog_WebApi.Repository.AuthRepo;
using Genilog_WebApi.Repository.BookingsRepo;
using Genilog_WebApi.Repository.InfoRepo;
using Genilog_WebApi.Repository.LogisticsRepo;
using Genilog_WebApi.Repository.NotificationRepo;
using Genilog_WebApi.Repository.PlacesRepo;
using Genilog_WebApi.Repository.UploadRepo;
using Genilog_WebApi.Repository.UserRepo;
using Genilog_WebApi.Repository.WalletRepo;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins(
                                "https://ginilog.onrender.com",
                                "http://localhost:3000",
                                "https://api-data.ginilog.com",
                                "https://ginilog-web.vercel.app",  // Your Vercel frontend
                                "https://ginilog-web.onrender.com" // Render API URL
                                )
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowCredentials(); // Important for SignalR
                      });
});

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter a Valid JWT bearer token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme,Array.Empty<string>() }
    });
});

builder.Services.ConfigureSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GeniLog Web App API",
        Version = "v1"
    });
});

// Initialize Firebase with error handling
try
{
    var firebaseCredentialPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Key", "ginilog-e3c8a-firebase-adminsdk-28ax3-07783858d2.json");
    if (File.Exists(firebaseCredentialPath))
    {
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(firebaseCredentialPath),
        });
        Console.WriteLine("Firebase initialized successfully.");
    }
    else
    {
        Console.WriteLine($"Warning: Firebase credential file not found at {firebaseCredentialPath}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Warning: Firebase initialization failed: {ex.Message}");
}

builder.Services.AddDbContext<Genilog_Data_Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Genilog_Data_Context"));
});

// Repository Here
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


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHttpClient<ITwilioRestClient, TwilioClient>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    });

// Add SignalR (no additional package required)
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Map SignalR hub

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseWebSockets();
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
