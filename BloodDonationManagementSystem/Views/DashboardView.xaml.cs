using System;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class DashboardView : UserControl
{
    public event Action? LogoutRequested;

    private readonly BloodRequestService _bloodRequestService = new();
    private readonly string _role;
    private readonly int _userId;

    public DashboardView(string role, string username, int userId)
    {
        InitializeComponent();
        _role = role;
        _userId = userId;

        DashboardTitle.Text = $"{role} Dashboard";
        WelcomeText.Text = $"Welcome {username}. You are signed in as {role}.";

        SetupRoleDashboard();
    }

    private void SetupRoleDashboard()
    {
        if (string.Equals(_role, "Hospital", StringComparison.OrdinalIgnoreCase))
        {
            HospitalPanel.Visibility = Visibility.Visible;
            LoadHospitalData();
            return;
        }

        if (string.Equals(_role, "Donor", StringComparison.OrdinalIgnoreCase))
        {
            DonorPanel.Visibility = Visibility.Visible;
            DonorRequestsGrid.ItemsSource = _bloodRequestService.GetDonorRequests(_userId);
            return;
        }

        GenericPanelText.Visibility = Visibility.Visible;
    }

    private void LoadHospitalData()
    {
        DonorComboBox.ItemsSource = _bloodRequestService.GetDonorOptions();
        HospitalRequestsGrid.ItemsSource = _bloodRequestService.GetHospitalRequests(_userId);
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
        HospitalRequestsGrid.ItemsSource = _bloodRequestService.GetHospitalRequests(_userId);
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        LogoutRequested?.Invoke();
    }
}

