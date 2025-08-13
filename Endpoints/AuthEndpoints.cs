// Faili: Endpoints/AuthEndpoints.cs
using Microsoft.EntityFrameworkCore;
using UmrahJourneyApi.Data;
using UmrahJourneyApi.Models;
// using BCrypt.Net; // چیتر ئەمە پێویست نییە چونکە ناوی تەواو بەکاردەهێنین

namespace UmrahJourneyApi.Endpoints;

public record RegisterRequest(string FullName, string PhoneNumber, string Password);
public record LoginRequest(string PhoneNumber, string Password);

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (RegisterRequest request, ApplicationDbContext db) =>
        {
            var existingUser = await db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (existingUser != null)
            {
                return Results.Conflict("This phone number is already registered.");
            }

            // هاشکردنی پاسۆردەکە بە ناوی تەواو
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

        group.MapPost("/login", async (LoginRequest request, ApplicationDbContext db) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);

            // پشکنینی پاسۆرد بە ناوی تەواو
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Results.Unauthorized();
            }

            return Results.Ok(new { message = "Login successful!", userId = user.Id, fullName = user.FullName });
        });
    }
}
