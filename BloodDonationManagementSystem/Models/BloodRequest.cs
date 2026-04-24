namespace BloodDonationManagementSystem.Models;

public class BloodRequest
{
    public int Id { get; set; }
    public string BloodGroup { get; set; } = "";
    public int Quantity { get; set; }
    public string Status { get; set; } = "Pending";
    public string Notes { get; set; } = "";
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public int HospitalId { get; set; }
    public int DonorId { get; set; }

    public Hospital? Hospital { get; set; }
    public Donor? Donor { get; set; }
}