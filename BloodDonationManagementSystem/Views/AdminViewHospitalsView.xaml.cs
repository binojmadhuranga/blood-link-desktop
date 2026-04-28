using System;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class AdminViewHospitalsView : UserControl
{
    private AdminService _adminService = new();
    public event Action? BackRequested;
    public event Action<int>? EditHospitalRequested;
    public event Action<int>? ViewHospitalRequested;

    public AdminViewHospitalsView()
    {
        InitializeComponent();
        RefreshButton.Click += Refresh_Click;
        BackButton.Click += Back_Click;
        LoadHospitals();
    }

    private void LoadHospitals()
    {
        try
        {
            var hospitals = _adminService.GetAllHospitals();
            HospitalsGrid.ItemsSource = hospitals;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading hospitals: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        LoadHospitals();
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke();
    }

    private void ViewHospital_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int hospitalId)
        {
            ViewHospitalRequested?.Invoke(hospitalId);
        }
    }

    private void EditHospital_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int hospitalId)
        {
            EditHospitalRequested?.Invoke(hospitalId);
        }
    }

    private void DeleteHospital_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int hospitalId)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete this hospital? This action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (_adminService.DeleteHospital(hospitalId, out var error))
                {
                    MessageBox.Show("Hospital deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadHospitals();
                }
                else
                {
                    MessageBox.Show($"Error deleting hospital: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

