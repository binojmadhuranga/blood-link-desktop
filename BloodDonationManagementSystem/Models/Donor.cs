public class Donor
{
    public int Id { get; set; }

    public string FullName { get; set; } = "";

    public string BloodGroup { get; set; } = "";

    public string Contact { get; set; } = "";

    public string Location { get; set; } = "";

    public DateTime LastDonationDate { get; set; }

    public int UserId { get; set; }

    public ICollection<BloodDonationManagementSystem.Models.BloodRequest> BloodRequests { get; set; } = new List<BloodDonationManagementSystem.Models.BloodRequest>();
}