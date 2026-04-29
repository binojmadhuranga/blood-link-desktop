using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using BloodDonationManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
namespace BloodDonationManagementSystem.Views;
public partial class AdminDashboardView : UserControl
{
    public event Action? LogoutRequested;
    public event Action? ViewUsersRequested;
    public event Action? EditDonorsRequested;
    public event Action? ViewHospitalsRequested;
    public event Action? EditHospitalsRequested;

    public AdminDashboardView(string username, int userId)
    {
        InitializeComponent();
        RefreshButton.Click += Refresh_Click;
            GenerateReportButton.Click += GenerateReport_Click;
        LogoutButton.Click += Logout_Click;
        ViewUsersButton.Click += ViewUsers_Click;
        EditDonorsButton.Click += EditDonors_Click;
        ViewHospitalsButton.Click += ViewHospitals_Click;
        EditHospitalsButton.Click += EditHospitals_Click;
        LoadDashboard();
    }

        private void GenerateReport_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                using var db = new AppDbContext();

                var hospitals = db.Hospitals
                    .AsNoTracking()
                    .OrderBy(h => h.Name)
                    .Select(h => new
                    {
                        h.Id,
                        h.Name,
                        h.Location,
                        h.ContactNumber,
                        RequestCount = db.BloodRequests.Count(r => r.HospitalId == h.Id)
                    })
                    .ToList();

                var requests = db.BloodRequests
                    .AsNoTracking()
                    .Include(r => r.Hospital)
                    .Include(r => r.Donor)
                    .OrderByDescending(r => r.RequestedAt)
                    .Select(r => new
                    {
                        r.Id,
                        HospitalId = r.HospitalId,
                        HospitalName = r.Hospital != null ? r.Hospital.Name : "",
                        DonorId = r.DonorId,
                        DonorName = r.Donor != null ? r.Donor.FullName : "",
                        r.BloodGroup,
                        r.Quantity,
                        r.Status,
                        r.RequestedAt
                    })
                    .ToList();

                var sb = new StringBuilder();

                sb.AppendLine("Hospitals Summary");
                sb.AppendLine("HospitalId,Name,Location,ContactNumber,TotalRequests");
                foreach (var h in hospitals)
                {
                    // escape commas
                    string name = EscapeCsv(h.Name);
                    string loc = EscapeCsv(h.Location);
                    string contact = EscapeCsv(h.ContactNumber);
                    sb.AppendLine($"{h.Id},{name},{loc},{contact},{h.RequestCount}");
                }

                sb.AppendLine();
                sb.AppendLine("Requests Details");
                sb.AppendLine("RequestId,HospitalId,HospitalName,DonorId,DonorName,BloodGroup,Quantity,Status,RequestedAt");
                foreach (var r in requests)
                {
                    string hname = EscapeCsv(r.HospitalName);
                    string dname = EscapeCsv(r.DonorName);
                    sb.AppendLine($"{r.Id},{r.HospitalId},{hname},{r.DonorId},{dname},{r.BloodGroup},{r.Quantity},{r.Status},{r.RequestedAt:O}");
                }

                var dlg = new SaveFileDialog()
                {
                    Title = "Save admin report",
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    FileName = $"admin-report-{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (dlg.ShowDialog() == true)
                {
                    File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show(Window.GetWindow(this), "Report saved to: " + dlg.FileName, "Report Generated", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Window.GetWindow(this), "Failed to generate report: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string EscapeCsv(string? input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            if (input.Contains(',') || input.Contains('"') || input.Contains('\n') || input.Contains('\r'))
            {
                return '"' + input.Replace("\"", "\"\"") + '"';
            }
            return input;
        }
    private void LoadDashboard()
    {
        using var db = new AppDbContext();
        UsersCountText.Text = db.Users.Count().ToString();
        HospitalsCountText.Text = db.Hospitals.Count().ToString();
        DonorsCountText.Text = db.Donors.Count().ToString();
        RequestsCountText.Text = db.BloodRequests.Count().ToString();
        RequestsGrid.ItemsSource = db.BloodRequests
            .AsNoTracking()
            .OrderByDescending(request => request.RequestedAt)
            .Take(50)
            .Select(request => new AdminRequestItem(
                request.Id,
                request.Hospital != null ? request.Hospital.Name : "Unknown",
                request.Donor != null ? request.Donor.FullName : "Unknown",
                request.BloodGroup,
                request.Quantity,
                request.Status,
                request.RequestedAt))
            .ToList();
    }
    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        LoadDashboard();
    }
    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        LogoutRequested?.Invoke();
    }

    private void ViewUsers_Click(object sender, RoutedEventArgs e)
    {
        ViewUsersRequested?.Invoke();
    }

    private void EditDonors_Click(object sender, RoutedEventArgs e)
    {
        EditDonorsRequested?.Invoke();
    }

    private void ViewHospitals_Click(object sender, RoutedEventArgs e)
    {
        ViewHospitalsRequested?.Invoke();
    }

    private void EditHospitals_Click(object sender, RoutedEventArgs e)
    {
        EditHospitalsRequested?.Invoke();
    }
}
public record AdminRequestItem(
    int Id,
    string HospitalName,
    string DonorName,
    string BloodGroup,
    int Quantity,
    string Status,
    DateTime RequestedAt);
