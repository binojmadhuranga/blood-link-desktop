using System;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
namespace BloodDonationManagementSystem.Views;
public partial class AdminDashboardView : UserControl
{
    public event Action? LogoutRequested;
    public event Action? ViewUsersRequested;
    public event Action? EditDonorsRequested;
    public event Action? ViewHospitalsRequested;
    public event Action? EditHospitalsRequested;

    public AdminDashboardView(string username, int userId)
    {
        InitializeComponent();
        RefreshButton.Click += Refresh_Click;
        LogoutButton.Click += Logout_Click;
        ViewUsersButton.Click += ViewUsers_Click;
        EditDonorsButton.Click += EditDonors_Click;
        ViewHospitalsButton.Click += ViewHospitals_Click;
        EditHospitalsButton.Click += EditHospitals_Click;
        LoadDashboard();
    }
    private void LoadDashboard()
    {
        using var db = new AppDbContext();
        UsersCountText.Text = db.Users.Count().ToString();
        HospitalsCountText.Text = db.Hospitals.Count().ToString();
        DonorsCountText.Text = db.Donors.Count().ToString();
        RequestsCountText.Text = db.BloodRequests.Count().ToString();
        RequestsGrid.ItemsSource = db.BloodRequests
            .AsNoTracking()
            .OrderByDescending(request => request.RequestedAt)
            .Take(50)
            .Select(request => new AdminRequestItem(
                request.Id,
                request.Hospital != null ? request.Hospital.Name : "Unknown",
                request.Donor != null ? request.Donor.FullName : "Unknown",
                request.BloodGroup,
                request.Quantity,
                request.Status,
                request.RequestedAt))
            .ToList();
    }
    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        LoadDashboard();
    }
    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        LogoutRequested?.Invoke();
    }

    private void ViewUsers_Click(object sender, RoutedEventArgs e)
    {
        ViewUsersRequested?.Invoke();
    }

    private void EditDonors_Click(object sender, RoutedEventArgs e)
    {
        EditDonorsRequested?.Invoke();
    }

    private void ViewHospitals_Click(object sender, RoutedEventArgs e)
    {
        ViewHospitalsRequested?.Invoke();
    }

    private void EditHospitals_Click(object sender, RoutedEventArgs e)
    {
        EditHospitalsRequested?.Invoke();
    }
}
public record AdminRequestItem(
    int Id,
    string HospitalName,
    string DonorName,
    string BloodGroup,
    int Quantity,
    string Status,
    DateTime RequestedAt);
