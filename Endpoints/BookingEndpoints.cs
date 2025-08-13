// Faili: Endpoints/BookingEndpoints.cs
using Microsoft.EntityFrameworkCore;
using UmrahJourneyApi.Data;
using UmrahJourneyApi.Dtos;
using UmrahJourneyApi.Models;

namespace UmrahJourneyApi.Endpoints;

public static class BookingEndpoints
{
    public static void MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookings");

        group.MapPost("/", CreateBooking);
        // لێرەدا دەتوانین Endpointـی تری Booking زیاد بکەین لە داهاتوودا
        // group.MapGet("/{id}", GetBookingById);
    }

    private static async Task<IResult> CreateBooking(ApplicationDbContext db, BookingRequestDto bookingRequest)
    {
        // هەنگاوی ١: پشکنینی گەشتەکە
        var trip = await db.Trips.FindAsync(bookingRequest.TripId);
        if (trip == null || !trip.IsAvailable)
        {
            return Results.NotFound("This trip is not available or does not exist.");
        }

        // هەنگاوی ٢: پشکنینی پاسپۆرت
        var existingBooking = await db.Bookings.FirstOrDefaultAsync(b =>
            b.TripId == bookingRequest.TripId && b.PassportNumber == bookingRequest.PassportNumber);

        if (existingBooking != null)
        {
            return Results.Conflict("This passport is already registered for this trip.");
        }

        // هەنگاوی ٣: دۆزینەوە یان دروستکردنی بەکارهێنەر
        var user = await db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == bookingRequest.UserPhoneNumber);
        if (user == null)
        {
            user = new User
            {
                FullName = bookingRequest.UserFullName,
                PhoneNumber = bookingRequest.UserPhoneNumber
            };
            db.Users.Add(user);
        }

        // هەنگاوی ٤: دروستکردنی حجزە نوێیەکە
        var newBooking = new Booking
        {
            User = user,
            TripId = bookingRequest.TripId,
            PassengerFullName = bookingRequest.PassengerFullName,
            PassportNumber = bookingRequest.PassportNumber,
            Status = BookingStatus.Pending,
            BookingDate = DateTime.UtcNow
        };

        db.Bookings.Add(newBooking);
        await db.SaveChangesAsync();

        return Results.Created($"/api/bookings/{newBooking.Id}", new { message = "Booking created successfully!", bookingId = newBooking.Id });
    }
}
