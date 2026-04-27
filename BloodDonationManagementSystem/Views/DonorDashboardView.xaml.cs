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
        AcceptRequestButton.Click += AcceptRequest_Click;
        RejectRequestButton.Click += RejectRequest_Click;
        LogoutButton.Click += Logout_Click;
        WelcomeText.Text = $"Welcome {username}. You are signed in as Donor.";
        LoadDashboard();
    }

    private void LoadDashboard()
    {
        var requests = _bloodRequestService.GetDonorRequests(_userId);

        DonorRequestsGrid.ItemsSource = requests;
        RequestsCountText.Text = requests.Count.ToString();
        PendingCountText.Text = requests.Count(request => string.Equals(request.Status, BloodRequestService.PendingStatus, StringComparison.OrdinalIgnoreCase)).ToString();
        ApprovedCountText.Text = requests.Count(request =>
            string.Equals(request.Status, BloodRequestService.AcceptedStatus, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(request.Status, "Approved", StringComparison.OrdinalIgnoreCase)).ToString();
    }

    private void AcceptRequest_Click(object sender, RoutedEventArgs e)
    {
        UpdateSelectedRequest(BloodRequestService.AcceptedStatus);
    }

    private void RejectRequest_Click(object sender, RoutedEventArgs e)
    {
        UpdateSelectedRequest(BloodRequestService.RejectedStatus);
    }

    private void UpdateSelectedRequest(string targetStatus)
    {
        if (DonorRequestsGrid.SelectedItem is not DonorRequestItem selectedRequest)
        {
            MessageBox.Show("Please select a request first.");
            return;
        }

        var updated = _bloodRequestService.UpdateRequestStatus(_userId, selectedRequest.Id, targetStatus, out var error);
        if (!updated)
        {
            MessageBox.Show(error);
            return;
        }

        MessageBox.Show($"Request marked as {targetStatus}.");
        LoadDashboard();
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

