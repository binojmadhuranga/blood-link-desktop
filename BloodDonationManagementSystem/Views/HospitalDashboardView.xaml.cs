using System;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class HospitalDashboardView : UserControl
{
    public event Action? LogoutRequested;

    private readonly BloodRequestService _bloodRequestService = new();
    private readonly int _userId;
    private bool _isDashboardLoaded;

    public HospitalDashboardView(string username, int userId)
    {
        InitializeComponent();
        _userId = userId;
        RefreshButton.Click += Refresh_Click;
        LogoutButton.Click += Logout_Click;
        DonorComboBox.SelectionChanged += DonorComboBox_OnSelectionChanged;
        CreateRequestButton.Click += CreateRequest_Click;
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
            DonorComboBox.ItemsSource = Array.Empty<DonorOption>();
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
        DonorComboBox.ItemsSource = donorOptions;
        DonorsAvailableText.Text = donorOptions.Count.ToString();

        var requests = _bloodRequestService.GetHospitalRequests(_userId);
        HospitalRequestsGrid.ItemsSource = requests;
        RequestsSentText.Text = requests.Count.ToString();
        PendingText.Text = requests.Count(request => string.Equals(request.Status, "Pending", StringComparison.OrdinalIgnoreCase)).ToString();
    }

    private void DonorComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DonorComboBox.SelectedItem is DonorOption donor)
        {
            BloodGroupBox.Text = donor.BloodGroup;
        }
    }

    private void CreateRequest_Click(object sender, RoutedEventArgs e)
    {
        if (DonorComboBox.SelectedItem is not DonorOption donor)
        {
            MessageBox.Show("Please select a donor.");
            return;
        }

        if (!int.TryParse(QuantityBox.Text, out var quantity))
        {
            MessageBox.Show("Please enter a valid quantity.");
            return;
        }

        var created = _bloodRequestService.CreateRequest(
            _userId,
            donor.Id,
            BloodGroupBox.Text,
            quantity,
            NotesBox.Text,
            out var error);

        if (!created)
        {
            MessageBox.Show(error);
            return;
        }

        MessageBox.Show("Blood request sent to donor.");
        QuantityBox.Text = "1";
        NotesBox.Text = string.Empty;
        LoadDashboard();
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


