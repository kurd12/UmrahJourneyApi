// بەکارهێنانی پاکێجە پێویستەکان
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using UmrahJourneyApi.Data;
using UmrahJourneyApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// =================== زیادکردنی خزمەتگوزارییەکان (Services) ===================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// دروستکردنی Connection String بە شێوەیەکی دینامیکی
var host = builder.Configuration["DB_HOST"];
var port = builder.Configuration["DB_PORT"];
var user = builder.Configuration["DB_USER"];
var password = builder.Configuration["DB_PASSWORD"];
var database = builder.Configuration["DB_DATABASE"];

var connectionString = !string.IsNullOrEmpty(host)
    ? $"Server={host};Port={port};Database={database};User={user};Password={password};"
    : builder.Configuration.GetConnectionString("DefaultConnection");

// دروستکردنی وەشانی سێرڤەر بە شێوەیەکی دەستی
var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

// زیادکردنی DbContext لەگەڵ ڕێکخستنی تایبەت بۆ Railway
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion, mySqlOptions =>
    {
        mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    })
);
// ==========================================================================


var app = builder.Build();


// =================== ڕێکخستنی HTTP Request Pipeline ===================

// بەشی یەکەم: جێبەجێکردنی Migrationـەکان لە کاتی کارپێکردندا
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// بەشی دووەم: ڕێکخستنی Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "UmrahJourneyApi v1");
    options.RoutePrefix = string.Empty;
});

// بەشی سێیەم: دروستکردنی فۆڵدەری Uploads و ڕێگەدان بە بەکارهێنانی
var uploadsFolderPath = Path.Combine(builder.Environment.ContentRootPath, "Uploads");
if (!Directory.Exists(uploadsFolderPath))
{
    Directory.CreateDirectory(uploadsFolderPath);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsFolderPath),
    RequestPath = "/Uploads"
});

// بەشی چوارەم: پێناسەکردنی هەموو API Endpoints
app.MapTripEndpoints();
app.MapBookingEndpoints();
app.MapRepresentativeEndpoints();
app.MapAuthEndpoints(); 

// ==========================================================================


// کارپێکردنی ئەپڵیکەیشن (تەنها یەک جار)
app.Run();
