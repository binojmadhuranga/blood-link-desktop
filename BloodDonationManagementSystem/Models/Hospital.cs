namespace BloodDonationManagementSystem.Models;

public class Hospital
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string ContactNumber { get; set; }
    public int UserId { get; set; }
}