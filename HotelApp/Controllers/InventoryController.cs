using HotelApp.Data;
using HotelApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Controllers;

public class InventoryController : Controller
{
    private readonly ApplicationDbContext _db;
    public InventoryController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewBag.Rooms = await _db.Rooms.AsNoTracking().OrderBy(r => r.Number).ToListAsync();
        ViewBag.Facilities = await _db.Facilities.AsNoTracking().OrderBy(f => f.Name).ToListAsync();
        return View();
    }

    // Rooms
    [HttpGet]
    public IActionResult CreateRoom() => View(new Room());

  
    [HttpPost]
    public async Task<IActionResult> CreateRoom(Room model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Check if the room number already exists
        bool exists = await _db.Rooms.AnyAsync(r => r.Number == model.Number);

        if (exists)
        {
            ModelState.AddModelError("Number", "Room number already exists. Please choose a different number.");
            return View(model);
        }

        _db.Rooms.Add(model);
        await _db.SaveChangesAsync();

        TempData["Msg"] = "Room added successfully!";
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> EditRoom(int id)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room == null) return NotFound();
        return View(room);
    }

    [HttpPost]
    public async Task<IActionResult> EditRoom(Room model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.Rooms.Update(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room != null) { _db.Rooms.Remove(room); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    // Facilities
    [HttpGet]
    public IActionResult CreateFacility() => View(new Facility());

    [HttpPost]
    public async Task<IActionResult> CreateFacility(Facility model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.Facilities.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EditFacility(int id)
    {
        var facility = await _db.Facilities.FindAsync(id);
        if (facility == null) return NotFound();
        return View(facility);
    }

    [HttpPost]
    public async Task<IActionResult> EditFacility(Facility model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.Facilities.Update(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteFacility(int id)
    {
        var f = await _db.Facilities.FindAsync(id);
        if (f != null) { _db.Facilities.Remove(f); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
