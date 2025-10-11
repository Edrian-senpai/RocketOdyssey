using RocketOdyssey.Database;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RocketOdyssey
{
    public partial class UpgradeControl : UserControl
    {
        // Permanent upgrades (purchased)
        private int purchasedSpeed;
        private int purchasedArmor;
        private int purchasedWeapon;

        // Temporary upgrades (adjustable with + / -)
        private int tempSpeed;
        private int tempArmor;
        private int tempWeapon;

        private int playerCoins;
        private string currentUser;

        private int baseSpeedPrice = 150;
        private int baseArmorPrice = 150;
        private int baseWeaponPrice = 1000;

        public UpgradeControl()
        {
            InitializeComponent();
            pnUpgrades.BackColor = Color.FromArgb(100, 0, 0, 0);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            currentUser = SessionManager.CurrentUsername;

            // --- Load permanent upgrades ---
            var (speed, armor, weapon) = DatabaseHelper.GetPlayerUpgrades(currentUser);
            purchasedSpeed = speed;
            purchasedArmor = armor;
            purchasedWeapon = weapon;

            // --- Load temporary upgrades ---
            var (tSpeed, tArmor, tWeapon) = DatabaseHelper.GetTempUpgrades(currentUser);
            tempSpeed = tSpeed;
            tempArmor = tArmor;
            tempWeapon = tWeapon;

            // Clamp temp upgrades so they can’t exceed permanent upgrades
            if (tempSpeed > purchasedSpeed) tempSpeed = purchasedSpeed;
            if (tempArmor > purchasedArmor) tempArmor = purchasedArmor;
            if (tempWeapon > purchasedWeapon) tempWeapon = purchasedWeapon;

            // --- Load player coins ---
            playerCoins = DatabaseHelper.GetPlayerCoins(currentUser);

            // --- Setup progress bars ---
            pbSpeed.MaxValue = 4;
            pbArmor.MaxValue = 4;  // Represents 100–200 armor in 25 increments
            pbWeapon.MaxValue = 4;

            UpdateUI();
        }

        // =====================================================
        //  UI Update
        // =====================================================
        private void UpdateUI()
        {
            lblCoins.Content = $"{FormatCoins(playerCoins)}";

            int armorSteps = (tempArmor - 100) / 25;
            armorSteps = Math.Max(0, Math.Min(armorSteps, 4));

            pbSpeed.Value = tempSpeed;
            pbArmor.Value = armorSteps;
            pbWeapon.Value = tempWeapon;

            UpdateBarColor(pbSpeed, tempSpeed, pbSpeed.MaxValue);
            UpdateBarColor(pbArmor, pbArmor.Value, pbArmor.MaxValue);
            UpdateBarColor(pbWeapon, tempWeapon, pbWeapon.MaxValue);

            // --- Price labels (for permanent upgrades only) ---
            lblSpeedPrice.Content = purchasedSpeed >= pbSpeed.MaxValue
                ? "MAXED"
                : $"{GetUpgradePrice(baseSpeedPrice, purchasedSpeed)}  Coins";

            int armorLevel = (purchasedArmor - 100) / 25;
            lblArmorPrice.Content = purchasedArmor >= 200
                ? "MAXED"
                : $"{GetUpgradePrice(baseArmorPrice, armorLevel)} Coins";

            lblWeaponPrice.Content = purchasedWeapon >= pbWeapon.MaxValue
                ? "MAXED"
                : $"{GetUpgradePrice(baseWeaponPrice, purchasedWeapon)} Coins";

            // --- Enable/disable temp buttons ---
            btnAddSpeed.Enabled = tempSpeed < purchasedSpeed;  // limit to perm value
            btnSubSpeed.Enabled = tempSpeed > 0;

            btnAddArmor.Enabled = tempArmor < purchasedArmor;  // limit to perm value
            btnSubArmor.Enabled = tempArmor > 100;

            btnAddWeapon.Enabled = tempWeapon < purchasedWeapon; // limit to perm value
            btnSubWeapon.Enabled = tempWeapon > 0;

            UpdateButtonStyle(btnAddSpeed);
            UpdateButtonStyle(btnSubSpeed);
            UpdateButtonStyle(btnAddArmor);
            UpdateButtonStyle(btnSubArmor);
            UpdateButtonStyle(btnAddWeapon);
            UpdateButtonStyle(btnSubWeapon);

            SyncTempValues();
        }
        private string FormatCoins(int coins)
        {
            if (coins >= 1_000_000) return $"{coins / 1_000_000.0:F1}M";
            if (coins >= 1_000) return $"{coins / 1_000.0:F1}K";
            return coins.ToString();
        }
        private void UpdateButtonStyle(dynamic btn)
        {
            if (btn.Enabled)
            {
                btn.NormalBackground = Color.DarkGoldenrod;
                btn.NormalForeColor = Color.White;
            }
            else
            {
                btn.NormalBackground = Color.DarkGray;
                btn.NormalForeColor = Color.Black;
            }
        }

        private void UpdateBarColor(dynamic pb, int value, int max)
        {
            if (max == 0) return;
            double ratio = (double)value / max;
            if (ratio < 0.33)
                pb.ForeColor = Color.LightGray;
            else if (ratio < 0.66)
                pb.ForeColor = Color.SkyBlue;
            else if (ratio < 0.9)
                pb.ForeColor = Color.MediumSeaGreen;
            else
                pb.ForeColor = Color.Gold;
        }

        private int GetUpgradePrice(int basePrice, int level)
        {
            return basePrice + (level * 50);
        }

        // =====================================================
        //  TEMPORARY ADJUSTER BUTTONS (+ / -)
        // =====================================================
        private void btnAddSpeed_Click(object sender, EventArgs e)
        {
            if (tempSpeed < purchasedSpeed)
            {
                tempSpeed++;
                UpdateUI();
            }
        }

        private void btnSubSpeed_Click(object sender, EventArgs e)
        {
            if (tempSpeed > 0)
            {
                tempSpeed--;
                UpdateUI();
            }
        }

        private void btnAddArmor_Click(object sender, EventArgs e)
        {
            if (tempArmor < purchasedArmor)
            {
                tempArmor += 25;
                UpdateUI();
            }
        }

        private void btnSubArmor_Click(object sender, EventArgs e)
        {
            if (tempArmor > 100)
            {
                tempArmor -= 25;
                UpdateUI();
            }
        }

        private void btnAddWeapon_Click(object sender, EventArgs e)
        {
            if (tempWeapon < purchasedWeapon)
            {
                tempWeapon++;
                UpdateUI();
            }
        }

        private void btnSubWeapon_Click(object sender, EventArgs e)
        {
            if (tempWeapon > 0)
            {
                tempWeapon--;
                UpdateUI();
            }
        }

        // =====================================================
        //  PERMANENT UPGRADE BUTTONS (COIN-BASED)
        // =====================================================
        private void btnSpeed_Click(object sender, EventArgs e)
        {
            int price = GetUpgradePrice(baseSpeedPrice, purchasedSpeed);
            if (playerCoins >= price && purchasedSpeed < pbSpeed.MaxValue)
            {
                playerCoins -= price;
                purchasedSpeed++;
                DatabaseHelper.UpdateUpgrade(currentUser, "RocketSpeed", purchasedSpeed);
                DatabaseHelper.UpdatePlayerCoins(currentUser, playerCoins);

                // Re-limit temp values to new max
                if (tempSpeed > purchasedSpeed) tempSpeed = purchasedSpeed;
                // Auto-sync temp level to new permanent value
                tempSpeed = purchasedSpeed;
                UpdateUI();
            }
            else if (purchasedSpeed >= pbSpeed.MaxValue)
                MessageBox.Show("Speed is already maxed out!");
            else
                MessageBox.Show("Not enough coins to upgrade Speed!");
        }

        private void btnArmor_Click(object sender, EventArgs e)
        {
            int armorLevel = (purchasedArmor - 100) / 25;
            int price = GetUpgradePrice(baseArmorPrice, armorLevel);
            if (playerCoins >= price && purchasedArmor < 200)
            {
                playerCoins -= price;
                purchasedArmor += 25;
                DatabaseHelper.UpdateUpgrade(currentUser, "RocketArmor", purchasedArmor);
                DatabaseHelper.UpdatePlayerCoins(currentUser, playerCoins);

                if (tempArmor > purchasedArmor) tempArmor = purchasedArmor;
                // Auto-sync temp armor to match permanent level
                tempArmor = purchasedArmor;
                UpdateUI();
            }
            else if (purchasedArmor >= 200)
                MessageBox.Show("Armor is already maxed out!");
            else
                MessageBox.Show("Not enough coins to upgrade Armor!");
        }

        private void btnWeapon_Click(object sender, EventArgs e)
        {
            int price = GetUpgradePrice(baseWeaponPrice, purchasedWeapon);
            if (playerCoins >= price && purchasedWeapon < pbWeapon.MaxValue)
            {
                playerCoins -= price;
                purchasedWeapon++;
                DatabaseHelper.UpdateUpgrade(currentUser, "RocketWeapon", purchasedWeapon);
                DatabaseHelper.UpdatePlayerCoins(currentUser, playerCoins);

                if (tempWeapon > purchasedWeapon) tempWeapon = purchasedWeapon;
                // Auto-sync temp weapon to new permanent level
                tempWeapon = purchasedWeapon;
                UpdateUI();
            }
            else if (purchasedWeapon >= pbWeapon.MaxValue)
                MessageBox.Show("Weapon is already maxed out!");
            else
                MessageBox.Show("Not enough coins to upgrade Weapon!");
        }

        // =====================================================
        //  TEMPORARY VALUE SYNC
        // =====================================================
        public static int TempSpeed { get; private set; } = 0;
        public static int TempArmor { get; private set; } = 100;
        public static int TempWeapon { get; private set; } = 0;

        private void SyncTempValues()
        {
            TempSpeed = tempSpeed;
            TempArmor = tempArmor;
            TempWeapon = tempWeapon;
        }

        // =====================================================
        //  BACK BUTTON
        // =====================================================
        private void btnBack_Click(object sender, EventArgs e)
        {
            // Save only the temporary upgrades
            DatabaseHelper.SaveTempUpgrades(currentUser, TempSpeed, TempArmor, TempWeapon);

            // Go back to main menu
            MainMenuControl menu = new MainMenuControl();
            GameForm parentForm = (GameForm)this.FindForm();
            parentForm.Controls.Clear();
            parentForm.Controls.Add(menu);
        }
    }
}
