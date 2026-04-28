using System;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class AdminEditHospitalView : UserControl
{
    private AdminService _adminService = new();
    private int _hospitalId;
    public event Action? BackRequested;

    public AdminEditHospitalView(int hospitalId)
    {
        InitializeComponent();
        _hospitalId = hospitalId;
        SaveButton.Click += Save_Click;
        CancelButton.Click += Cancel_Click;
        LoadHospitalData();
    }

    private void LoadHospitalData()
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

            NameInput.Text = hospital.Name;
            LocationInput.Text = hospital.Location;
            ContactNumberInput.Text = hospital.ContactNumber;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading hospital data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            BackRequested?.Invoke();
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameInput.Text))
        {
            MessageBox.Show("Hospital name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(LocationInput.Text))
        {
            MessageBox.Show("Location is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(ContactNumberInput.Text))
        {
            MessageBox.Show("Contact number is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_adminService.UpdateHospital(
            _hospitalId,
            NameInput.Text,
            LocationInput.Text,
            ContactNumberInput.Text,
            out var error))
        {
            MessageBox.Show("Hospital updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            BackRequested?.Invoke();
        }
        else
        {
            MessageBox.Show($"Error updating hospital: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke();
    }
}

