using RocketOdyssey.User_controls;
using System;
using System.Windows.Forms;

namespace RocketOdyssey
{
    public partial class MainMenuControl : UserControl
    {
        public MainMenuControl()
        {
            InitializeComponent();
        }

        // Event handler for the Start button click
        private void btnStart_Click(object sender, EventArgs e)
        {
            // Create an instance of the GameControl
            GameControl gameControl = new GameControl();

            // Get a reference to the parent form (GameForm)
            GameForm parentForm = (GameForm)this.FindForm();

            // Clear all existing controls on the form (i.e., remove MainMenuControl)
            parentForm.Controls.Clear();

            // Add the GameControl to the form
            parentForm.Controls.Add(gameControl);

            gameControl.Focus();
        }

        // Event handler for the Upgrades button (can be implemented later)
        private void btnUpgrades_Click(object sender, EventArgs e)
        {
            // Create an instance of the GameControl
            UpgradeControl upgradeControl = new UpgradeControl();

            // Get a reference to the parent form (GameForm)
            GameForm parentForm = (GameForm)this.FindForm();

            // Clear all existing controls on the form (i.e., remove MainMenuControl)
            parentForm.Controls.Clear();

            // Add the GameControl to the form
            parentForm.Controls.Add(upgradeControl);
        }

        // Event handler for the Leaderboards button (can be implemented later)
        private void btnLeaderboards_Click(object sender, EventArgs e)
        {
            // Create an instance of the GameControl
            LeaderboardControl leaderboardControl = new LeaderboardControl();

            // Get a reference to the parent form (GameForm)
            GameForm parentForm = (GameForm)this.FindForm();

            // Clear all existing controls on the form (i.e., remove MainMenuControl)
            parentForm.Controls.Clear();

            // Add the GameControl to the form
            parentForm.Controls.Add(leaderboardControl);
        }

        // Event handler for the Log out button 
        private void btnLogOut_Click(object sender, EventArgs e)
        {
            // Ask for confirmation
            DialogResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Log Out",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {

                // Close the game form and return to login
                GameForm parentForm = (GameForm)this.FindForm();

                // Hide GameForm first (to avoid flashing)
                parentForm.Hide();

                using (var loginForm = new LoginForm())
                {
                    // Show login again as modal
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        // User logged in again → re-open GameForm
                        var newGameForm = new GameForm();
                        newGameForm.Show();
                    }
                    else
                    {
                        // User canceled login → exit app
                        Application.Exit();
                    }
                }

                // Close the old GameForm after returning from login
                parentForm.Close();
            }
            else
            {
                // User clicked No → do nothing
                return;
            }
        }

    }
}
