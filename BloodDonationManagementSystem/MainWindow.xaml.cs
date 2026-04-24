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
        var dashboardView = new DashboardView(user.Role, user.Username, user.Id);
        dashboardView.LogoutRequested += () => _ = ShowLoginViewAsync();

        await NavigateToAsync(dashboardView);
    }

    private async Task NavigateToAsync(UserControl view)
    {
        await _navigationLock.WaitAsync();
        try
        {
            await AnimateOpacityAsync(1, 0, 140);
            ShellContent.Content = view;
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

