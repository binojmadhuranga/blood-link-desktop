using System;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class HospitalDashboardView : UserControl
{
    public event Action? LogoutRequested;
    public event Action? RequestDonorRequested;

    private readonly BloodRequestService _bloodRequestService = new();
    private readonly int _userId;
    private bool _isDashboardLoaded;

    public HospitalDashboardView(string username, int userId)
    {
        InitializeComponent();
        _userId = userId;
        OpenRequestPanelButton.Click += OpenRequestPanel_Click;
        RefreshButton.Click += Refresh_Click;
        LogoutButton.Click += Logout_Click;
        Loaded += HospitalDashboardView_OnLoaded;
        WelcomeText.Text = $"Welcome {username}. You are signed in as Hospital.";
    }

    private void HospitalDashboardView_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_isDashboardLoaded)
            return;

        _isDashboardLoaded = true;
        TryLoadDashboard();
    }

    private void TryLoadDashboard()
    {
        try
        {
            LoadDashboard();
        }
        catch (Exception ex)
        {
            HospitalRequestsGrid.ItemsSource = Array.Empty<HospitalRequestItem>();
            RequestsSentText.Text = "0";
            PendingText.Text = "0";
            DonorsAvailableText.Text = "0";
            MessageBox.Show($"Unable to load hospital data: {ex.Message}");
        }
    }

    private void LoadDashboard()
    {
        var donorOptions = _bloodRequestService.GetDonorOptions();
        DonorsAvailableText.Text = donorOptions.Count.ToString();

        var requests = _bloodRequestService.GetHospitalRequests(_userId);
        HospitalRequestsGrid.ItemsSource = requests;
        RequestsSentText.Text = requests.Count.ToString();
        PendingText.Text = requests.Count(request => string.Equals(request.Status, "Pending", StringComparison.OrdinalIgnoreCase)).ToString();
    }

    private void OpenRequestPanel_Click(object sender, RoutedEventArgs e)
    {
        RequestDonorRequested?.Invoke();
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        TryLoadDashboard();
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        LogoutRequested?.Invoke();
    }
}


