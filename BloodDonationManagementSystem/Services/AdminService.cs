using BloodDonationManagementSystem.Data;
using BloodDonationManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationManagementSystem.Services;

public class AdminService
{
    // ============ Donor Management ============

    public IReadOnlyList<AdminDonorItem> GetAllDonors()
    {
        using var db = new AppDbContext();

        return db.Donors
            .AsNoTracking()
            .Include(d => d.BloodRequests)
            .OrderByDescending(d => d.Id)
            .Select(donor => new AdminDonorItem(
                donor.Id,
                donor.FullName,
                donor.BloodGroup,
                donor.Contact,
                donor.Location,
                donor.LastDonationDate,
                donor.BloodRequests.Count))
            .ToList();
    }

    public AdminDonorDetail? GetDonorById(int donorId)
    {
        using var db = new AppDbContext();

        var donor = db.Donors
            .AsNoTracking()
            .Include(d => d.BloodRequests)
            .FirstOrDefault(d => d.Id == donorId);

        if (donor == null)
            return null;

        return new AdminDonorDetail(
            donor.Id,
            donor.FullName,
            donor.BloodGroup,
            donor.Contact,
            donor.Location,
            donor.LastDonationDate,
            donor.UserId,
            donor.BloodRequests.Count,
            donor.BloodRequests
                .OrderByDescending(r => r.RequestedAt)
                .Select(r => new DonorRequestSummary(
                    r.Id,
                    r.Hospital?.Name ?? "Unknown",
                    r.BloodGroup,
                    r.Quantity,
                    r.Status,
                    r.RequestedAt))
                .ToList());
    }

