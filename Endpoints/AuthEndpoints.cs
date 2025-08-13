// Faili: Endpoints/AuthEndpoints.cs
using Microsoft.EntityFrameworkCore;
using UmrahJourneyApi.Data;
using UmrahJourneyApi.Models;

namespace UmrahJourneyApi.Endpoints;

// DTO بۆ وەرگرتنی داتای تۆمارکردن
public record RegisterRequest(string FullName, string PhoneNumber, string Password);
// DTO بۆ وەرگرتنی داتای لۆگین
public record LoginRequest(string PhoneNumber, string Password);

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        // Endpointی تۆمارکردن
        group.MapPost("/register", async (RegisterRequest request, ApplicationDbContext db) =>
        {
            // پشکنینی ئەوەی ئایا ژمارە مۆبایلەکە پێشتر تۆمارکراوە
            var existingUser = await db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (existingUser != null)
            {
                return Results.Conflict("This phone number is already registered.");
            }

            // هاشکردنی پاسۆردەکە
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = hashedPassword
            };

            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            return Results.Created($"/api/users/{newUser.Id}", new { message = "User registered successfully!" });
        });

        // Endpointی لۆگین
        group.MapPost("/login", async (LoginRequest request, ApplicationDbContext db) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);

            // ئەگەر بەکارهێنەر نەدۆزرایەوە یان پاسۆرد هەڵە بوو
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Results.Unauthorized(); // 401 Unauthorized
            }

            // لێرەدا دەتوانین Tokenـێک دروست بکەین، بەڵام بۆ ئێستا تەنها پەیامێکی سەرکەوتن دەنێرینەوە
            return Results.Ok(new { message = "Login successful!", userId = user.Id, fullName = user.FullName });
        });
    }
}
