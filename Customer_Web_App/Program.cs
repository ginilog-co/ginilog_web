using Customer_Web_App.Repository;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(200);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".bit.special";
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});



var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"ginilog-e3c8a-firebase-adminsdk-28ax3-07783858d2.json");

// Load service account JSON
using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

// Create Google Credential (Correct way)
var credential = ServiceAccountCredential.FromServiceAccountData(stream);

// Create Firebase App
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromServiceAccountCredential(credential)
});

// ? Register your own services
builder.Services.AddScoped<RepositoryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
