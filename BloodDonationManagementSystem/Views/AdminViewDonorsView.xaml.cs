using System;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class AdminViewDonorsView : UserControl
{
    private AdminService _adminService = new();
    public event Action? BackRequested;
    public event Action<int>? EditDonorRequested;
    public event Action<int>? ViewDonorRequested;

    public AdminViewDonorsView()
    {
        InitializeComponent();
        RefreshButton.Click += Refresh_Click;
        BackButton.Click += Back_Click;
        LoadDonors();
    }

    private void LoadDonors()
    {
        try
        {
            var donors = _adminService.GetAllDonors();
            DonorsGrid.ItemsSource = donors;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading donors: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        LoadDonors();
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke();
    }

    private void ViewDonor_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int donorId)
        {
            ViewDonorRequested?.Invoke(donorId);
        }
    }

    private void EditDonor_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int donorId)
        {
            EditDonorRequested?.Invoke(donorId);
        }
    }

    private void DeleteDonor_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int donorId)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete this donor? This action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (_adminService.DeleteDonor(donorId, out var error))
                {
                    MessageBox.Show("Donor deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDonors();
                }
                else
                {
                    MessageBox.Show($"Error deleting donor: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

