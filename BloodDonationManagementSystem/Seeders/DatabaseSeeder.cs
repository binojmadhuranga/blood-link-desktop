using BloodDonationManagementSystem.Data;
using BloodDonationManagementSystem.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationManagementSystem.Seeders;

public static class DatabaseSeeder
{
    public static void Seed()
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        EnsureRegistrationColumns(db);
        EnsureBloodRequestColumns(db);
        NormalizeUsersAndProfiles(db);

        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Username = "admin",
                Password = "1234",
                Role = "Admin",
                ContactNumber = "",
                Location = ""
            });

            db.SaveChanges();
        }
    }

    private static void NormalizeUsersAndProfiles(AppDbContext db)
    {
        var users = db.Users.ToList();
        var hasChanges = false;

        foreach (var user in users)
        {
            var normalizedRole = NormalizeRole(user.Role);
            user.ContactNumber ??= string.Empty;
            user.Location ??= string.Empty;

            if (!string.Equals(user.Role, normalizedRole, StringComparison.Ordinal))
            {
                user.Role = normalizedRole;
                hasChanges = true;
            }

            if (normalizedRole == "Donor" && !db.Donors.Any(d => d.UserId == user.Id))
            {
                db.Donors.Add(new Donor
                {
                    FullName = user.Username,
                    Contact = user.ContactNumber,
                    Location = user.Location,
                    UserId = user.Id
                });
                hasChanges = true;
            }

            if (normalizedRole == "Donor")
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

            if (normalizedRole == "Hospital" && !db.Hospitals.Any(h => h.UserId == user.Id))
            {
                db.Hospitals.Add(new Hospital
                {
                    Name = user.Username,
                    ContactNumber = user.ContactNumber,
                    Location = user.Location,
                    UserId = user.Id
                });
                hasChanges = true;
            }

            if (normalizedRole == "Hospital")
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
        }

        if (hasChanges)
            db.SaveChanges();
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

    private static void EnsureBloodRequestColumns(AppDbContext db)
    {
        if (!TableExists(db, "BloodRequests"))
            return;

        AddColumnIfMissing(db, "BloodRequests", "DonorId", $"ALTER TABLE BloodRequests ADD COLUMN DonorId INTEGER NOT NULL DEFAULT 0;");
        AddColumnIfMissing(db, "BloodRequests", "Notes", $"ALTER TABLE BloodRequests ADD COLUMN Notes TEXT NOT NULL DEFAULT '';");
        AddColumnIfMissing(db, "BloodRequests", "RequestedAt", $"ALTER TABLE BloodRequests ADD COLUMN RequestedAt TEXT NOT NULL DEFAULT '';");
    }

    private static void EnsureRegistrationColumns(AppDbContext db)
    {
        if (TableExists(db, "Users"))
        {
            AddColumnIfMissing(db, "Users", "ContactNumber", $"ALTER TABLE Users ADD COLUMN ContactNumber TEXT NOT NULL DEFAULT '';");
            AddColumnIfMissing(db, "Users", "Location", $"ALTER TABLE Users ADD COLUMN Location TEXT NOT NULL DEFAULT '';");
        }

        if (TableExists(db, "Donors"))
        {
            AddColumnIfMissing(db, "Donors", "BloodGroup", $"ALTER TABLE Donors ADD COLUMN BloodGroup TEXT NOT NULL DEFAULT '';");
            AddColumnIfMissing(db, "Donors", "Contact", $"ALTER TABLE Donors ADD COLUMN Contact TEXT NOT NULL DEFAULT '';");
            AddColumnIfMissing(db, "Donors", "Location", $"ALTER TABLE Donors ADD COLUMN Location TEXT NOT NULL DEFAULT '';");
        }

        if (TableExists(db, "Hospitals"))
        {
            AddColumnIfMissing(db, "Hospitals", "ContactNumber", $"ALTER TABLE Hospitals ADD COLUMN ContactNumber TEXT NOT NULL DEFAULT '';");
            AddColumnIfMissing(db, "Hospitals", "Location", $"ALTER TABLE Hospitals ADD COLUMN Location TEXT NOT NULL DEFAULT '';");
        }
    }

    private static bool TableExists(AppDbContext db, string tableName)
    {
        db.Database.OpenConnection();
        try
        {
            using var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE type='table' AND name=$tableName;";

            var parameter = new SqliteParameter("$tableName", tableName);
            command.Parameters.Add(parameter);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
        finally
        {
            db.Database.CloseConnection();
        }
    }

    private static void AddColumnIfMissing(AppDbContext db, string tableName, string columnName, FormattableString alterSql)
    {
        db.Database.OpenConnection();
        try
        {
            using var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = $"PRAGMA table_info({tableName});";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (string.Equals(reader[1]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
                    return;
            }
        }
        finally
        {
            db.Database.CloseConnection();
        }

        db.Database.ExecuteSql(alterSql);
    }
}