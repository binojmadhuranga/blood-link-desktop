# Blood Donation Management System

> A simple WPF desktop application for managing donors, hospitals and blood requests using SQLite and Entity Framework Core.

## Key Features

- Admin, Donor and Hospital roles with simple authentication
- Create and manage donors and hospitals
- Hospitals can request blood from donors (single or bulk requests)
- Donors can accept/reject requests
- Lightweight SQLite database (`blooddonation.db`) with automatic seeding

## Tech Stack

- .NET 8 (WPF Desktop)
- C#
- Entity Framework Core 8 with SQLite

## Prerequisites

- Windows (WPF UI)
- .NET 8 SDK (install from https://dotnet.microsoft.com)

## Getting Started

1. Clone the repository and open the solution in Visual Studio or use the CLI:

```powershell
git clone <repo-url>
cd BloodDonationManagementSystem
dotnet restore
dotnet build
dotnet run --project BloodDonationManagementSystem\BloodDonationManagementSystem.csproj
```

2. On first run the application will create `blooddonation.db` in the application folder and seed a default admin account.

- Default Admin credentials: username: `admin`  password: `1234`

## Database

- The project uses EF Core `EnsureCreated()` and an application seeder located at `Seeders/DatabaseSeeder.cs` to create and migrate schema changes at runtime.
- Connection string: configured in `Data/AppDbContext.cs` as `Data Source=blooddonation.db`.

## Project Structure (important files)

- `BloodDonationManagementSystem/` - WPF application project
  - `App.xaml.cs` — application entry; calls the database seeder
  - `MainWindow.xaml` — single window shell for swapping views
  - `Data/AppDbContext.cs` — EF Core DbContext (SQLite)
  - `Seeders/DatabaseSeeder.cs` — creates DB and seeds default admin + schema column checks
  - `Services/` — `AuthService`, `AdminService`, `BloodRequestService` provide business logic
  - `Models/` — `User`, `Donor`, `Hospital`, `BloodRequest`, `BloodInventory`
  - `Views/` — WPF views for Admin, Donor, Hospital and auth screens

## Common Tasks

- Reset database: delete `blooddonation.db` then run the app (seeder recreates it).
- Change connection: update `UseSqlite("Data Source=blooddonation.db")` in `Data/AppDbContext.cs`.

## Development notes

- No EF Core migrations are included; the seeder attempts to add missing columns via raw SQL pragmas. For production use prefer proper migrations with `dotnet ef migrations`.
- Passwords are stored in plain text (for demo). Replace with a secure hashing mechanism before production use.

## Contributing

Feel free to open issues or pull requests. Suggestions:

- Add EF Core migrations and a migration workflow
- Replace plain-text passwords with hashed passwords
- Improve validation and error handling in UI

## License

This repository does not include a license file. Add a `LICENSE` if you intend to publish with a specific license.

---
Generated on April 29, 2026 — use this README as a starting point and edit as needed.
