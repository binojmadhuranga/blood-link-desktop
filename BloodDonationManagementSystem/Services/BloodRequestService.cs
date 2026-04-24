using BloodDonationManagementSystem.Data;
using BloodDonationManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationManagementSystem.Services;

public class BloodRequestService
{
    public IReadOnlyList<DonorOption> GetDonorOptions()
    {
        using var db = new AppDbContext();

        return db.Donors
            .Select(donor => new DonorOption(donor.Id, donor.FullName, donor.BloodGroup, donor.Location))
            .OrderBy(donor => donor.FullName)
            .ToList();
    }

    public bool CreateRequest(int hospitalUserId, int donorId, string bloodGroup, int quantity, string notes, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(bloodGroup))
        {
            error = "Blood group is required.";
            return false;
        }

        if (quantity <= 0)
        {
            error = "Quantity must be greater than zero.";
            return false;
        }

        using var db = new AppDbContext();

        var hospital = db.Hospitals.FirstOrDefault(h => h.UserId == hospitalUserId);
        if (hospital == null)
        {
            error = "Hospital profile not found for this user.";
            return false;
        }

        var donor = db.Donors.FirstOrDefault(d => d.Id == donorId);
        if (donor == null)
        {
            error = "Selected donor was not found.";
            return false;
        }

        var request = new BloodRequest
        {
            HospitalId = hospital.Id,
            DonorId = donor.Id,
            BloodGroup = bloodGroup.Trim(),
            Quantity = quantity,
            Notes = notes.Trim(),
            Status = "Pending",
            RequestedAt = DateTime.UtcNow
        };

        db.BloodRequests.Add(request);
        db.SaveChanges();

        return true;
    }

    public IReadOnlyList<HospitalRequestItem> GetHospitalRequests(int hospitalUserId)
    {
        using var db = new AppDbContext();

        return db.BloodRequests
            .AsNoTracking()
            .Where(request => request.Hospital != null && request.Hospital.UserId == hospitalUserId)
            .OrderByDescending(request => request.RequestedAt)
            .Select(request => new HospitalRequestItem(
                request.Id,
                request.BloodGroup,
                request.Quantity,
                request.Status,
                request.Donor != null ? request.Donor.FullName : "Unknown",
                request.RequestedAt,
                request.Notes))
            .ToList();
    }

    public IReadOnlyList<DonorRequestItem> GetDonorRequests(int donorUserId)
    {
        using var db = new AppDbContext();

        return db.BloodRequests
            .AsNoTracking()
            .Where(request => request.Donor != null && request.Donor.UserId == donorUserId)
            .OrderByDescending(request => request.RequestedAt)
            .Select(request => new DonorRequestItem(
                request.Id,
                request.BloodGroup,
                request.Quantity,
                request.Status,
                request.Hospital != null ? request.Hospital.Name : "Unknown",
                request.RequestedAt,
                request.Notes))
            .ToList();
    }
}

public record DonorOption(int Id, string FullName, string BloodGroup, string Location)
{
    public string DisplayName => string.IsNullOrWhiteSpace(BloodGroup)
        ? FullName
        : $"{FullName} ({BloodGroup})";
}

public record HospitalRequestItem(
    int Id,
    string BloodGroup,
    int Quantity,
    string Status,
    string DonorName,
    DateTime RequestedAt,
    string Notes);

public record DonorRequestItem(
    int Id,
    string BloodGroup,
    int Quantity,
    string Status,
    string HospitalName,
    DateTime RequestedAt,
    string Notes);

