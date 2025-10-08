namespace RocketOdyssey
{
    partial class MainMenuControl
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnStart = new CuoreUI.Controls.cuiButton();
            this.btnUpgrades = new CuoreUI.Controls.cuiButton();
            this.btnExit = new CuoreUI.Controls.cuiButton();
            this.btnLeaderboards = new CuoreUI.Controls.cuiButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Image = global::RocketOdyssey.Properties.Resources.rocket_2_land_smol;
            this.pictureBox1.ImageLocation = "";
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(326, 520);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 100);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.WaitOnLoad = true;
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.Transparent;
            this.btnStart.CheckButton = false;
            this.btnStart.Checked = false;
            this.btnStart.CheckedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.btnStart.CheckedForeColor = System.Drawing.Color.White;
            this.btnStart.CheckedImageTint = System.Drawing.Color.White;
            this.btnStart.CheckedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.btnStart.Content = "Launch!";
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStart.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnStart.Font = new System.Drawing.Font("Palatino Linotype", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.HoverBackground = System.Drawing.Color.DarkCyan;
            this.btnStart.HoverForeColor = System.Drawing.Color.LightGray;
            this.btnStart.HoverImageTint = System.Drawing.Color.DarkCyan;
            this.btnStart.HoverOutline = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnStart.Image = null;
            this.btnStart.ImageAutoCenter = true;
            this.btnStart.ImageExpand = new System.Drawing.Point(0, 0);
            this.btnStart.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnStart.Location = new System.Drawing.Point(270, 250);
            this.btnStart.Name = "btnStart";
            this.btnStart.NormalBackground = System.Drawing.Color.Teal;
            this.btnStart.NormalForeColor = System.Drawing.Color.White;
            this.btnStart.NormalImageTint = System.Drawing.Color.Teal;
            this.btnStart.NormalOutline = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnStart.OutlineThickness = 1F;
            this.btnStart.PressedBackground = System.Drawing.Color.PaleTurquoise;
            this.btnStart.PressedForeColor = System.Drawing.Color.LightGray;
            this.btnStart.PressedImageTint = System.Drawing.Color.LightGray;
            this.btnStart.PressedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnStart.Rounding = new System.Windows.Forms.Padding(8);
            this.btnStart.Size = new System.Drawing.Size(169, 45);
            this.btnStart.TabIndex = 6;
            this.btnStart.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnStart.TextOffset = new System.Drawing.Point(0, 0);
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnUpgrades
            // 
            this.btnUpgrades.BackColor = System.Drawing.Color.Transparent;
            this.btnUpgrades.CheckButton = false;
            this.btnUpgrades.Checked = false;
            this.btnUpgrades.CheckedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.btnUpgrades.CheckedForeColor = System.Drawing.Color.White;
            this.btnUpgrades.CheckedImageTint = System.Drawing.Color.White;
            this.btnUpgrades.CheckedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.btnUpgrades.Content = "Upgrades";
            this.btnUpgrades.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpgrades.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnUpgrades.Font = new System.Drawing.Font("Palatino Linotype", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpgrades.ForeColor = System.Drawing.Color.White;
            this.btnUpgrades.HoverBackground = System.Drawing.Color.Goldenrod;
            this.btnUpgrades.HoverForeColor = System.Drawing.Color.LightGray;
            this.btnUpgrades.HoverImageTint = System.Drawing.Color.Goldenrod;
            this.btnUpgrades.HoverOutline = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnUpgrades.Image = null;
            this.btnUpgrades.ImageAutoCenter = true;
            this.btnUpgrades.ImageExpand = new System.Drawing.Point(0, 0);
            this.btnUpgrades.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnUpgrades.Location = new System.Drawing.Point(270, 320);
            this.btnUpgrades.Name = "btnUpgrades";
            this.btnUpgrades.NormalBackground = System.Drawing.Color.DarkGoldenrod;
            this.btnUpgrades.NormalForeColor = System.Drawing.Color.White;
            this.btnUpgrades.NormalImageTint = System.Drawing.Color.DarkGoldenrod;
            this.btnUpgrades.NormalOutline = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnUpgrades.OutlineThickness = 1F;
            this.btnUpgrades.PressedBackground = System.Drawing.Color.Cornsilk;
            this.btnUpgrades.PressedForeColor = System.Drawing.Color.LightGray;
            this.btnUpgrades.PressedImageTint = System.Drawing.Color.Cornsilk;
            this.btnUpgrades.PressedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnUpgrades.Rounding = new System.Windows.Forms.Padding(8);
            this.btnUpgrades.Size = new System.Drawing.Size(169, 45);
            this.btnUpgrades.TabIndex = 7;
            this.btnUpgrades.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnUpgrades.TextOffset = new System.Drawing.Point(0, 0);
            this.btnUpgrades.Click += new System.EventHandler(this.btnUpgrades_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.CheckButton = false;
            this.btnExit.Checked = false;
            this.btnExit.CheckedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.btnExit.CheckedForeColor = System.Drawing.Color.White;
            this.btnExit.CheckedImageTint = System.Drawing.Color.White;
            this.btnExit.CheckedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.btnExit.Content = "Exit";
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnExit.Font = new System.Drawing.Font("Palatino Linotype", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.HoverBackground = System.Drawing.Color.Red;
            this.btnExit.HoverForeColor = System.Drawing.Color.LightGray;
            this.btnExit.HoverImageTint = System.Drawing.Color.Red;
            this.btnExit.HoverOutline = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnExit.Image = null;
            this.btnExit.ImageAutoCenter = true;
            this.btnExit.ImageExpand = new System.Drawing.Point(0, 0);
            this.btnExit.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnExit.Location = new System.Drawing.Point(270, 460);
            this.btnExit.Name = "btnExit";
            this.btnExit.NormalBackground = System.Drawing.Color.Firebrick;
            this.btnExit.NormalForeColor = System.Drawing.Color.White;
            this.btnExit.NormalImageTint = System.Drawing.Color.Firebrick;
            this.btnExit.NormalOutline = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnExit.OutlineThickness = 1F;
            this.btnExit.PressedBackground = System.Drawing.Color.RosyBrown;
            this.btnExit.PressedForeColor = System.Drawing.Color.LightGray;
            this.btnExit.PressedImageTint = System.Drawing.Color.RosyBrown;
            this.btnExit.PressedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnExit.Rounding = new System.Windows.Forms.Padding(8);
            this.btnExit.Size = new System.Drawing.Size(169, 45);
            this.btnExit.TabIndex = 8;
            this.btnExit.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnExit.TextOffset = new System.Drawing.Point(0, 0);
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnLeaderboards
            // 
            this.btnLeaderboards.BackColor = System.Drawing.Color.Transparent;
            this.btnLeaderboards.CheckButton = false;
            this.btnLeaderboards.Checked = false;
            this.btnLeaderboards.CheckedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.btnLeaderboards.CheckedForeColor = System.Drawing.Color.White;
            this.btnLeaderboards.CheckedImageTint = System.Drawing.Color.White;
            this.btnLeaderboards.CheckedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.btnLeaderboards.Content = "Leaderboards";
            this.btnLeaderboards.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLeaderboards.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnLeaderboards.Font = new System.Drawing.Font("Palatino Linotype", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLeaderboards.ForeColor = System.Drawing.Color.White;
            this.btnLeaderboards.HoverBackground = System.Drawing.Color.LimeGreen;
            this.btnLeaderboards.HoverForeColor = System.Drawing.Color.LightGray;
            this.btnLeaderboards.HoverImageTint = System.Drawing.Color.ForestGreen;
            this.btnLeaderboards.HoverOutline = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnLeaderboards.Image = null;
            this.btnLeaderboards.ImageAutoCenter = true;
            this.btnLeaderboards.ImageExpand = new System.Drawing.Point(0, 0);
            this.btnLeaderboards.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnLeaderboards.Location = new System.Drawing.Point(270, 390);
            this.btnLeaderboards.Name = "btnLeaderboards";
            this.btnLeaderboards.NormalBackground = System.Drawing.Color.ForestGreen;
            this.btnLeaderboards.NormalForeColor = System.Drawing.Color.White;
            this.btnLeaderboards.NormalImageTint = System.Drawing.Color.ForestGreen;
            this.btnLeaderboards.NormalOutline = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnLeaderboards.OutlineThickness = 1F;
            this.btnLeaderboards.PressedBackground = System.Drawing.Color.LimeGreen;
            this.btnLeaderboards.PressedForeColor = System.Drawing.Color.LightGray;
            this.btnLeaderboards.PressedImageTint = System.Drawing.Color.LimeGreen;
            this.btnLeaderboards.PressedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnLeaderboards.Rounding = new System.Windows.Forms.Padding(8);
            this.btnLeaderboards.Size = new System.Drawing.Size(169, 45);
            this.btnLeaderboards.TabIndex = 9;
            this.btnLeaderboards.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnLeaderboards.TextOffset = new System.Drawing.Point(0, 0);
            this.btnLeaderboards.Click += new System.EventHandler(this.btnLeaderboards_Click);
            // 
            // MainMenuControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImage = global::RocketOdyssey.Properties.Resources.HomeBase;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.btnLeaderboards);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnUpgrades);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.pictureBox1);
            this.Name = "MainMenuControl";
            this.Size = new System.Drawing.Size(720, 720);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private CuoreUI.Controls.cuiButton btnStart;
        private CuoreUI.Controls.cuiButton btnUpgrades;
        private CuoreUI.Controls.cuiButton btnExit;
        private CuoreUI.Controls.cuiButton btnLeaderboards;
    }
}
