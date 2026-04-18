namespace BloodDonationManagementSystem.Models;

public class Donor
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string BloodGroup { get; set; }
    public string ContactNumber { get; set; }
    public string Location { get; set; }
    public DateTime LastDonationDate { get; set; }
    public int UserId { get; set; }
}