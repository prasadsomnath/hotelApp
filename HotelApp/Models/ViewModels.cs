using System.ComponentModel.DataAnnotations;
using HotelApp.Models;

namespace HotelApp.ViewModels;

//public class CheckInViewModel
//{
//    [Required]
//    public int RoomId { get; set; }

//    [Required, StringLength(100)]
//    public string GuestFullName { get; set; } = string.Empty;

//    [DataType(DataType.Date)]
//    public DateTime CheckIn { get; set; } = DateTime.Today;

//    [DataType(DataType.Date)]
//    public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(1);

//    public List<int> SelectedFacilityIds { get; set; } = new();
//    public IEnumerable<HotelApp.Models.Room>? Rooms { get; set; }
//    public IEnumerable<HotelApp.Models.Facility>? Facilities { get; set; }
//}
public class CheckInViewModel
{
    [Required]
    public int RoomId { get; set; }

    [Required, StringLength(100)]
    public string GuestFullName { get; set; } = string.Empty;

    [Required,Phone, StringLength(20)]
    public string? Phone { get; set; }

    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }

    [DataType(DataType.Date)]
    public DateTime CheckIn { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(1);

    public List<int> SelectedFacilityIds { get; set; } = new();
    public IEnumerable<Room>? Rooms { get; set; }
    public IEnumerable<Facility>? Facilities { get; set; }
}


public class CheckOutViewModel
{
    public int BookingId { get; set; }
    public string GuestName { get; set; } = "";
    public string RoomNumber { get; set; } = "";
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Nights { get; set; }
    public decimal RoomRate { get; set; }
    public List<(string name, decimal perDay)> Facilities { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
}
