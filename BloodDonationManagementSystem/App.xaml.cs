using System.Windows;
using BloodDonationManagementSystem.Seeders;

namespace BloodDonationManagementSystem;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Initialize database and seed admin account
        DatabaseSeeder.Seed();

        // Open the single-window shell and swap views inside it.
        var mainWindow = new MainWindow();
        MainWindow = mainWindow;
        mainWindow.Show();
    }
}