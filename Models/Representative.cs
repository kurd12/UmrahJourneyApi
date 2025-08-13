// Faili: Models/Representative.cs
using System.ComponentModel.DataAnnotations;

namespace UmrahJourneyApi.Models;

public class Representative
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; } = true;
}
