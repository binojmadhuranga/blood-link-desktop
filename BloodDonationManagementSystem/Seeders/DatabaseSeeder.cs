using BloodDonationManagementSystem.Data;
using BloodDonationManagementSystem.Models;

namespace BloodDonationManagementSystem.Seeders;

public static class DatabaseSeeder
{
    public static void Seed()
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();

        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Username = "admin",
                Password = "1234",
                Role = "Admin"
            });

            db.SaveChanges();
        }
    }
}