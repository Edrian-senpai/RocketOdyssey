using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RocketOdyssey.Database;

namespace RocketOdyssey
{
    public partial class RegisterForm : Form
    {
        

        public RegisterForm()
        {
            InitializeComponent();
            pnRegister.BackColor = Color.FromArgb(100, 0, 0, 0);
        }

        private void lnLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }

         private void btnR_ShowPassword_Click(object sender, EventArgs e)
         {
             // Toggle password visibility
             txtR_Password.PasswordChar = !txtR_Password.PasswordChar;

             // Swap image based on visibility
             if (txtR_Password.PasswordChar)
             {
                 // Password is hidden
                 btnR_ShowPassword.Image = Properties.Resources.hidepass_icon;
             }
             else
             {
                 // Password is visible
                 btnR_ShowPassword.Image = Properties.Resources.showpass_icon;
             }
         }

         private void btnR_ShowConfirmPassword_Click(object sender, EventArgs e)
         {
             // Toggle password visibility
             txtR_ConfirmPassword.PasswordChar = !txtR_ConfirmPassword.PasswordChar;

             // Swap image based on visibility
             if (txtR_ConfirmPassword.PasswordChar)
             {
                 // Password is hidden
                 btnR_ShowConfirmPassword.Image = Properties.Resources.hidepass_icon;
             }
             else
             {
                 // Password is visible
                 btnR_ShowConfirmPassword.Image = Properties.Resources.showpass_icon;
             }
         }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Use Content instead of Text
            string username = txtR_Username.Content.Trim();
            string password = txtR_Password.Content;
            string confirmPassword = txtR_ConfirmPassword.Content;

            // Basic validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match!");
                return;
            }

            // Attempt registration
            bool success = DatabaseHelper.RegisterUser(username, password);
            if (success)
            {
                MessageBox.Show("Registration successful!");
                this.DialogResult = DialogResult.OK;   // go back to login
                this.Close();
            }
            else
            {
                MessageBox.Show("Username already exists. Please choose another.");
            }
        }

    }
}
