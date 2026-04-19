using BloodDonationManagementSystem.Data;
using BloodDonationManagementSystem.Models;

namespace BloodDonationManagementSystem.Services;

public class AuthService
{
    public User? Login(string username, string password)
    {
        using var db = new AppDbContext();

        return db.Users.FirstOrDefault(u =>
            u.Username == username && u.Password == password);
    }

    public bool Register(string username, string password, string role)
    {
        using var db = new AppDbContext();

        if (db.Users.Any(u => u.Username == username))
            return false;

        var user = new User
        {
            Username = username,
            Password = password,
            Role = role
        };

        db.Users.Add(user);
        db.SaveChanges();

        if (role == "Donor")
        {
            db.Donors.Add(new Donor
            {
                FullName = username,
                UserId = user.Id
            });
        }

        if (role == "Hospital")
        {
            db.Hospitals.Add(new Hospital
            {
                Name = username,
                UserId = user.Id
            });
        }

        db.SaveChanges();
        return true;
    }
}