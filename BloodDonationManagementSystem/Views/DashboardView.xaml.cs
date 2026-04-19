using System;
using System.Windows;
using System.Windows.Controls;

namespace BloodDonationManagementSystem.Views;

public partial class DashboardView : UserControl
{
    public event Action? LogoutRequested;

    public DashboardView(string role, string username)
    {
        InitializeComponent();

        DashboardTitle.Text = $"{role} Dashboard";
        WelcomeText.Text = $"Welcome {username}. You are signed in as {role}.";
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        LogoutRequested?.Invoke();
    }
}

