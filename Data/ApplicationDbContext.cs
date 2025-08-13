// Faili: Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using UmrahJourneyApi.Models;

namespace UmrahJourneyApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // پێناسەکردنی تەیبڵەکان
    public DbSet<Trip> Trips { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Representative> Representatives { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // دڵنیابوونەوە لەوەی کە ژمارەی پاسپۆرت بۆ هەر گەشتێک یەک جار بەکاردێت
        modelBuilder.Entity<Booking>()
            .HasIndex(b => new { b.TripId, b.PassportNumber })
            .IsUnique();
    }
}
