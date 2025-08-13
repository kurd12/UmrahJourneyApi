// بەکارهێنانی پاکێجە پێویستەکان
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// دروستکردنی ئەپڵیکەیشن
var app = builder.Build();

// ڕێکخستنی HTTP request pipeline

// هەمیشە Swagger چالاک بکە (چارەسەری هەڵەی 404)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
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

// کارپێکردنی ئەپڵیکەیشن
app.Run();
