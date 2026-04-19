using System.Windows;
using BloodDonationManagementSystem.Seeders;

namespace BloodDonationManagementSystem;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        DatabaseSeeder.Seed();
        base.OnStartup(e);
    }
}