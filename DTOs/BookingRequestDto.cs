// Faili: DTOs/BookingRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace UmrahJourneyApi.Dtos;

public class BookingRequestDto
{
    [Required]
    public string UserFullName { get; set; } = string.Empty;

    [Required]
    public string UserPhoneNumber { get; set; } = string.Empty;

    [Required]
    public int TripId { get; set; }

    [Required]
    public string PassengerFullName { get; set; } = string.Empty;

    [Required]
    public string PassportNumber { get; set; } = string.Empty;
}
