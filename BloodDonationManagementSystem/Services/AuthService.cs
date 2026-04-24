using BloodDonationManagementSystem.Data;
using BloodDonationManagementSystem.Models;

namespace BloodDonationManagementSystem.Services;

public class AuthService
{
    public User? Login(string username, string password)
    {
        using var db = new AppDbContext();

        var normalizedUsername = NormalizeUsernameForLookup(username);
        var normalizedPassword = (password ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(normalizedUsername) || string.IsNullOrWhiteSpace(normalizedPassword))
            return null;

        var user = db.Users.FirstOrDefault(u =>
            u.Username.ToLower() == normalizedUsername && u.Password == normalizedPassword);

        if (user == null)
            return null;

        var normalizedRole = NormalizeRole(user.Role);
        var hasChanges = normalizedRole != user.Role;
        user.Role = normalizedRole;

        hasChanges |= EnsureRoleProfile(db, user);

        if (hasChanges)
            db.SaveChanges();

        return user;
    }

    public bool Register(string username, string password, string role)
    {
        using var db = new AppDbContext();

        var normalizedUsername = (username ?? string.Empty).Trim();
        var normalizedPassword = (password ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(normalizedUsername) || string.IsNullOrWhiteSpace(normalizedPassword))
            return false;

        var lookupUsername = NormalizeUsernameForLookup(normalizedUsername);

        if (db.Users.Any(u => u.Username.ToLower() == lookupUsername))
            return false;

        var normalizedRole = NormalizeRole(role);

        var user = new User
        {
            Username = normalizedUsername,
            Password = normalizedPassword,
            Role = normalizedRole
        };

        db.Users.Add(user);
        db.SaveChanges();

        if (normalizedRole == "Donor")
        {
            db.Donors.Add(new Donor
            {
                FullName = normalizedUsername,
                UserId = user.Id
            });
        }

        if (normalizedRole == "Hospital")
        {
            db.Hospitals.Add(new Hospital
            {
                Name = normalizedUsername,
                UserId = user.Id
            });
        }

        db.SaveChanges();
        return true;
    }

    private static string NormalizeRole(string? role)
    {
        var value = (role ?? string.Empty).Trim();
        var key = new string(value.Where(char.IsLetter).ToArray()).ToLowerInvariant();

        if (key is "donor" or "donors")
            return "Donor";

        if (key is "hospital" or "hospitals" or "hostpital" or "hospitial" or "hospitral" or "hostpitral")
            return "Hospital";

        if (key is "admin" or "administrator")
            return "Admin";

        return value;
    }

    private static string NormalizeUsernameForLookup(string? username)
    {
        return (username ?? string.Empty).Trim().ToLower();
    }

    private static bool EnsureRoleProfile(AppDbContext db, User user)
    {
        if (user.Role == "Donor" && !db.Donors.Any(d => d.UserId == user.Id))
        {
            db.Donors.Add(new Donor
            {
                FullName = user.Username,
                UserId = user.Id
            });
            return true;
        }

        if (user.Role == "Hospital" && !db.Hospitals.Any(h => h.UserId == user.Id))
        {
            db.Hospitals.Add(new Hospital
            {
                Name = user.Username,
                UserId = user.Id
            });
            return true;
        }

        return false;
    }
}