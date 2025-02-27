using CA3_s00219815;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace workwise_app
{
    public partial class MainWindow : Window
    {
        public Booking CurrentBooking { get; set; }

        BookingModelContainer db = new BookingModelContainer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Load(object sender, RoutedEventArgs e)
        {
            var users = from u in db.Users
                        select new { u.UserID, FullName = u.FirstName + " " + u.LastName };

            cbxUser.ItemsSource = users.ToList();
            cbxUser.DisplayMemberPath = "FullName";
            cbxUser.SelectedValuePath = "UserID";

            DisplayAllWorkspaces();
            DisplayAllBookings();
        }

        private void DisplayAllWorkspaces()
        {
            var query = from w in db.Workspaces
                        select w;
            dgWorkspaces.ItemsSource = query.ToList();
        }

        private void DisplayAllBookings()
        {
            var query = from b in db.Bookings
                        select new
                        {
                            b.BookingID,
                            b.StartDate,
                            b.EndDate,
                            WorkspaceName = b.Workspace.Name,
                            UserName = b.User.FirstName + " " + b.User.LastName
                        };
            dgBookings.ItemsSource = query.ToList();
        }

        private void ShowAvailableWorkspaces()
        {
            if (dpStartDate.SelectedDate.HasValue && dpEndDate.SelectedDate.HasValue)
            {
                DateTime startDate = dpStartDate.SelectedDate.Value;
                DateTime endDate = dpEndDate.SelectedDate.Value;

                var query = from w in db.Workspaces
                            where !w.Bookings.Any(b => (b.StartDate <= endDate && b.EndDate >= startDate))
                            orderby w.Name descending
                            select new
                            {
                                w.WorkspaceID,
                                w.Name,
                                w.WorkspaceType,
                                w.Capacity
                            };

                var workspaces = query.Distinct().ToList().Select(w => new Workspace
                {
                    WorkspaceID = w.WorkspaceID,
                    Name = w.Name,
                    WorkspaceType = w.WorkspaceType,
                    Capacity = w.Capacity
                }).ToList();

                dgWorkspacesAvailable.ItemsSource = workspaces;
            }
        }

        private void dgWorkspacesAvailable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgWorkspacesAvailable.SelectedItem is Workspace selectedWorkspace)
            {
                tblkSelectedWorkspace.Text = $"{selectedWorkspace.Name}, {selectedWorkspace.WorkspaceType} (Capacity: {selectedWorkspace.Capacity})";

                if (CurrentBooking == null)
                {
                    CurrentBooking = new Booking();
                }

                CurrentBooking.WorkspaceID = selectedWorkspace.WorkspaceID;
                CurrentBooking.StartDate = DateTime.Now;
                CurrentBooking.EndDate = DateTime.Now.AddDays(1);

                // Extract base name of workspace
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\"));
                string imagesDirectory = Path.Combine(projectDirectory, "Images");

                if (Directory.Exists(imagesDirectory))
                {
                    string[] imageFiles = Directory.GetFiles(imagesDirectory, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                        .ToArray();

                    bool imageFound = false;
                    foreach (string imageFile in imageFiles)
                    {
                        string candidateName = Path.GetFileNameWithoutExtension(imageFile).Replace("_", " ");
                        if (selectedWorkspace.Name.IndexOf(candidateName, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            imgSelectedWorkspace.Source = new BitmapImage(new Uri(imageFile, UriKind.Absolute));
                            imageFound = true;
                            break;
                        }
                    }
                    if (!imageFound)
                    {
                        imgSelectedWorkspace.Source = null;
                    }
                }
                else
                {
                    imgSelectedWorkspace.Source = null;
                }
            }
        }

        private void dpStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpStartDate.SelectedDate.HasValue && dpEndDate.SelectedDate.HasValue)
            {
                if (dpStartDate.SelectedDate.Value.Date >= dpEndDate.SelectedDate.Value.Date)
                {
                    MessageBox.Show("Start date must be before end date.");
                    dpStartDate.SelectedDate = null;
                    return;
                }
            }

            if (dpStartDate.SelectedDate.HasValue)
            {
                if (CurrentBooking == null)
                    CurrentBooking = new Booking();

                CurrentBooking.StartDate = dpStartDate.SelectedDate.Value;
            }

            ShowAvailableWorkspaces();
        }

        private void dpEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpEndDate.SelectedDate.HasValue && dpStartDate.SelectedDate.HasValue)
            {
                if (dpEndDate.SelectedDate.Value.Date <= dpStartDate.SelectedDate.Value.Date)
                {
                    MessageBox.Show("End date must be after start date.");
                    dpEndDate.SelectedDate = null;
                    return;
                }
            }

            if (dpEndDate.SelectedDate.HasValue)
            {
                if (CurrentBooking == null)
                    CurrentBooking = new Booking();

                CurrentBooking.EndDate = dpEndDate.SelectedDate.Value;
            }

            ShowAvailableWorkspaces();
        }

        private void btnBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentBooking != null)
                {
                    if (cbxUser.SelectedItem == null)
                    {
                        MessageBox.Show("Please select a user.");
                        return;
                    }

                    // Ensure UserID is set
                    var selectedUser = cbxUser.SelectedItem as dynamic;
                    if (selectedUser != null)
                    {
                        CurrentBooking.UserID = selectedUser.UserID;
                    }

                    db.Bookings.Add(CurrentBooking);
                    db.SaveChanges();
                    DisplayAllBookings(); // refresh bookings data grid

                    MessageBox.Show("The workspace has been booked!");
                }
                else
                {
                    MessageBox.Show("Please select a workspace and date range first.");
                }
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"An error occurred while booking the workspace: {innerException}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while booking the workspace: {ex.Message}");
            }
        }

        private void btnSearchWorkspaces_Click(object sender, RoutedEventArgs e)
        {
            ShowAvailableWorkspaces();
        }

        private void cbxUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxUser.SelectedItem != null)
            {
                var selectedUser = cbxUser.SelectedItem as dynamic;

                if (selectedUser != null)
                {
                    if (CurrentBooking == null)
                    {
                        CurrentBooking = new Booking();
                    }

                    CurrentBooking.UserID = selectedUser.UserID;
                }
                else
                {
                    MessageBox.Show("Selected user is invalid.");
                }
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addUserWindow = new AddUserWindow(db);
            addUserWindow.ShowDialog();

            var users = from u in db.Users
                        select new { u.UserID, FullName = u.FirstName + " " + u.LastName };

            cbxUser.ItemsSource = users.ToList();
            cbxUser.DisplayMemberPath = "FullName";
            cbxUser.SelectedValuePath = "UserID";
        }
    }
}
