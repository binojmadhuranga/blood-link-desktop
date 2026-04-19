using System.Windows;
using BloodDonationManagementSystem.Seeders;
using BloodDonationManagementSystem.Views;

namespace BloodDonationManagementSystem;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Initialize database and seed admin account
        DatabaseSeeder.Seed();

        // Open login window first
        var loginWindow = new LoginWindow();
        loginWindow.Show();
    }
}