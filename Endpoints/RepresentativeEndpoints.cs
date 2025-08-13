using Microsoft.EntityFrameworkCore;
using UmrahJourneyApi.Data;

namespace UmrahJourneyApi.Endpoints;

public static class RepresentativeEndpoints
{
    public static void MapRepresentativeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/representatives");

        // GET /api/representatives
        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            var representatives = await db.Representatives.Where(r => r.IsActive).ToListAsync();
            return Results.Ok(representatives);
        });
    }
}
