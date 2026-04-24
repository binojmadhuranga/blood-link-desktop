using Microsoft.EntityFrameworkCore;
using BloodDonationManagementSystem.Models;

namespace BloodDonationManagementSystem.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Donor> Donors { get; set; }
    public DbSet<Hospital> Hospitals { get; set; }
    public DbSet<BloodRequest> BloodRequests { get; set; }
    public DbSet<BloodInventory> BloodInventories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=blooddonation.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BloodRequest>()
            .HasOne(request => request.Hospital)
            .WithMany(hospital => hospital.BloodRequests)
            .HasForeignKey(request => request.HospitalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BloodRequest>()
            .HasOne(request => request.Donor)
            .WithMany(donor => donor.BloodRequests)
            .HasForeignKey(request => request.DonorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BloodRequest>()
            .HasIndex(request => request.HospitalId);

        modelBuilder.Entity<BloodRequest>()
            .HasIndex(request => request.DonorId);
    }
}