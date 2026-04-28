using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using BloodDonationManagementSystem.Models;
using BloodDonationManagementSystem.Views;

namespace BloodDonationManagementSystem;

public partial class MainWindow : Window
{
    private readonly SemaphoreSlim _navigationLock = new(1, 1);

    public MainWindow()
    {
        InitializeComponent();
        _ = ShowLoginViewAsync();
    }

    private async Task ShowLoginViewAsync()
    {
        var loginView = new LoginView();
        loginView.RegisterRequested += () => _ = ShowRegisterViewAsync();
        loginView.LoginSucceeded += user => _ = ShowDashboardViewAsync(user);

        await NavigateToAsync(loginView);
    }

    private async Task ShowRegisterViewAsync()
    {
        var registerView = new RegisterView();
        registerView.LoginRequested += () => _ = ShowLoginViewAsync();

        await NavigateToAsync(registerView);
    }

    private async Task ShowDashboardViewAsync(User user)
    {
        try
        {
            var dashboardView = CreateDashboardView(user);
            if (dashboardView == null)
            {
                MessageBox.Show($"Unsupported role '{user.Role}'. Please login again.");
                await ShowLoginViewAsync();
                return;
            }

            switch (dashboardView)
            {
                case AdminDashboardView adminDashboard:
                    adminDashboard.LogoutRequested += () => _ = ShowLoginViewAsync();
                    adminDashboard.ViewUsersRequested += () => _ = ShowAdminViewUsersAsync(user);
                    adminDashboard.EditDonorsRequested += () => _ = ShowAdminEditDonorsAsync(user);
                    adminDashboard.ViewHospitalsRequested += () => _ = ShowAdminViewHospitalsAsync(user);
                    adminDashboard.EditHospitalsRequested += () => _ = ShowAdminEditHospitalsAsync(user);
                    break;
                case HospitalDashboardView hospitalDashboard:
                    hospitalDashboard.LogoutRequested += () => _ = ShowLoginViewAsync();
                    hospitalDashboard.RequestDonorRequested += () => _ = ShowHospitalDonorRequestViewAsync(user);
                    break;
                case DonorDashboardView donorDashboard:
                    donorDashboard.LogoutRequested += () => _ = ShowLoginViewAsync();
                    break;
            }

            await NavigateToAsync(dashboardView);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to open dashboard: {ex.Message}");
            await ShowLoginViewAsync();
        }
    }

    private static UserControl? CreateDashboardView(User user)
    {
        var normalizedRole = NormalizeRoleForNavigation(user.Role);

        if (string.Equals(normalizedRole, "Admin", System.StringComparison.OrdinalIgnoreCase))
            return new AdminDashboardView(user.Username, user.Id);

        if (string.Equals(normalizedRole, "Hospital", System.StringComparison.OrdinalIgnoreCase))
            return new HospitalDashboardView(user.Username, user.Id);

        if (string.Equals(normalizedRole, "Donor", System.StringComparison.OrdinalIgnoreCase))
            return new DonorDashboardView(user.Username, user.Id);

        return null;
    }

    private static string NormalizeRoleForNavigation(string? role)
    {
        var value = (role ?? string.Empty).Trim();
        var key = new string(value.Where(char.IsLetter).ToArray()).ToLowerInvariant();

        if (key is "admin" or "administrator")
            return "Admin";

        if (key is "donor" or "donors")
            return "Donor";

        if (key is "hospital" or "hospitals" or "hostpital" or "hospitial" or "hospitral" or "hostpitral")
            return "Hospital";

        return value;
    }

    private async Task ShowHospitalDonorRequestViewAsync(User user)
    {
        var requestView = new HospitalDonorRequestView(user.Username, user.Id);
        requestView.BackRequested += () => _ = ShowDashboardViewAsync(user);
        requestView.LogoutRequested += () => _ = ShowLoginViewAsync();

        await NavigateToAsync(requestView);
    }

     private async Task ShowAdminManageDonorsAsync(User user)
     {
         var manageDonorsView = new AdminViewDonorsView();
         manageDonorsView.BackRequested += () => _ = ShowDashboardViewAsync(user);

         await NavigateToAsync(manageDonorsView);
     }

     private async Task ShowAdminViewUsersAsync(User user)
     {
         var viewUsersView = new AdminViewDonorsView();
         viewUsersView.BackRequested += () => _ = ShowDashboardViewAsync(user);

         await NavigateToAsync(viewUsersView);
     }

     private async Task ShowAdminEditDonorsAsync(User user)
     {
         var editDonorsView = new AdminViewDonorsView();
         editDonorsView.BackRequested += () => _ = ShowDashboardViewAsync(user);

         await NavigateToAsync(editDonorsView);
     }

     private async Task ShowAdminViewHospitalsAsync(User user)
     {
         var viewHospitalsView = new AdminViewHospitalsView();
         viewHospitalsView.BackRequested += () => _ = ShowDashboardViewAsync(user);

         await NavigateToAsync(viewHospitalsView);
     }

     private async Task ShowAdminEditHospitalsAsync(User user)
     {
         var editHospitalsView = new AdminViewHospitalsView();
         editHospitalsView.BackRequested += () => _ = ShowDashboardViewAsync(user);

         await NavigateToAsync(editHospitalsView);
     }

     private async Task NavigateToAsync(UIElement newContent)
    {
        try
        {
            await _navigationLock.WaitAsync();

            if (ShellContent.Content == newContent)
                return;

            await AnimateOpacityAsync(1, 0, 170);
            ShellContent.Content = newContent;
            await AnimateOpacityAsync(0, 1, 170);
        }
        finally
        {
            _navigationLock.Release();
        }
    }

    private Task AnimateOpacityAsync(double from, double to, int durationMs)
    {
        var tcs = new TaskCompletionSource<bool>();

        var animation = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(durationMs))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
        };

        animation.Completed += (_, _) => tcs.TrySetResult(true);
        ShellContent.BeginAnimation(OpacityProperty, animation);

        return tcs.Task;
    }
}
