using RocketOdyssey.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketOdyssey
{
    public partial class UpgradeControl : UserControl
    {
        // Store upgrade levels locally
        private int rocketSpeed;
        private int rocketArmor;
        private int rocketWeapon;

        public UpgradeControl()
        {
            InitializeComponent();
            pnUpgrades.BackColor = Color.FromArgb(100, 0, 0, 0);
        }

        // Load upgrades when this control is displayed
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!string.IsNullOrEmpty(SessionManager.CurrentUsername))
            {
                // Load upgrades from DB
                var (speed, armor, weapon) = DatabaseHelper.GetPlayerUpgrades(SessionManager.CurrentUsername);
                rocketSpeed = speed;
                rocketArmor = armor;
                rocketWeapon = weapon;

                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            // ✅ Update labels / progress bars
            lblSpeed.Text = $"Speed: {1 + rocketSpeed}";   // default speed is 1
            lblArmor.Text = $"Armor: {rocketArmor} HP";
            lblWeapon.Text = $"Weapon: Level {rocketWeapon}";
        }

        private void btnSpeed_Click(object sender, EventArgs e)
        {
            if (rocketSpeed < 4) // max level 4
            {
                rocketSpeed++;
                DatabaseHelper.UpdateUpgrade(SessionManager.CurrentUsername, "RocketSpeedUpgrade", rocketSpeed);
                UpdateUI();
            }
            else
            {
                MessageBox.Show("Speed is already at max level!");
            }
        }

        private void btnArmor_Click(object sender, EventArgs e)
        {
            if (rocketArmor < 200) // example: cap armor at 200
            {
                rocketArmor += 25;  // example increment
                DatabaseHelper.UpdateUpgrade(SessionManager.CurrentUsername, "RocketArmor", rocketArmor);
                UpdateUI();
            }
            else
            {
                MessageBox.Show("Armor is already at max HP!");
            }
        }

        private void btnWeapon_Click(object sender, EventArgs e)
        {
            if (rocketWeapon < 4)
            {
                rocketWeapon++;
                DatabaseHelper.UpdateUpgrade(SessionManager.CurrentUsername, "RocketWeapon", rocketWeapon);
                UpdateUI();
            }
            else
            {
                MessageBox.Show("Weapon is already at max level!");
            }
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            // Get a reference to the parent form (GameForm)
            GameForm parentForm = (GameForm)this.FindForm();

            if (parentForm != null)
            {
                // Create a new MainMenuControl instance
                MainMenuControl mainMenu = new MainMenuControl();

                // Clear the current controls (removes UpgradeControl)
                parentForm.Controls.Clear();

                // Add the MainMenuControl
                parentForm.Controls.Add(mainMenu);

                // Make sure the main menu gets focus
                mainMenu.Focus();
            }
        }
    }
}
