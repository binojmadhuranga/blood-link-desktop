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

    public HospitalDashboardView(string username, int userId)
    {
        InitializeComponent();
        _userId = userId;
        RefreshButton.Click += Refresh_Click;
        LogoutButton.Click += Logout_Click;
        DonorComboBox.SelectionChanged += DonorComboBox_OnSelectionChanged;
        CreateRequestButton.Click += CreateRequest_Click;
        WelcomeText.Text = $"Welcome {username}. You are signed in as Hospital.";
        LoadDashboard();
    }

    private void LoadDashboard()
    {
        DonorComboBox.ItemsSource = _bloodRequestService.GetDonorOptions();
        DonorsAvailableText.Text = _bloodRequestService.GetDonorOptions().Count.ToString();
        HospitalRequestsGrid.ItemsSource = _bloodRequestService.GetHospitalRequests(_userId);

        var requests = _bloodRequestService.GetHospitalRequests(_userId);
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
        LoadDashboard();
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        LogoutRequested?.Invoke();
    }
}


