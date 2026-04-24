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
        UpdatePasswordPlaceholder();
        UpdateRolePlaceholder();
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

    private void Register_Click(object sender, RoutedEventArgs e)
    {
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

        var result = _authService.Register(
            UsernameBox.Text,
            PasswordBox.Password,
            selectedRole);

        MessageBox.Show(result ? "Registered" : "Username exists");

        if (result)
            LoginRequested?.Invoke();
    }

    private void OpenLogin_Click(object sender, RoutedEventArgs e)
    {
        LoginRequested?.Invoke();
    }
}

