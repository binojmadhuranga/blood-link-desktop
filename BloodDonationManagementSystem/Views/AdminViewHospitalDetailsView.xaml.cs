using System;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class AdminViewHospitalDetailsView : UserControl
{
    private AdminService _adminService = new();
    private int _hospitalId;
    public event Action? BackRequested;

    public AdminViewHospitalDetailsView(int hospitalId)
    {
        InitializeComponent();
        _hospitalId = hospitalId;
        BackButton.Click += Back_Click;
        LoadHospitalDetails();
    }

    private void LoadHospitalDetails()
    {
        try
        {
            var hospital = _adminService.GetHospitalById(_hospitalId);
            if (hospital == null)
            {
                MessageBox.Show("Hospital not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                BackRequested?.Invoke();
                return;
            }

            HospitalNameDisplay.Text = hospital.Name;
            LocationDisplay.Text = hospital.Location;
            ContactDisplay.Text = hospital.ContactNumber;
            RequestCountDisplay.Text = hospital.RequestCount.ToString();

            RequestsGrid.ItemsSource = hospital.Requests;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading hospital details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            BackRequested?.Invoke();
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke();
    }
}

