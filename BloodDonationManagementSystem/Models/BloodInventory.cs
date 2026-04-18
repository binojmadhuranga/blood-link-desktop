namespace BloodDonationManagementSystem.Models;

public class BloodInventory
{
    public int Id { get; set; }
    public string BloodGroup { get; set; } =  "";
    public int UnitsAvailable { get; set; }
}