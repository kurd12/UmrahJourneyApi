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

        group.MapPost("/", CreateBooking).DisableAntiforgery();
        // لێرەدا دەتوانین Endpointـی تری Booking زیاد بکەین لە داهاتوودا
        // group.MapGet("/{id}", GetBookingById);
    }

    // فەنکشنی نوێکراوە بۆ وەرگرتنی فۆڕم و فایل
    private static async Task<IResult> CreateBooking(HttpContext context, ApplicationDbContext db)
    {
        // وەرگرتنی داتا لە فۆڕمەکە
        var form = await context.Request.ReadFormAsync();
        var tripId = int.Parse(form["TripId"]);
        var passengerFullName = form["PassengerFullName"];
        var passportNumber = form["PassportNumber"];
        var userFullName = form["UserFullName"];
        var userPhoneNumber = form["UserPhoneNumber"];

        // وەرگرتنی فایلەکان (وێنەکان)
        var passportPhoto = form.Files.GetFile("passportPhoto");
        var personalPhoto = form.Files.GetFile("personalPhoto");

        // پشکنینی گەشت و پاسپۆرت (ئەم بەشە وەک خۆیەتی)
        var trip = await db.Trips.FindAsync(tripId);
        if (trip == null || !trip.IsAvailable)
        {
            return Results.NotFound("This trip is not available or does not exist.");
        }
        var existingBooking = await db.Bookings.FirstOrDefaultAsync(b =>
            b.TripId == tripId && b.PassportNumber == passportNumber);
        if (existingBooking != null)
        {
            return Results.Conflict("This passport is already registered for this trip.");
        }

        // پاشەکەوتکردنی وێنەکان و وەرگرتنی URLـەکانیان
        var passportPhotoUrl = await SaveFile(passportPhoto, context.Request.Host.Value);
        var personalPhotoUrl = await SaveFile(personalPhoto, context.Request.Host.Value);

        // دۆزینەوە یان دروستکردنی بەکارهێنەر (ئەم بەشە وەک خۆیەتی)
        var user = await db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == userPhoneNumber);
        if (user == null)
        {
            user = new User { FullName = userFullName, PhoneNumber = userPhoneNumber };
            db.Users.Add(user);
        }

        // دروستکردنی حجزە نوێیەکە لەگەڵ لینکی وێنەکان
        var newBooking = new Booking
        {
            User = user,
            TripId = tripId,
            PassengerFullName = passengerFullName,
            PassportNumber = passportNumber,
            PassportPhotoUrl = passportPhotoUrl, // زیادکرا
            PersonalPhotoUrl = personalPhotoUrl, // زیادکرا
            Status = BookingStatus.Pending,
            BookingDate = DateTime.UtcNow
        };

        db.Bookings.Add(newBooking);
        await db.SaveChangesAsync();

        return Results.Created($"/api/bookings/{newBooking.Id}", new { message = "Booking created successfully!", bookingId = newBooking.Id });
    }
    // ... (کۆدی CreateBooking لێرە تەواو دەبێت)

    // فەنکشنی هاوبەش بۆ پاشەکەوتکردنی فایل
    private static async Task<string> SaveFile(IFormFile file, string host)
    {
        if (file == null || file.Length == 0)
            return null;

        var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        if (!Directory.Exists(uploadsFolderPath))
        {
            Directory.CreateDirectory(uploadsFolderPath);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"https://{host}/Uploads/{fileName}";
        return fileUrl;
    }

    // } <-- دواین کەوانەی کلاسەکە لێرەیە

}
