// Faili: Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace UmrahJourneyApi.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
