using HotelApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingFacility> BookingFacilities => Set<BookingFacility>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Room>().HasIndex(r => r.Number).IsUnique();

        modelBuilder.Entity<BookingFacility>().HasKey(bf => new { bf.BookingId, bf.FacilityId });
        modelBuilder.Entity<BookingFacility>()
            .HasOne(bf => bf.Booking).WithMany(b => b.BookingFacilities).HasForeignKey(bf => bf.BookingId);
        modelBuilder.Entity<BookingFacility>()
            .HasOne(bf => bf.Facility).WithMany().HasForeignKey(bf => bf.FacilityId);
    }
}

public static class SeedData
{
    public static async Task EnsureSeedAsync(ApplicationDbContext db)
    {
        if (!db.Rooms.Any())
        {
            db.Rooms.AddRange(new[] {
                new Room { Number = "101", Type = "Standard", Capacity = 2, RatePerNight = 2500 },
                new Room { Number = "102", Type = "Deluxe", Capacity = 3, RatePerNight = 4000 },
                new Room { Number = "201", Type = "Suite", Capacity = 4, RatePerNight = 7500 }
            });
        }
        if (!db.Facilities.Any())
        {
            db.Facilities.AddRange(new[] {
                new Facility { Name = "Breakfast Buffet", Description = "Daily breakfast", UnitPricePerDay = 500 },
                new Facility { Name = "Gym Access", Description = "Unlimited access", UnitPricePerDay = 200 },
                new Facility { Name = "Swimming Pool", Description = "Daily pool access", UnitPricePerDay = 300 }
            });
        }
        await db.SaveChangesAsync();
    }
}
