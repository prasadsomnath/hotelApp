using System.ComponentModel.DataAnnotations;

namespace HotelApp.Models;

public class Guest
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, Phone, StringLength(20)]
    [RegularExpression(@"^[1-9]\d{9}$", ErrorMessage = "Phone must be 10 digits and cannot start with 0")]
    public string? Phone { get; set; }

    [Required, EmailAddress, StringLength(100)]
    [RegularExpression(@"^.*@.*\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid domain")]
    public string? Email { get; set; }
}
