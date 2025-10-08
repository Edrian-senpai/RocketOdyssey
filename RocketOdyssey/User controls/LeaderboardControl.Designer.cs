namespace RocketOdyssey.User_controls
{
    partial class LeaderboardControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnLeaderboard = new CuoreUI.Controls.cuiPanel();
            this.tblLeaderboards = new System.Windows.Forms.DataGridView();
            this.lblUpgrades = new CuoreUI.Controls.cuiLabel();
            this.btnBack = new CuoreUI.Controls.cuiButton();
            this.pnLeaderboard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tblLeaderboards)).BeginInit();
            this.SuspendLayout();
            // 
            // pnLeaderboard
            // 
            this.pnLeaderboard.BackColor = System.Drawing.SystemColors.Control;
            this.pnLeaderboard.Controls.Add(this.btnBack);
            this.pnLeaderboard.Controls.Add(this.tblLeaderboards);
            this.pnLeaderboard.Controls.Add(this.lblUpgrades);
            this.pnLeaderboard.Location = new System.Drawing.Point(123, 89);
            this.pnLeaderboard.Name = "pnLeaderboard";
            this.pnLeaderboard.OutlineThickness = 0.5F;
            this.pnLeaderboard.PanelColor = System.Drawing.Color.Transparent;
            this.pnLeaderboard.PanelOutlineColor = System.Drawing.Color.LightGray;
            this.pnLeaderboard.Rounding = new System.Windows.Forms.Padding(8);
            this.pnLeaderboard.Size = new System.Drawing.Size(475, 542);
            this.pnLeaderboard.TabIndex = 6;
            // 
            // tblLeaderboards
            // 
            this.tblLeaderboards.BackgroundColor = System.Drawing.Color.DarkGray;
            this.tblLeaderboards.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tblLeaderboards.Location = new System.Drawing.Point(16, 95);
            this.tblLeaderboards.Name = "tblLeaderboards";
            this.tblLeaderboards.RowHeadersWidth = 62;
            this.tblLeaderboards.RowTemplate.Height = 28;
            this.tblLeaderboards.Size = new System.Drawing.Size(441, 405);
            this.tblLeaderboards.TabIndex = 6;
            // 
            // lblUpgrades
            // 
            this.lblUpgrades.BackColor = System.Drawing.Color.Transparent;
            this.lblUpgrades.BackgroundImage = global::RocketOdyssey.Properties.Resources.Leaderboards_Logo;
            this.lblUpgrades.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lblUpgrades.Content = "";
            this.lblUpgrades.Font = new System.Drawing.Font("Script MT Bold", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpgrades.ForeColor = System.Drawing.Color.White;
            this.lblUpgrades.HorizontalAlignment = System.Drawing.StringAlignment.Center;
            this.lblUpgrades.Location = new System.Drawing.Point(115, 5);
            this.lblUpgrades.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblUpgrades.Name = "lblUpgrades";
            this.lblUpgrades.Size = new System.Drawing.Size(254, 113);
            this.lblUpgrades.TabIndex = 5;
            this.lblUpgrades.VerticalAlignment = System.Drawing.StringAlignment.Center;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.Transparent;
            this.btnBack.CheckButton = false;
            this.btnBack.Checked = false;
            this.btnBack.CheckedBackground = System.Drawing.Color.Transparent;
            this.btnBack.CheckedForeColor = System.Drawing.Color.Transparent;
            this.btnBack.CheckedImageTint = System.Drawing.Color.Transparent;
            this.btnBack.CheckedOutline = System.Drawing.Color.Transparent;
            this.btnBack.Content = "";
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnBack.Font = new System.Drawing.Font("Palatino Linotype", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.ForeColor = System.Drawing.Color.Transparent;
            this.btnBack.HoverBackground = System.Drawing.Color.Silver;
            this.btnBack.HoverForeColor = System.Drawing.Color.White;
            this.btnBack.HoverImageTint = System.Drawing.Color.Silver;
            this.btnBack.HoverOutline = System.Drawing.Color.Transparent;
            this.btnBack.Image = global::RocketOdyssey.Properties.Resources.Back_icon;
            this.btnBack.ImageAutoCenter = true;
            this.btnBack.ImageExpand = new System.Drawing.Point(0, 0);
            this.btnBack.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnBack.Location = new System.Drawing.Point(3, 0);
            this.btnBack.Name = "btnBack";
            this.btnBack.NormalBackground = System.Drawing.Color.Transparent;
            this.btnBack.NormalForeColor = System.Drawing.Color.Transparent;
            this.btnBack.NormalImageTint = System.Drawing.Color.White;
            this.btnBack.NormalOutline = System.Drawing.Color.Transparent;
            this.btnBack.OutlineThickness = 1F;
            this.btnBack.PressedBackground = System.Drawing.Color.LightGray;
            this.btnBack.PressedForeColor = System.Drawing.Color.LightGray;
            this.btnBack.PressedImageTint = System.Drawing.Color.LightGray;
            this.btnBack.PressedOutline = System.Drawing.Color.Transparent;
            this.btnBack.Rounding = new System.Windows.Forms.Padding(8);
            this.btnBack.Size = new System.Drawing.Size(51, 45);
            this.btnBack.TabIndex = 26;
            this.btnBack.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnBack.TextOffset = new System.Drawing.Point(0, 0);
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // LeaderboardControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackgroundImage = global::RocketOdyssey.Properties.Resources.MoonOrbit;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.pnLeaderboard);
            this.Name = "LeaderboardControl";
            this.Size = new System.Drawing.Size(720, 720);
            this.pnLeaderboard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tblLeaderboards)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CuoreUI.Controls.cuiPanel pnLeaderboard;
        private System.Windows.Forms.DataGridView tblLeaderboards;
        private CuoreUI.Controls.cuiLabel lblUpgrades;
        private CuoreUI.Controls.cuiButton btnBack;
    }
}
