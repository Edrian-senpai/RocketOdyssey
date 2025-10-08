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
            this.pnLeaderboard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tblLeaderboards)).BeginInit();
            this.SuspendLayout();
            // 
            // pnLeaderboard
            // 
            this.pnLeaderboard.BackColor = System.Drawing.SystemColors.Control;
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
            this.tblLeaderboards.Size = new System.Drawing.Size(441, 429);
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
    }
}
