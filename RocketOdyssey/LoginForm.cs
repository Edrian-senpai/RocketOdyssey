using RocketOdyssey.Database;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RocketOdyssey
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            pnLogin.BackColor = Color.FromArgb(100, 0, 0, 0);
        }

        private void lnRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide(); // Hide login form while register form is open

            using (var registerForm = new RegisterForm())
            {
                var result = registerForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Registration successful
                    MessageBox.Show("Please login with your new account.");
                }
            }

            this.Show(); // Show login form again after register form closes
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void btnShowPassword_Click(object sender, EventArgs e)
        {
            // Toggle password visibility
            txtPassword.PasswordChar = !txtPassword.PasswordChar;

            // Swap image based on visibility
            if (txtPassword.PasswordChar)
            {
                // Password is hidden
                btnShowPassword.Image = Properties.Resources.hidepass_icon;
            }
            else
            {
                // Password is visible
                btnShowPassword.Image = Properties.Resources.showpass_icon;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Use Content instead of Text
            string username = txtUsername.Content.Trim();
            string password = txtPassword.Content;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Missing Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DatabaseHelper.ValidateLogin(username, password))
            {
                SessionManager.CurrentUsername = username;

                MessageBox.Show("Login successful! Proceeding to main application...",
                    "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.",
                    "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
