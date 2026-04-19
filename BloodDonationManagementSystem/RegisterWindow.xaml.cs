using System.Windows;
using BloodDonationManagementSystem.Services;
using System.Windows.Controls;

namespace BloodDonationManagementSystem.Views;

public partial class RegisterWindow : Window
{
    private readonly AuthService _authService = new();

    public RegisterWindow()
    {
        InitializeComponent();
    }

    private void Register_Click(object sender, RoutedEventArgs e)
    {
        var selectedRole = ((ComboBoxItem)RoleBox.SelectedItem).Content.ToString();

        var result = _authService.Register(
            UsernameBox.Text,
            PasswordBox.Password,
            selectedRole);

        MessageBox.Show(result ? "Registered" : "Username exists");
    }
}