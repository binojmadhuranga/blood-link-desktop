namespace BloodDonationManagementSystem.Models;

public class BloodRequest
{
    public int Id { get; set; }
    public string BloodGroup { get; set; } = "";
    public int Quantity { get; set; }
    public string Status { get; set; } = "";
    public int HospitalId { get; set; }
}