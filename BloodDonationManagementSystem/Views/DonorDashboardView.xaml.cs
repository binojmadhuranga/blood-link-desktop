using System;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class DonorDashboardView : UserControl
{
    public event Action? LogoutRequested;

    private readonly BloodRequestService _bloodRequestService = new();
    private readonly int _userId;

    public DonorDashboardView(string username, int userId)
    {
        InitializeComponent();
        _userId = userId;
        RefreshButton.Click += Refresh_Click;
        LogoutButton.Click += Logout_Click;
        WelcomeText.Text = $"Welcome {username}. You are signed in as Donor.";
        LoadDashboard();
    }

    private void LoadDashboard()
    {
        var requests = _bloodRequestService.GetDonorRequests(_userId);

        DonorRequestsGrid.ItemsSource = requests;
        RequestsCountText.Text = requests.Count.ToString();
        PendingCountText.Text = requests.Count(request => string.Equals(request.Status, "Pending", StringComparison.OrdinalIgnoreCase)).ToString();
        ApprovedCountText.Text = requests.Count(request => string.Equals(request.Status, "Approved", StringComparison.OrdinalIgnoreCase)).ToString();
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        LoadDashboard();
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        LogoutRequested?.Invoke();
    }
}

