using System;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class AdminEditDonorView : UserControl
{
    private AdminService _adminService = new();
    private int _donorId;
    public event Action? BackRequested;

    public AdminEditDonorView(int donorId)
    {
        InitializeComponent();
        _donorId = donorId;
        SaveButton.Click += Save_Click;
        CancelButton.Click += Cancel_Click;
        LoadDonorData();
    }

    private void LoadDonorData()
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

            FullNameInput.Text = donor.FullName;
            BloodGroupInput.Text = donor.BloodGroup;
            ContactInput.Text = donor.Contact;
            LocationInput.Text = donor.Location;
            LastDonationDateDisplay.Text = donor.LastDonationDate == DateTime.MinValue 
                ? "Never" 
                : donor.LastDonationDate.ToString("dd-MMM-yyyy");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading donor data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            BackRequested?.Invoke();
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FullNameInput.Text))
        {
            MessageBox.Show("Full name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(ContactInput.Text))
        {
            MessageBox.Show("Contact number is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(LocationInput.Text))
        {
            MessageBox.Show("Location is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_adminService.UpdateDonor(
            _donorId,
            FullNameInput.Text,
            BloodGroupInput.Text,
            ContactInput.Text,
            LocationInput.Text,
            out var error))
        {
            MessageBox.Show("Donor updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            BackRequested?.Invoke();
        }
        else
        {
            MessageBox.Show($"Error updating donor: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke();
    }
}

