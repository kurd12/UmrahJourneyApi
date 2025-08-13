using Microsoft.EntityFrameworkCore;
using UmrahJourneyApi.Data;
using UmrahJourneyApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// دروستکردنی Connection String بە شێوەیەکی دینامیکی
var host = builder.Configuration["DB_HOST"];
var port = builder.Configuration["DB_PORT"];
var user = builder.Configuration["DB_USER"];
var password = builder.Configuration["DB_PASSWORD"];
var database = builder.Configuration["DB_DATABASE"];

// ئەگەر گۆڕاوەکان بوونیان هەبوو (واتە لەسەر Railway کاردەکەین)، ئەوا Connection Stringـی Railway بەکاربهێنە
// ئەگینا، Connection Stringـی ناو appsettings.json (بۆ کۆمپیوتەری خۆمان) بەکاربهێنە
var connectionString = !string.IsNullOrEmpty(host)
    ? $"Server={host};Port={port};Database={database};User={user};Password={password};"
    : builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
app.MapGet("/api/trips", async (ApplicationDbContext db) =>
{
    var trips = await db.Trips.Where(t => t.IsAvailable).ToListAsync();
    return Results.Ok(trips);
});
app.MapBookingEndpoints();
// زیادکردنی هەموو Endpointـەکانی Representative
app.MapRepresentativeEndpoints(); // <-- ئەمە زیادکرا
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
