namespace HotelApp.Models;

public class BookingFacility
{
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }

    public int FacilityId { get; set; }
    public Facility? Facility { get; set; }
}
