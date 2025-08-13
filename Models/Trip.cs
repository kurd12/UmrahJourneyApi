// Faili: Models/Trip.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UmrahJourneyApi.Models;

public enum TripType
{
    Land,
    Air
}

public class Trip
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public TripType TripType { get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    public string? HotelName { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime DepartureDate { get; set; }
}
