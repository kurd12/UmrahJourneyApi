// Faili: Endpoints/UserEndpoints.cs
using Microsoft.EntityFrameworkCore;
using UmrahJourneyApi.Data;

namespace UmrahJourneyApi.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");

        // GET /api/users/{userId}/active-booking
        group.MapGet("/{userId}/active-booking", async (int userId, ApplicationDbContext db) =>
        {
            // گەڕان بەدوای حجزێکدا کە هی ئەم بەکارهێنەرە بێت و هێشتا تەواو نەبووبێت
            var activeBooking = await db.Bookings
                .Include(b => b.Trip) // زانیارییەکانی گەشتەکەشی لەگەڵ بهێنە
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Status != Models.BookingStatus.Completed);

            if (activeBooking == null)
            {
                return Results.NotFound("No active booking found for this user.");
            }

            // گەڕاندنەوەی زانیارییە گرنگەکان
            return Results.Ok(new
            {
                TripTitle = activeBooking.Trip.Title,
                HotelName = activeBooking.Trip.HotelName,
                DepartureDate = activeBooking.Trip.DepartureDate
                // لێرەدا دەتوانین زانیاریی زیاتر بگەڕێنینەوە
            });
        });
    }
}
