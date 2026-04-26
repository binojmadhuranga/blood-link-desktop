using System;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class RegisterView : UserControl
{
    private readonly AuthService _authService = new();

    public event Action? LoginRequested;

    public RegisterView()
    {
        InitializeComponent();
        RoleBox.SelectedIndex = -1;
        BloodGroupBox.SelectedIndex = -1;
        UpdatePasswordPlaceholder();
        UpdateRolePlaceholder();
        UpdateBloodGroupPlaceholder();
        UpdateBloodGroupVisibility();
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        UpdatePasswordPlaceholder();
    }

    private void PasswordBox_OnFocusChanged(object sender, RoutedEventArgs e)
    {
        UpdatePasswordPlaceholder();
    }

    private void RoleBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateRolePlaceholder();
        UpdateBloodGroupVisibility();
    }

    private void BloodGroupBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateBloodGroupPlaceholder();
    }

    private void UpdatePasswordPlaceholder()
    {
        if (PasswordPlaceholder == null)
            return;

        PasswordPlaceholder.Visibility =
            string.IsNullOrEmpty(PasswordBox.Password) && !PasswordBox.IsKeyboardFocused
                ? Visibility.Visible
                : Visibility.Collapsed;
    }

    private void UpdateRolePlaceholder()
    {
        if (RolePlaceholder == null)
            return;

        RolePlaceholder.Visibility = RoleBox.SelectedIndex < 0
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void UpdateBloodGroupPlaceholder()
    {
        if (BloodGroupPlaceholder == null)
            return;

        BloodGroupPlaceholder.Visibility = BloodGroupBox.SelectedIndex < 0
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void UpdateBloodGroupVisibility()
    {
        var selectedRole = (RoleBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ??
                           (RoleBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;

        var isDonor = string.Equals(selectedRole, "Donor", StringComparison.OrdinalIgnoreCase);
        BloodGroupSection.Visibility = isDonor ? Visibility.Visible : Visibility.Collapsed;

        if (!isDonor)
        {
            BloodGroupBox.SelectedIndex = -1;
            UpdateBloodGroupPlaceholder();
        }
    }

    private void Register_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ContactNumberBox.Text))
        {
            MessageBox.Show("Please enter a contact number.");
            return;
        }

        if (string.IsNullOrWhiteSpace(LocationBox.Text))
        {
            MessageBox.Show("Please enter a location.");
            return;
        }

        if (RoleBox.SelectedItem is not ComboBoxItem selectedRoleItem)
        {
            MessageBox.Show("Please select a role.");
            return;
        }

        var selectedRole = selectedRoleItem.Tag?.ToString() ?? selectedRoleItem.Content?.ToString();
        if (string.IsNullOrWhiteSpace(selectedRole))
        {
            MessageBox.Show("Please select a role.");
            return;
        }

        var isDonor = string.Equals(selectedRole, "Donor", StringComparison.OrdinalIgnoreCase);
        if (isDonor && BloodGroupBox.SelectedItem is not ComboBoxItem selectedBloodGroupItem)
        {
            MessageBox.Show("Please select a blood group.");
            return;
        }

        var bloodGroup = isDonor
            ? (BloodGroupBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty
            : string.Empty;

        var result = _authService.Register(
            UsernameBox.Text,
            PasswordBox.Password,
            selectedRole,
            ContactNumberBox.Text,
            LocationBox.Text,
            bloodGroup);

        MessageBox.Show(result ? "Registered" : "Registration failed. Check your details or use another username.");

        if (result)
            LoginRequested?.Invoke();
    }

    private void OpenLogin_Click(object sender, RoutedEventArgs e)
    {
        LoginRequested?.Invoke();
    }
}

