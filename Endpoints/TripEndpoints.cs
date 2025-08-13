using Microsoft.EntityFrameworkCore;
using UmrahJourneyApi.Data;

namespace UmrahJourneyApi.Endpoints;

public static class TripEndpoints
{
    public static void MapTripEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/trips");

        // GET /api/trips
        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            var trips = await db.Trips.Where(t => t.IsAvailable).ToListAsync();
            return Results.Ok(trips);
        });
    }
}
