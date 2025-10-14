﻿namespace RocketOdyssey
{
    partial class GameControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameControl));
            this.panelBackground = new System.Windows.Forms.Panel();
            this.pbWeapon_Logo = new System.Windows.Forms.PictureBox();
            this.pbWeapon = new CuoreUI.Controls.cuiProgressBarHorizontal();
            this.btnPause = new CuoreUI.Controls.cuiButton();
            this.pbCoin_Logo = new System.Windows.Forms.PictureBox();
            this.lblCoins = new CuoreUI.Controls.cuiLabel();
            this.lblScore = new CuoreUI.Controls.cuiLabel();
            this.pbHP_Logo = new System.Windows.Forms.PictureBox();
            this.pbFuel_Logo = new System.Windows.Forms.PictureBox();
            this.pbFuel = new CuoreUI.Controls.cuiProgressBarHorizontal();
            this.PlayerRocket = new System.Windows.Forms.PictureBox();
            this.lblHP1 = new CuoreUI.Controls.cuiLabel();
            this.pbHP = new CuoreUI.Controls.cuiProgressBarHorizontal();
            this.panelBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbWeapon_Logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCoin_Logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHP_Logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFuel_Logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlayerRocket)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBackground
            // 
            this.panelBackground.BackColor = System.Drawing.Color.Transparent;
            this.panelBackground.BackgroundImage = global::RocketOdyssey.Properties.Resources.HomeBase;
            this.panelBackground.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelBackground.Controls.Add(this.pbWeapon_Logo);
            this.panelBackground.Controls.Add(this.pbWeapon);
            this.panelBackground.Controls.Add(this.btnPause);
            this.panelBackground.Controls.Add(this.pbCoin_Logo);
            this.panelBackground.Controls.Add(this.lblCoins);
            this.panelBackground.Controls.Add(this.lblScore);
            this.panelBackground.Controls.Add(this.pbHP_Logo);
            this.panelBackground.Controls.Add(this.pbFuel_Logo);
            this.panelBackground.Controls.Add(this.pbFuel);
            this.panelBackground.Controls.Add(this.PlayerRocket);
            this.panelBackground.Controls.Add(this.lblHP1);
            this.panelBackground.Controls.Add(this.pbHP);
            this.panelBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBackground.Location = new System.Drawing.Point(0, 0);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(720, 720);
            this.panelBackground.TabIndex = 0;
            this.panelBackground.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBackground_Paint);
            // 
            // pbWeapon_Logo
            // 
            this.pbWeapon_Logo.BackColor = System.Drawing.Color.Transparent;
            this.pbWeapon_Logo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbWeapon_Logo.BackgroundImage")));
            this.pbWeapon_Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbWeapon_Logo.Location = new System.Drawing.Point(545, 48);
            this.pbWeapon_Logo.Name = "pbWeapon_Logo";
            this.pbWeapon_Logo.Size = new System.Drawing.Size(53, 30);
            this.pbWeapon_Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbWeapon_Logo.TabIndex = 28;
            this.pbWeapon_Logo.TabStop = false;
            this.pbWeapon_Logo.WaitOnLoad = true;
            // 
            // pbWeapon
            // 
            this.pbWeapon.Background = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.pbWeapon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbWeapon.Flipped = false;
            this.pbWeapon.Foreground = System.Drawing.Color.DarkGoldenrod;
            this.pbWeapon.Location = new System.Drawing.Point(600, 48);
            this.pbWeapon.MaxValue = 100;
            this.pbWeapon.Name = "pbWeapon";
            this.pbWeapon.Rounding = 8;
            this.pbWeapon.Size = new System.Drawing.Size(95, 30);
            this.pbWeapon.TabIndex = 27;
            this.pbWeapon.TabStop = false;
            this.pbWeapon.Value = 0;
            // 
            // btnPause
            // 
            this.btnPause.BackColor = System.Drawing.Color.Transparent;
            this.btnPause.CheckButton = false;
            this.btnPause.Checked = false;
            this.btnPause.CheckedBackground = System.Drawing.Color.Transparent;
            this.btnPause.CheckedForeColor = System.Drawing.Color.Transparent;
            this.btnPause.CheckedImageTint = System.Drawing.Color.Transparent;
            this.btnPause.CheckedOutline = System.Drawing.Color.Transparent;
            this.btnPause.Content = "";
            this.btnPause.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPause.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnPause.Font = new System.Drawing.Font("Palatino Linotype", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPause.ForeColor = System.Drawing.Color.Transparent;
            this.btnPause.HoverBackground = System.Drawing.Color.Silver;
            this.btnPause.HoverForeColor = System.Drawing.Color.White;
            this.btnPause.HoverImageTint = System.Drawing.Color.Silver;
            this.btnPause.HoverOutline = System.Drawing.Color.Transparent;
            this.btnPause.Image = global::RocketOdyssey.Properties.Resources.Pause_icon;
            this.btnPause.ImageAutoCenter = true;
            this.btnPause.ImageExpand = new System.Drawing.Point(10, 10);
            this.btnPause.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnPause.Location = new System.Drawing.Point(326, 10);
            this.btnPause.Name = "btnPause";
            this.btnPause.NormalBackground = System.Drawing.Color.Transparent;
            this.btnPause.NormalForeColor = System.Drawing.Color.Transparent;
            this.btnPause.NormalImageTint = System.Drawing.Color.White;
            this.btnPause.NormalOutline = System.Drawing.Color.Transparent;
            this.btnPause.OutlineThickness = 1F;
            this.btnPause.PressedBackground = System.Drawing.Color.LightGray;
            this.btnPause.PressedForeColor = System.Drawing.Color.LightGray;
            this.btnPause.PressedImageTint = System.Drawing.Color.LightGray;
            this.btnPause.PressedOutline = System.Drawing.Color.Transparent;
            this.btnPause.Rounding = new System.Windows.Forms.Padding(8);
            this.btnPause.Size = new System.Drawing.Size(50, 45);
            this.btnPause.TabIndex = 26;
            this.btnPause.TabStop = false;
            this.btnPause.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnPause.TextOffset = new System.Drawing.Point(0, 0);
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // pbCoin_Logo
            // 
            this.pbCoin_Logo.BackColor = System.Drawing.Color.Transparent;
            this.pbCoin_Logo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbCoin_Logo.BackgroundImage")));
            this.pbCoin_Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbCoin_Logo.Location = new System.Drawing.Point(542, 0);
            this.pbCoin_Logo.Name = "pbCoin_Logo";
            this.pbCoin_Logo.Size = new System.Drawing.Size(56, 33);
            this.pbCoin_Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCoin_Logo.TabIndex = 22;
            this.pbCoin_Logo.TabStop = false;
            this.pbCoin_Logo.WaitOnLoad = true;
            // 
            // lblCoins
            // 
            this.lblCoins.Content = "---";
            this.lblCoins.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCoins.ForeColor = System.Drawing.Color.White;
            this.lblCoins.HorizontalAlignment = System.Drawing.StringAlignment.Near;
            this.lblCoins.Location = new System.Drawing.Point(600, 3);
            this.lblCoins.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblCoins.Name = "lblCoins";
            this.lblCoins.Size = new System.Drawing.Size(97, 30);
            this.lblCoins.TabIndex = 21;
            this.lblCoins.TabStop = false;
            this.lblCoins.VerticalAlignment = System.Drawing.StringAlignment.Center;
            // 
            // lblScore
            // 
            this.lblScore.Content = "Score:\\ \\ ---";
            this.lblScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScore.ForeColor = System.Drawing.Color.White;
            this.lblScore.HorizontalAlignment = System.Drawing.StringAlignment.Near;
            this.lblScore.Location = new System.Drawing.Point(0, 5);
            this.lblScore.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(187, 30);
            this.lblScore.TabIndex = 1;
            this.lblScore.TabStop = false;
            this.lblScore.VerticalAlignment = System.Drawing.StringAlignment.Center;
            // 
            // pbHP_Logo
            // 
            this.pbHP_Logo.BackColor = System.Drawing.Color.Transparent;
            this.pbHP_Logo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbHP_Logo.BackgroundImage")));
            this.pbHP_Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbHP_Logo.Location = new System.Drawing.Point(0, 48);
            this.pbHP_Logo.Name = "pbHP_Logo";
            this.pbHP_Logo.Size = new System.Drawing.Size(56, 30);
            this.pbHP_Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbHP_Logo.TabIndex = 20;
            this.pbHP_Logo.TabStop = false;
            this.pbHP_Logo.WaitOnLoad = true;
            // 
            // pbFuel_Logo
            // 
            this.pbFuel_Logo.BackColor = System.Drawing.Color.Transparent;
            this.pbFuel_Logo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbFuel_Logo.BackgroundImage")));
            this.pbFuel_Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbFuel_Logo.Location = new System.Drawing.Point(0, 90);
            this.pbFuel_Logo.Name = "pbFuel_Logo";
            this.pbFuel_Logo.Size = new System.Drawing.Size(56, 30);
            this.pbFuel_Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbFuel_Logo.TabIndex = 18;
            this.pbFuel_Logo.TabStop = false;
            this.pbFuel_Logo.WaitOnLoad = true;
            // 
            // pbFuel
            // 
            this.pbFuel.Background = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.pbFuel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbFuel.Flipped = false;
            this.pbFuel.Foreground = System.Drawing.Color.DarkGoldenrod;
            this.pbFuel.Location = new System.Drawing.Point(62, 90);
            this.pbFuel.MaxValue = 100;
            this.pbFuel.Name = "pbFuel";
            this.pbFuel.Rounding = 8;
            this.pbFuel.Size = new System.Drawing.Size(125, 30);
            this.pbFuel.TabIndex = 17;
            this.pbFuel.TabStop = false;
            this.pbFuel.Value = 90;
            // 
            // PlayerRocket
            // 
            this.PlayerRocket.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlayerRocket.BackColor = System.Drawing.Color.Transparent;
            this.PlayerRocket.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PlayerRocket.Image = global::RocketOdyssey.Properties.Resources.rocket_2_smol;
            this.PlayerRocket.ImageLocation = "";
            this.PlayerRocket.InitialImage = null;
            this.PlayerRocket.Location = new System.Drawing.Point(326, 510);
            this.PlayerRocket.Margin = new System.Windows.Forms.Padding(0);
            this.PlayerRocket.Name = "PlayerRocket";
            this.PlayerRocket.Size = new System.Drawing.Size(50, 100);
            this.PlayerRocket.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PlayerRocket.TabIndex = 2;
            this.PlayerRocket.TabStop = false;
            this.PlayerRocket.WaitOnLoad = true;
            this.PlayerRocket.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.GameControl_PreviewKeyDown);
            // 
            // lblHP1
            // 
            this.lblHP1.BackColor = System.Drawing.Color.Transparent;
            this.lblHP1.Content = "---";
            this.lblHP1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHP1.ForeColor = System.Drawing.Color.White;
            this.lblHP1.HorizontalAlignment = System.Drawing.StringAlignment.Center;
            this.lblHP1.Location = new System.Drawing.Point(4, 147);
            this.lblHP1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblHP1.Name = "lblHP1";
            this.lblHP1.Size = new System.Drawing.Size(183, 30);
            this.lblHP1.TabIndex = 29;
            this.lblHP1.TabStop = false;
            this.lblHP1.VerticalAlignment = System.Drawing.StringAlignment.Near;
            this.lblHP1.Visible = false;
            // 
            // pbHP
            // 
            this.pbHP.Background = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.pbHP.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbHP.Flipped = false;
            this.pbHP.Foreground = System.Drawing.Color.ForestGreen;
            this.pbHP.Location = new System.Drawing.Point(62, 48);
            this.pbHP.MaxValue = 100;
            this.pbHP.Name = "pbHP";
            this.pbHP.Rounding = 8;
            this.pbHP.Size = new System.Drawing.Size(125, 30);
            this.pbHP.TabIndex = 19;
            this.pbHP.TabStop = false;
            this.pbHP.Value = 90;
            // 
            // GameControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Controls.Add(this.panelBackground);
            this.DoubleBuffered = true;
            this.Name = "GameControl";
            this.Size = new System.Drawing.Size(720, 720);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameControl_KeyDown);
            this.panelBackground.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbWeapon_Logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCoin_Logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHP_Logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFuel_Logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlayerRocket)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBackground;
        private System.Windows.Forms.PictureBox PlayerRocket;
        private CuoreUI.Controls.cuiLabel lblScore;
        private CuoreUI.Controls.cuiProgressBarHorizontal pbFuel;
        private System.Windows.Forms.PictureBox pbFuel_Logo;
        private System.Windows.Forms.PictureBox pbHP_Logo;
        private CuoreUI.Controls.cuiProgressBarHorizontal pbHP;
        private CuoreUI.Controls.cuiLabel lblCoins;
        private System.Windows.Forms.PictureBox pbCoin_Logo;
        private CuoreUI.Controls.cuiButton btnPause;
        private System.Windows.Forms.PictureBox pbWeapon_Logo;
        private CuoreUI.Controls.cuiProgressBarHorizontal pbWeapon;
        private CuoreUI.Controls.cuiLabel lblHP1;
    }
}
