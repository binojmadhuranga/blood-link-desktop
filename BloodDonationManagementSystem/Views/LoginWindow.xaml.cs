using System.Windows;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class LoginWindow : Window
{
    private readonly AuthService _authService = new();

    public LoginWindow()
    {
        InitializeComponent();
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        var user = _authService.Login(
            UsernameBox.Text,
            PasswordBox.Password);

        if (user == null)
        {
            MessageBox.Show("Invalid credentials");
            return;
        }

        if (user.Role == "Admin")
            new AdminDashboard().Show();

        else if (user.Role == "Donor")
            new DonorDashboard().Show();

        else if (user.Role == "Hospital")
            new HospitalDashboard().Show();

        Close();
    }
    
    private void OpenRegister_Click(object sender, RoutedEventArgs e)
    {
        var registerWindow = new RegisterWindow();
        registerWindow.Show();
        Close();
    }
}