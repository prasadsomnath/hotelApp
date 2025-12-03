using System.ComponentModel.DataAnnotations;

namespace HotelApp.Models;

public class Facility
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Description { get; set; }

    [Range(0, 100000)]
    public decimal UnitPricePerDay { get; set; } = 0m; // Charged per day
}
