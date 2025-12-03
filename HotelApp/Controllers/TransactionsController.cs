using HotelApp.Data;
using HotelApp.Models;
using HotelApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Controllers;

public class TransactionsController : Controller
{
    private readonly ApplicationDbContext _db;
    public TransactionsController(ApplicationDbContext db) => _db = db;

    // Check-In screen
    [HttpGet]
    public async Task<IActionResult> CheckIn()
    {
        var vm = new CheckInViewModel
        {
            Rooms = await _db.Rooms.AsNoTracking().OrderBy(r => r.Number).ToListAsync(),
            Facilities = await _db.Facilities.AsNoTracking().OrderBy(f => f.Name).ToListAsync()
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> CheckIn(CheckInViewModel vm)
    {
        if (vm.CheckOut <= vm.CheckIn) ModelState.AddModelError(nameof(vm.CheckOut), "Check-out must be after check-in.");
        if (!ModelState.IsValid)
        {
            vm.Rooms = await _db.Rooms.AsNoTracking().OrderBy(r => r.Number).ToListAsync();
            vm.Facilities = await _db.Facilities.AsNoTracking().OrderBy(f => f.Name).ToListAsync();
            return View(vm);
        }

        // Ensure room availability (no overlapping active booking)
        bool occupied = await _db.Bookings.AnyAsync(b => b.RoomId == vm.RoomId &&
                                    (b.CheckOut == null || b.CheckOut > vm.CheckIn));
        if (occupied)
        {
            ModelState.AddModelError(nameof(vm.RoomId), "Selected room is currently occupied.");
            vm.Rooms = await _db.Rooms.AsNoTracking().OrderBy(r => r.Number).ToListAsync();
            vm.Facilities = await _db.Facilities.AsNoTracking().OrderBy(f => f.Name).ToListAsync();
            return View(vm);
        }

        //var guest = new Guest { FullName = vm.GuestFullName };
        var guest = new Guest
        {
            FullName = vm.GuestFullName,
            Phone = vm.Phone,
            Email = vm.Email
        };

        _db.Guests.Add(guest);
        await _db.SaveChangesAsync();

        var booking = new Booking
        {
            RoomId = vm.RoomId,
            GuestId = guest.Id,
            CheckIn = vm.CheckIn.Date,
            CheckOut = vm.CheckOut.Date
        };
        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        foreach (var fid in vm.SelectedFacilityIds.Distinct())
        {
            _db.BookingFacilities.Add(new BookingFacility { BookingId = booking.Id, FacilityId = fid });
        }
        await _db.SaveChangesAsync();

        TempData["Msg"] = $"Checked in {guest.FullName} to room #{booking.RoomId}.";
        return RedirectToAction(nameof(ActiveStays));
    }

    // List active stays
    public async Task<IActionResult> ActiveStays()
    {
        var active = await _db.Bookings.Include(b => b.Room).Include(b => b.Guest)
            .Where(b => b.CheckOut == null || b.CheckOut >= DateTime.Today)
            .OrderBy(b => b.CheckIn)
            .ToListAsync();
        return View(active);
    }

    // Check-out & bill
    [HttpGet]
    public async Task<IActionResult> CheckOut(int id)
    {
        var booking = await _db.Bookings
            .Include(b => b.Room)
            .Include(b => b.Guest)
            .Include(b => b.BookingFacilities).ThenInclude(bf => bf.Facility)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (booking == null) return NotFound();

        var nights = Math.Max(1, (int)Math.Ceiling(((booking.CheckOut ?? DateTime.Today).Date - booking.CheckIn.Date).TotalDays));
        var roomCost = booking.Room!.RatePerNight * nights;
        var facilities = booking.BookingFacilities.Select(bf => bf.Facility!).ToList();
        var facilityCost = facilities.Sum(f => f.UnitPricePerDay * nights);
        var subtotal = roomCost + facilityCost;
        var tax = Math.Round(subtotal * 0.12m, 2);
        var total = subtotal + tax;

        var vm = new CheckOutViewModel
        {
            BookingId = booking.Id,
            GuestName = booking.Guest!.FullName,
            RoomNumber = booking.Room!.Number,
            CheckIn = booking.CheckIn,
            CheckOut = (booking.CheckOut ?? DateTime.Today),
            Nights = nights,
            RoomRate = booking.Room.RatePerNight,
            Facilities = facilities.Select(f => (f.Name, f.UnitPricePerDay)).ToList(),
            Subtotal = subtotal,
            Tax = tax,
            Total = total
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmCheckOut(int id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking == null) return NotFound();
        booking.CheckOut = DateTime.Today;
        await _db.SaveChangesAsync();
        TempData["Msg"] = "Guest checked out. Bill generated.";
        return RedirectToAction(nameof(ActiveStays));
    }

    [HttpGet]
    public async Task<IActionResult> EditCheckOut(int id)
    {
        var booking = await _db.Bookings
            .Include(b => b.Guest)
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return NotFound();

        return View(booking);
    }

    [HttpPost]
    public async Task<IActionResult> EditCheckOut(Booking model)
    {
        var booking = await _db.Bookings.FindAsync(model.Id);
        if (booking == null) return NotFound();

        booking.CheckOut = model.CheckOut;

        await _db.SaveChangesAsync();

        TempData["Msg"] = "Stay extended successfully!";
        return RedirectToAction(nameof(ActiveStays));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        // Only allow deleting completed (leaved) bookings
        if (booking.CheckOut.HasValue && booking.CheckOut.Value.Date <= DateTime.Today)
        {
            _db.Bookings.Remove(booking); // BookingFacilities will cascade delete
            await _db.SaveChangesAsync();
            TempData["Msg"] = "History entry deleted.";
        }
        else
        {
            TempData["Msg"] = "Cannot delete an active stay.";
        }

        return RedirectToAction(nameof(ActiveStays));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClearHistory()
    {
        var leaved = await _db.Bookings
            .Where(b => b.CheckOut != null && b.CheckOut.Value.Date <= DateTime.Today)
            .ToListAsync();

        if (leaved.Any())
        {
            _db.Bookings.RemoveRange(leaved);
            await _db.SaveChangesAsync();
            TempData["Msg"] = $"Cleared {leaved.Count} completed booking(s).";
        }
        else
        {
            TempData["Msg"] = "No history to clear.";
        }

        return RedirectToAction(nameof(ActiveStays));
    }



}
