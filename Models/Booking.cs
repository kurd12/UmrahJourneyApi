// Faili: Models/Booking.cs
using System.ComponentModel.DataAnnotations;

namespace UmrahJourneyApi.Models;

public enum BookingStatus
{
    Pending,
    Approved,
    Paid,
    Completed,
    Rejected
}

public class Booking
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public User? User { get; set; }

    [Required]
    public int TripId { get; set; }
    public Trip? Trip { get; set; }

    [Required]
    public string PassengerFullName { get; set; } = string.Empty;

    [Required]
    public string PassportNumber { get; set; } = string.Empty;

    public string? PassportPhotoUrl { get; set; }
    public string? PersonalPhotoUrl { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
}
