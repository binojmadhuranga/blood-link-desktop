using BloodDonationManagementSystem.Data;
using BloodDonationManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationManagementSystem.Services;

public class BloodRequestService
{
    public const string PendingStatus = "Pending";
    public const string AcceptedStatus = "Accepted";
    public const string RejectedStatus = "Rejected";

    public IReadOnlyList<DonorOption> GetDonorOptions()
    {
        using var db = new AppDbContext();

        return db.Donors
            .AsNoTracking()
            .OrderBy(donor => donor.FullName)
            .Select(donor => new DonorOption(donor.Id, donor.FullName, donor.BloodGroup, donor.Location))
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
            Status = PendingStatus,
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

    public bool UpdateRequestStatus(int donorUserId, int requestId, string newStatus, out string error)
    {
        error = string.Empty;
        var normalizedStatus = NormalizeTargetStatus(newStatus);

        if (string.IsNullOrWhiteSpace(normalizedStatus))
        {
            error = "Invalid request status.";
            return false;
        }

        using var db = new AppDbContext();

        var donor = db.Donors.FirstOrDefault(d => d.UserId == donorUserId);
        if (donor == null)
        {
            error = "Donor profile not found for this account.";
            return false;
        }

        var request = db.BloodRequests.FirstOrDefault(r => r.Id == requestId && r.DonorId == donor.Id);
        if (request == null)
        {
            error = "Request not found for this donor.";
            return false;
        }

        if (!string.Equals(request.Status, PendingStatus, StringComparison.OrdinalIgnoreCase))
        {
            error = "Only pending requests can be updated.";
            return false;
        }

        request.Status = normalizedStatus;
        db.SaveChanges();
        return true;
    }

    private static string NormalizeTargetStatus(string? status)
    {
        var value = (status ?? string.Empty).Trim();

        if (string.Equals(value, AcceptedStatus, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "Approved", StringComparison.OrdinalIgnoreCase))
            return AcceptedStatus;

        if (string.Equals(value, RejectedStatus, StringComparison.OrdinalIgnoreCase))
            return RejectedStatus;

        return string.Empty;
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

