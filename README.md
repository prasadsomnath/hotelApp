# HotelApp (.NET 8 MVC + MySQL)

Implements Inventory and Transaction screens (check-in / check-out + billing).

## Prereqs
- .NET 8 SDK
- MySQL 8+ running locally (e.g., `root` user)
- Update `appsettings.json` -> `DefaultConnection` password

## Run
```bash
cd HotelApp
dotnet restore
dotnet run
```
Open http://localhost:5000 or https://localhost:5001

## Notes
- Database is created automatically (EnsureCreated) and seeded with sample rooms & facilities.
- Tax is fixed at 12% for demo.
- Room availability prevents overlapping active bookings for the same room.
