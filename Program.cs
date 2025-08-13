// بەکارهێنانی پاکێجە پێویستەکان
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using UmrahJourneyApi.Data;
using UmrahJourneyApi.Endpoints;
using UmrahJourneyApi.Models; // ئەمە زیادکرا بۆ Trips

var builder = WebApplication.CreateBuilder(args);

// زیادکردنی خزمەتگوزارییەکان بۆ کۆنتەینەر
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
var serverVersion = new MySqlServerVersion(new Version(8, 0, 29)); // وەشانی باوی MySQL

// زیادکردنی DbContext لەگەڵ ڕێکخستنی تایبەت بۆ Railway
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion, mySqlOptions =>
    {
        // ئەمە زۆربەی کێشەکانی SSL لەگەڵ Railway چارەسەر دەکات
        mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    })
);

// دروستکردنی ئەپڵیکەیشن
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}
// ڕێکخستنی HTTP request pipeline

// هەمیشە Swagger چالاک بکە (چارەسەری هەڵەی 404)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "UmrahJourneyApi v1");

    // ئەمە وا دەکات کە کاتێک دەچیتە سەر لینکی سەرەکی، یەکسەر بچێتە سەر Swagger
    options.RoutePrefix = string.Empty;
});

// app.UseHttpsRedirection(); // ئەمە لەسەر Railway پێویست نییە و لەوانەیە کێشە دروست بکات

//============ پێناسەکردنی API Endpoints ============

// GET: /api/trips (بۆ هێنانی هەموو گەشتە بەردەستەکان)
app.MapGet("/api/trips", async (ApplicationDbContext db) =>
{
    var trips = await db.Trips.Where(t => t.IsAvailable).ToListAsync();
    return Results.Ok(trips);
});

// زیادکردنی هەموو Endpointـەکانی Booking لە فایلی جیاوازەوە
app.MapBookingEndpoints();

// زیادکردنی هەموو Endpointـەکانی Representative لە فایلی جیاوازەوە
app.MapRepresentativeEndpoints();

//====================================================

// ڕێگەدان بە پیشاندانی فایلە ستاتیکییەکانی ناو فۆڵدەری Uploads
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/Uploads"
});

app.Run();

// کارپێکردنی ئەپڵیکەیشن
app.Run();
