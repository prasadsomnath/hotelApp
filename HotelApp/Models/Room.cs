using System.ComponentModel.DataAnnotations;

namespace HotelApp.Models;

public class Room
{
    public int Id { get; set; }

    [Required, StringLength(20)]
    public string Number { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string Type { get; set; } = "Standard"; // Standard, Deluxe, Suite

    [Range(0, 20)]
    public int Capacity { get; set; } = 2;

    [Range(0, 100000)]
    public decimal RatePerNight { get; set; } = 2500m; // INR
}
