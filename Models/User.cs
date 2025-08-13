// Faili: Models/User.cs
using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }
    public string? FullName { get; set; }

    [Required] // دڵنیادەبینەوە کە ئەمە بەتاڵ نییە
    public string? PhoneNumber { get; set; }

    // =================== ئەم دوو دێڕە زیادکرا ===================
    [Required]
    public string? PasswordHash { get; set; } // بۆ پاشەکەوتکردنی پاسۆردی هاشکراو
    // ==========================================================
}
