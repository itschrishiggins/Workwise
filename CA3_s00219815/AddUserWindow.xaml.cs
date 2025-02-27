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
    public partial class AddUserWindow : Window
    {
        private BookingModelContainer db;

        public AddUserWindow(BookingModelContainer dbContext)
        {
            InitializeComponent();
            db = dbContext;
        }

        private void btnSaveUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string firstName = txtFirstName.Text;
                string lastName = txtLastName.Text;
                string email = txtEmail.Text;

                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                User newUser = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                MessageBox.Show("User added successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the user: {ex.Message}");
            }
        }
    }
}

