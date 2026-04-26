using BloodDonationManagementSystem.Data;
using BloodDonationManagementSystem.Models;

namespace BloodDonationManagementSystem.Services;

public class AuthService
{
    public User? Login(string username, string password)
    {
        using var db = new AppDbContext();

        var normalizedUsername = NormalizeUsernameForLookup(username);
        var normalizedPassword = password.Trim();

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

    public bool Register(string username, string password, string role, string contactNumber, string location, string bloodGroup)
    {
        using var db = new AppDbContext();

        var normalizedUsername = username.Trim();
        var normalizedPassword = password.Trim();
        var normalizedContactNumber = contactNumber.Trim();
        var normalizedLocation = location.Trim();
        var normalizedBloodGroup = bloodGroup.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalizedUsername) ||
            string.IsNullOrWhiteSpace(normalizedPassword) ||
            string.IsNullOrWhiteSpace(normalizedContactNumber) ||
            string.IsNullOrWhiteSpace(normalizedLocation))
            return false;

        var lookupUsername = NormalizeUsernameForLookup(normalizedUsername);

        if (db.Users.Any(u => u.Username.ToLower() == lookupUsername))
            return false;

        var normalizedRole = NormalizeRole(role);
        if (!IsSupportedRegistrationRole(normalizedRole))
            return false;

        if (normalizedRole == "Donor" && string.IsNullOrWhiteSpace(normalizedBloodGroup))
            return false;

        var user = new User
        {
            Username = normalizedUsername,
            Password = normalizedPassword,
            Role = normalizedRole,
            ContactNumber = normalizedContactNumber,
            Location = normalizedLocation
        };

        db.Users.Add(user);
        db.SaveChanges();

        if (normalizedRole == "Donor")
        {
            db.Donors.Add(new Donor
            {
                FullName = normalizedUsername,
                BloodGroup = normalizedBloodGroup,
                Contact = normalizedContactNumber,
                Location = normalizedLocation,
                UserId = user.Id
            });
        }

        if (normalizedRole == "Hospital")
        {
            db.Hospitals.Add(new Hospital
            {
                Name = normalizedUsername,
                ContactNumber = normalizedContactNumber,
                Location = normalizedLocation,
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

    private static bool IsSupportedRegistrationRole(string role)
    {
        return role == "Donor" || role == "Hospital" || role == "Admin";
    }

    private static bool EnsureRoleProfile(AppDbContext db, User user)
    {
        var hasChanges = false;

        user.ContactNumber ??= string.Empty;
        user.Location ??= string.Empty;

        if (user.Role == "Donor" && !db.Donors.Any(d => d.UserId == user.Id))
        {
            db.Donors.Add(new Donor
            {
                FullName = user.Username,
                Contact = user.ContactNumber,
                Location = user.Location,
                UserId = user.Id
            });
            return true;
        }

        if (user.Role == "Donor")
        {
            var donor = db.Donors.FirstOrDefault(d => d.UserId == user.Id);
            if (donor != null)
            {
                donor.Contact ??= string.Empty;
                donor.Location ??= string.Empty;

                if (string.IsNullOrWhiteSpace(donor.Contact) && !string.IsNullOrWhiteSpace(user.ContactNumber))
                {
                    donor.Contact = user.ContactNumber;
                    hasChanges = true;
                }

                if (string.IsNullOrWhiteSpace(donor.Location) && !string.IsNullOrWhiteSpace(user.Location))
                {
                    donor.Location = user.Location;
                    hasChanges = true;
                }
            }
        }

        if (user.Role == "Hospital" && !db.Hospitals.Any(h => h.UserId == user.Id))
        {
            db.Hospitals.Add(new Hospital
            {
                Name = user.Username,
                ContactNumber = user.ContactNumber,
                Location = user.Location,
                UserId = user.Id
            });
            return true;
        }

        if (user.Role == "Hospital")
        {
            var hospital = db.Hospitals.FirstOrDefault(h => h.UserId == user.Id);
            if (hospital != null)
            {
                hospital.ContactNumber ??= string.Empty;
                hospital.Location ??= string.Empty;

                if (string.IsNullOrWhiteSpace(hospital.ContactNumber) && !string.IsNullOrWhiteSpace(user.ContactNumber))
                {
                    hospital.ContactNumber = user.ContactNumber;
                    hasChanges = true;
                }

                if (string.IsNullOrWhiteSpace(hospital.Location) && !string.IsNullOrWhiteSpace(user.Location))
                {
                    hospital.Location = user.Location;
                    hasChanges = true;
                }
            }
        }

        return hasChanges;
    }
}