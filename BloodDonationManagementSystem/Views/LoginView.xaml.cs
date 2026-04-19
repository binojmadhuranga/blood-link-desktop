using System;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Models;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class LoginView : UserControl
{
    private readonly AuthService _authService = new();

    public event Action? RegisterRequested;
    public event Action<User>? LoginSucceeded;

    public LoginView()
    {
        InitializeComponent();
        UpdatePasswordPlaceholder();
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        UpdatePasswordPlaceholder();
    }

    private void PasswordBox_OnFocusChanged(object sender, RoutedEventArgs e)
    {
        UpdatePasswordPlaceholder();
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

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        var user = _authService.Login(UsernameBox.Text, PasswordBox.Password);

        if (user == null)
        {
            MessageBox.Show("Invalid credentials");
            return;
        }

        LoginSucceeded?.Invoke(user);
    }

    private void OpenRegister_Click(object sender, RoutedEventArgs e)
    {
        RegisterRequested?.Invoke();
    }
}