    public bool UpdateDonor(int donorId, string fullName, string bloodGroup, string contact, string location, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(fullName))
        {
            error = "Full name is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(contact))
        {
            error = "Contact number is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(location))
        {
            error = "Location is required.";
            return false;
        }

        using var db = new AppDbContext();

        var donor = db.Donors.FirstOrDefault(d => d.Id == donorId);
        if (donor == null)
        {
            error = "Donor not found.";
            return false;
        }

        donor.FullName = fullName.Trim();
        donor.BloodGroup = bloodGroup?.Trim() ?? "";
        donor.Contact = contact.Trim();
        donor.Location = location.Trim();

        try
        {
            db.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            error = $"An error occurred while updating the donor: {ex.Message}";
            return false;
        }
    }

    public bool DeleteDonor(int donorId, out string error)
    {
        error = string.Empty;

        using var db = new AppDbContext();

        var donor = db.Donors
            .Include(d => d.BloodRequests)
            .FirstOrDefault(d => d.Id == donorId);

        if (donor == null)
        {
            error = "Donor not found.";
            return false;
        }

        if (donor.BloodRequests.Any())
        {
            error = "Cannot delete donor with pending or completed blood requests.";
            return false;
        }

        try
        {
            db.Donors.Remove(donor);
            db.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            error = $"An error occurred while deleting the donor: {ex.Message}";
            return false;
        }
    }

    // ============ Hospital Management ============

    public IReadOnlyList<AdminHospitalItem> GetAllHospitals()
    {
        using var db = new AppDbContext();

        return db.Hospitals
            .AsNoTracking()
            .Include(h => h.BloodRequests)
            .OrderByDescending(h => h.Id)
            .Select(hospital => new AdminHospitalItem(
                hospital.Id,
                hospital.Name,
                hospital.Location,
                hospital.ContactNumber,
                hospital.BloodRequests.Count))
            .ToList();
    }

    public AdminHospitalDetail? GetHospitalById(int hospitalId)
    {
        using var db = new AppDbContext();

        var hospital = db.Hospitals
            .AsNoTracking()
            .Include(h => h.BloodRequests)
            .FirstOrDefault(h => h.Id == hospitalId);

        if (hospital == null)
            return null;

        return new AdminHospitalDetail(
            hospital.Id,
            hospital.Name,
            hospital.Location,
            hospital.ContactNumber,
            hospital.UserId,
            hospital.BloodRequests.Count,
            hospital.BloodRequests
                .OrderByDescending(r => r.RequestedAt)
                .Select(r => new HospitalRequestSummary(
                    r.Id,
                    r.Donor?.FullName ?? "Unknown",
                    r.BloodGroup,
                    r.Quantity,
                    r.Status,
                    r.RequestedAt))
                .ToList());
    }

    public bool UpdateHospital(int hospitalId, string name, string location, string contactNumber, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            error = "Hospital name is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(location))
        {
            error = "Location is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(contactNumber))
        {
            error = "Contact number is required.";
            return false;
        }

        using var db = new AppDbContext();

        var hospital = db.Hospitals.FirstOrDefault(h => h.Id == hospitalId);
        if (hospital == null)
        {
            error = "Hospital not found.";
            return false;
        }

        hospital.Name = name.Trim();
        hospital.Location = location.Trim();
        hospital.ContactNumber = contactNumber.Trim();

        try
        {
            db.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            error = $"An error occurred while updating the hospital: {ex.Message}";
            return false;
        }
    }

    public bool DeleteHospital(int hospitalId, out string error)
    {
        error = string.Empty;

        using var db = new AppDbContext();

        var hospital = db.Hospitals
            .Include(h => h.BloodRequests)
            .FirstOrDefault(h => h.Id == hospitalId);

        if (hospital == null)
        {
            error = "Hospital not found.";
            return false;
        }

        if (hospital.BloodRequests.Any())
        {
            error = "Cannot delete hospital with blood requests.";
            return false;
        }

        try
        {
            db.Hospitals.Remove(hospital);
            db.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            error = $"An error occurred while deleting the hospital: {ex.Message}";
            return false;
        }
    }

    // ============ Statistics ============

    public AdminStatistics GetStatistics()
    {
        using var db = new AppDbContext();

        var totalUsers = db.Users.Count();
        var totalDonors = db.Donors.Count();
        var totalHospitals = db.Hospitals.Count();
        var totalRequests = db.BloodRequests.Count();
        var pendingRequests = db.BloodRequests.Count(r => r.Status == "Pending");
        var acceptedRequests = db.BloodRequests.Count(r => r.Status == "Accepted");
        var rejectedRequests = db.BloodRequests.Count(r => r.Status == "Rejected");

        return new AdminStatistics(totalUsers, totalDonors, totalHospitals, totalRequests, pendingRequests, acceptedRequests, rejectedRequests);
    }
}

// ============ Data Models ============

public record AdminDonorItem(
    int Id,
    string FullName,
    string BloodGroup,
    string Contact,
    string Location,
    DateTime LastDonationDate,
    int RequestCount);

public record AdminDonorDetail(
    int Id,
    string FullName,
    string BloodGroup,
    string Contact,
    string Location,
    DateTime LastDonationDate,
    int UserId,
    int RequestCount,
    IReadOnlyList<DonorRequestSummary> Requests);

public record DonorRequestSummary(
    int Id,
    string HospitalName,
    string BloodGroup,
    int Quantity,
    string Status,
    DateTime RequestedAt);

public record AdminHospitalItem(
    int Id,
    string Name,
    string Location,
    string ContactNumber,
    int RequestCount);

public record AdminHospitalDetail(
    int Id,
    string Name,
    string Location,
    string ContactNumber,
    int UserId,
    int RequestCount,
    IReadOnlyList<HospitalRequestSummary> Requests);

public record HospitalRequestSummary(
    int Id,
    string DonorName,
    string BloodGroup,
    int Quantity,
    string Status,
    DateTime RequestedAt);

public record AdminStatistics(
    int TotalUsers,
    int TotalDonors,
    int TotalHospitals,
    int TotalRequests,
    int PendingRequests,
    int AcceptedRequests,
    int RejectedRequests);

