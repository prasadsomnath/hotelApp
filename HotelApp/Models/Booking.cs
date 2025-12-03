using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelApp.Models;

public class Booking
{
    public int Id { get; set; }

    [Required]
    public int RoomId { get; set; }
    public Room? Room { get; set; }

    [Required]
    public int GuestId { get; set; }
    public Guest? Guest { get; set; }

    [DataType(DataType.Date)]
    public DateTime CheckIn { get; set; }

    [DataType(DataType.Date)]
    public DateTime? CheckOut { get; set; }

    public ICollection<BookingFacility> BookingFacilities { get; set; } = new List<BookingFacility>();

    [NotMapped]
    public int Nights => (int)Math.Ceiling(((CheckOut ?? DateTime.UtcNow).Date - CheckIn.Date).TotalDays);
}
