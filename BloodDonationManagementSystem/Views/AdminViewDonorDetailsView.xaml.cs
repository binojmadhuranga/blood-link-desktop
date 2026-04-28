using System;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class AdminViewDonorDetailsView : UserControl
{
    private AdminService _adminService = new();
    private int _donorId;
    public event Action? BackRequested;

    public AdminViewDonorDetailsView(int donorId)
    {
        InitializeComponent();
        _donorId = donorId;
        BackButton.Click += Back_Click;
        LoadDonorDetails();
    }

    private void LoadDonorDetails()
    {
        try
        {
            var donor = _adminService.GetDonorById(_donorId);
            if (donor == null)
            {
                MessageBox.Show("Donor not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                BackRequested?.Invoke();
                return;
            }

            DonorNameDisplay.Text = donor.FullName;
            BloodGroupDisplay.Text = donor.BloodGroup ?? "Not specified";
            ContactDisplay.Text = donor.Contact;
            LocationDisplay.Text = donor.Location;
            LastDonationDisplay.Text = donor.LastDonationDate == DateTime.MinValue 
                ? "Never" 
                : donor.LastDonationDate.ToString("dd-MMM-yyyy");

            RequestsGrid.ItemsSource = donor.Requests;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading donor details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            BackRequested?.Invoke();
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke();
    }
}

