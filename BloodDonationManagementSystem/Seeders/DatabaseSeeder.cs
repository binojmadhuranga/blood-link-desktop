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
        EnsureBloodRequestColumns(db);

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

    private static void EnsureBloodRequestColumns(AppDbContext db)
    {
        if (!TableExists(db, "BloodRequests"))
            return;

        AddColumnIfMissing(db, "BloodRequests", "DonorId", $"ALTER TABLE BloodRequests ADD COLUMN DonorId INTEGER NOT NULL DEFAULT 0;");
        AddColumnIfMissing(db, "BloodRequests", "Notes", $"ALTER TABLE BloodRequests ADD COLUMN Notes TEXT NOT NULL DEFAULT '';");
        AddColumnIfMissing(db, "BloodRequests", "RequestedAt", $"ALTER TABLE BloodRequests ADD COLUMN RequestedAt TEXT NOT NULL DEFAULT '';");
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