using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Services;

namespace BloodDonationManagementSystem.Views;

public partial class HospitalDonorRequestView : UserControl
{
    public event Action? BackRequested;
    public event Action? LogoutRequested;

    private readonly BloodRequestService _bloodRequestService = new();
    private readonly int _userId;

    public HospitalDonorRequestView(string username, int userId)
    {
        InitializeComponent();
        _userId = userId;

        BackButton.Click += Back_Click;
        LogoutButton.Click += Logout_Click;
        RegisterRequestButton.Click += RegisterRequest_Click;
        DonorListBox.SelectionChanged += DonorListBox_OnSelectionChanged;

        PageInfoText.Text = $"Hospital: {username}. Select one or more donors.";
        LoadDonors();
    }

    private void LoadDonors()
    {
        var donors = _bloodRequestService.GetDonorOptions();
        DonorListBox.ItemsSource = donors;
        SelectionInfoText.Text = $"0 selected out of {donors.Count} registered donors.";
    }

    private void DonorListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var total = DonorListBox.Items.Count;
        var selected = DonorListBox.SelectedItems.Count;
        SelectionInfoText.Text = $"{selected} selected out of {total} registered donors.";
    }

    private void RegisterRequest_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(QuantityBox.Text, out var quantity) || quantity <= 0)
        {
            MessageBox.Show("Please enter a valid quantity greater than zero.");
            return;
        }

        var selectedDonorIds = DonorListBox.SelectedItems
            .OfType<DonorOption>()
            .Select(donor => donor.Id)
            .Distinct()
            .ToList();

        var result = _bloodRequestService.CreateRequestsForDonors(_userId, selectedDonorIds, quantity, NotesBox.Text);

        if (result.SuccessCount == 0)
        {
            var errorText = string.Join(Environment.NewLine, result.Failures);
            MessageBox.Show(string.IsNullOrWhiteSpace(errorText)
                ? "Request could not be created."
                : errorText);
            return;
        }

        if (result.Failures.Count > 0)
        {
            MessageBox.Show($"Requests sent to {result.SuccessCount} donor(s).{Environment.NewLine}{Environment.NewLine}Issues:{Environment.NewLine}{string.Join(Environment.NewLine, result.Failures)}");
        }
        else
        {
            MessageBox.Show($"Requests sent to {result.SuccessCount} donor(s).", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        QuantityBox.Text = "1";
        NotesBox.Text = string.Empty;
        DonorListBox.SelectedItems.Clear();
        LoadDonors();
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke();
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        LogoutRequested?.Invoke();
    }
}
