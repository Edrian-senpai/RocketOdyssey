namespace RocketOdyssey
{
    partial class LoginForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.cuiFormRounder1 = new CuoreUI.Components.cuiFormRounder();
            this.cuiFormDrag1 = new CuoreUI.Components.cuiFormDrag(this.components);
            this.cuiFormAnimator1 = new CuoreUI.Components.cuiFormAnimator(this.components);
            this.pnLogin = new CuoreUI.Controls.cuiPanel();
            this.pbR_Logo = new System.Windows.Forms.PictureBox();
            this.btnShowPassword = new CuoreUI.Controls.cuiButton();
            this.lnRegister = new System.Windows.Forms.LinkLabel();
            this.lblLogin = new CuoreUI.Controls.cuiLabel();
            this.btnLogin = new CuoreUI.Controls.cuiButton();
            this.txtUsername = new CuoreUI.Controls.cuiTextBox();
            this.txtPassword = new CuoreUI.Controls.cuiTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnLogin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbR_Logo)).BeginInit();
            this.SuspendLayout();
            // 
            // cuiFormRounder1
            // 
            this.cuiFormRounder1.OutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.cuiFormRounder1.Rounding = 4;
            this.cuiFormRounder1.TargetForm = null;
            // 
            // cuiFormDrag1
            // 
            this.cuiFormDrag1.TargetForm = this;
            // 
            // cuiFormAnimator1
            // 
            this.cuiFormAnimator1.AnimateOnStart = true;
            this.cuiFormAnimator1.Duration = 1000;
            this.cuiFormAnimator1.EasingType = CuoreUI.Drawing.EasingTypes.SextInOut;
            this.cuiFormAnimator1.StartOpacity = 0D;
            this.cuiFormAnimator1.TargetForm = this;
            this.cuiFormAnimator1.TargetOpacity = 1D;
            // 
            // pnLogin
            // 
            this.pnLogin.BackColor = System.Drawing.SystemColors.Control;
            this.pnLogin.Controls.Add(this.pbR_Logo);
            this.pnLogin.Controls.Add(this.btnShowPassword);
            this.pnLogin.Controls.Add(this.lnRegister);
            this.pnLogin.Controls.Add(this.lblLogin);
            this.pnLogin.Controls.Add(this.btnLogin);
            this.pnLogin.Controls.Add(this.txtUsername);
            this.pnLogin.Controls.Add(this.txtPassword);
            this.pnLogin.Location = new System.Drawing.Point(112, 61);
            this.pnLogin.Name = "pnLogin";
            this.pnLogin.OutlineThickness = 0.5F;
            this.pnLogin.PanelColor = System.Drawing.Color.Transparent;
            this.pnLogin.PanelOutlineColor = System.Drawing.Color.LightGray;
            this.pnLogin.Rounding = new System.Windows.Forms.Padding(8);
            this.pnLogin.Size = new System.Drawing.Size(475, 542);
            this.pnLogin.TabIndex = 1;
            // 
            // pbR_Logo
            // 
            this.pbR_Logo.BackColor = System.Drawing.Color.Transparent;
            this.pbR_Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbR_Logo.Image = global::RocketOdyssey.Properties.Resources.Game_Logo;
            this.pbR_Logo.Location = new System.Drawing.Point(115, 0);
            this.pbR_Logo.Name = "pbR_Logo";
            this.pbR_Logo.Size = new System.Drawing.Size(254, 133);
            this.pbR_Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbR_Logo.TabIndex = 7;
            this.pbR_Logo.TabStop = false;
            this.pbR_Logo.WaitOnLoad = true;
            // 
            // btnShowPassword
            // 
            this.btnShowPassword.BackColor = System.Drawing.Color.Transparent;
            this.btnShowPassword.CheckButton = false;
            this.btnShowPassword.Checked = false;
            this.btnShowPassword.CheckedBackground = System.Drawing.Color.Transparent;
            this.btnShowPassword.CheckedForeColor = System.Drawing.Color.Transparent;
            this.btnShowPassword.CheckedImageTint = System.Drawing.Color.Transparent;
            this.btnShowPassword.CheckedOutline = System.Drawing.Color.Transparent;
            this.btnShowPassword.Content = "";
            this.btnShowPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShowPassword.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnShowPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.btnShowPassword.ForeColor = System.Drawing.Color.Black;
            this.btnShowPassword.HoverBackground = System.Drawing.Color.Gray;
            this.btnShowPassword.HoverForeColor = System.Drawing.Color.White;
            this.btnShowPassword.HoverImageTint = System.Drawing.Color.Gray;
            this.btnShowPassword.HoverOutline = System.Drawing.Color.Transparent;
            this.btnShowPassword.Image = global::RocketOdyssey.Properties.Resources.hidepass_icon;
            this.btnShowPassword.ImageAutoCenter = true;
            this.btnShowPassword.ImageExpand = new System.Drawing.Point(0, 0);
            this.btnShowPassword.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnShowPassword.Location = new System.Drawing.Point(377, 353);
            this.btnShowPassword.Margin = new System.Windows.Forms.Padding(0);
            this.btnShowPassword.Name = "btnShowPassword";
            this.btnShowPassword.NormalBackground = System.Drawing.Color.LightGray;
            this.btnShowPassword.NormalForeColor = System.Drawing.Color.Black;
            this.btnShowPassword.NormalImageTint = System.Drawing.Color.White;
            this.btnShowPassword.NormalOutline = System.Drawing.Color.Black;
            this.btnShowPassword.OutlineThickness = 1F;
            this.btnShowPassword.PressedBackground = System.Drawing.Color.LightGray;
            this.btnShowPassword.PressedForeColor = System.Drawing.Color.White;
            this.btnShowPassword.PressedImageTint = System.Drawing.Color.White;
            this.btnShowPassword.PressedOutline = System.Drawing.Color.Black;
            this.btnShowPassword.Rounding = new System.Windows.Forms.Padding(8, 0, 0, 8);
            this.btnShowPassword.Size = new System.Drawing.Size(56, 44);
            this.btnShowPassword.TabIndex = 6;
            this.btnShowPassword.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnShowPassword.TextOffset = new System.Drawing.Point(0, 0);
            this.btnShowPassword.Click += new System.EventHandler(this.btnShowPassword_Click);
            this.btnShowPassword.DoubleClick += new System.EventHandler(this.btnShowPassword_Click);
            // 
            // lnRegister
            // 
            this.lnRegister.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lnRegister.BackColor = System.Drawing.Color.Transparent;
            this.lnRegister.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnRegister.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lnRegister.Location = new System.Drawing.Point(115, 483);
            this.lnRegister.Name = "lnRegister";
            this.lnRegister.Size = new System.Drawing.Size(254, 43);
            this.lnRegister.TabIndex = 1;
            this.lnRegister.TabStop = true;
            this.lnRegister.Text = "New pilot? Register here!";
            this.lnRegister.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnRegister.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lnRegister.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnRegister_LinkClicked);
            // 
            // lblLogin
            // 
            this.lblLogin.BackColor = System.Drawing.Color.Transparent;
            this.lblLogin.BackgroundImage = global::RocketOdyssey.Properties.Resources.Login_Logo;
            this.lblLogin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lblLogin.Content = "";
            this.lblLogin.Font = new System.Drawing.Font("Script MT Bold", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogin.ForeColor = System.Drawing.Color.White;
            this.lblLogin.HorizontalAlignment = System.Drawing.StringAlignment.Center;
            this.lblLogin.Location = new System.Drawing.Point(115, 105);
            this.lblLogin.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(254, 113);
            this.lblLogin.TabIndex = 4;
            this.lblLogin.VerticalAlignment = System.Drawing.StringAlignment.Center;
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.Transparent;
            this.btnLogin.CheckButton = false;
            this.btnLogin.Checked = false;
            this.btnLogin.CheckedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.btnLogin.CheckedForeColor = System.Drawing.Color.White;
            this.btnLogin.CheckedImageTint = System.Drawing.Color.White;
            this.btnLogin.CheckedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.btnLogin.Content = "Lets get Started!";
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnLogin.Font = new System.Drawing.Font("Palatino Linotype", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.HoverBackground = System.Drawing.Color.MediumSlateBlue;
            this.btnLogin.HoverForeColor = System.Drawing.Color.LightGray;
            this.btnLogin.HoverImageTint = System.Drawing.Color.White;
            this.btnLogin.HoverOutline = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnLogin.Image = null;
            this.btnLogin.ImageAutoCenter = true;
            this.btnLogin.ImageExpand = new System.Drawing.Point(0, 0);
            this.btnLogin.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnLogin.Location = new System.Drawing.Point(115, 420);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.NormalBackground = System.Drawing.Color.SlateBlue;
            this.btnLogin.NormalForeColor = System.Drawing.Color.White;
            this.btnLogin.NormalImageTint = System.Drawing.Color.White;
            this.btnLogin.NormalOutline = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnLogin.OutlineThickness = 1F;
            this.btnLogin.PressedBackground = System.Drawing.Color.LightSteelBlue;
            this.btnLogin.PressedForeColor = System.Drawing.Color.White;
            this.btnLogin.PressedImageTint = System.Drawing.Color.White;
            this.btnLogin.PressedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnLogin.Rounding = new System.Windows.Forms.Padding(8);
            this.btnLogin.Size = new System.Drawing.Size(254, 45);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnLogin.TextOffset = new System.Drawing.Point(0, 0);
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtUsername
            // 
            this.txtUsername.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.txtUsername.BackgroundColor = System.Drawing.Color.LightGray;
            this.txtUsername.Content = "";
            this.txtUsername.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtUsername.FocusBackgroundColor = System.Drawing.Color.White;
            this.txtUsername.FocusImageTint = System.Drawing.Color.White;
            this.txtUsername.FocusOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.txtUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsername.ForeColor = System.Drawing.Color.Black;
            this.txtUsername.Image = ((System.Drawing.Image)(resources.GetObject("txtUsername.Image")));
            this.txtUsername.ImageExpand = new System.Drawing.Point(5, 5);
            this.txtUsername.ImageOffset = new System.Drawing.Point(-5, 0);
            this.txtUsername.Location = new System.Drawing.Point(33, 250);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(4);
            this.txtUsername.Multiline = false;
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.NormalImageTint = System.Drawing.Color.Silver;
            this.txtUsername.OutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.txtUsername.Padding = new System.Windows.Forms.Padding(82, 6, 82, 0);
            this.txtUsername.PasswordChar = false;
            this.txtUsername.PlaceholderColor = System.Drawing.Color.Black;
            this.txtUsername.PlaceholderText = "Username";
            this.txtUsername.Rounding = new System.Windows.Forms.Padding(8);
            this.txtUsername.Size = new System.Drawing.Size(400, 45);
            this.txtUsername.TabIndex = 2;
            this.txtUsername.TextOffset = new System.Drawing.Size(50, 0);
            this.txtUsername.UnderlinedStyle = true;
            // 
            // txtPassword
            // 
            this.txtPassword.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.txtPassword.BackgroundColor = System.Drawing.Color.LightGray;
            this.txtPassword.Content = "";
            this.txtPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPassword.FocusBackgroundColor = System.Drawing.Color.White;
            this.txtPassword.FocusImageTint = System.Drawing.Color.White;
            this.txtPassword.FocusOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.ForeColor = System.Drawing.Color.Black;
            this.txtPassword.Image = ((System.Drawing.Image)(resources.GetObject("txtPassword.Image")));
            this.txtPassword.ImageExpand = new System.Drawing.Point(5, 5);
            this.txtPassword.ImageOffset = new System.Drawing.Point(-5, 0);
            this.txtPassword.Location = new System.Drawing.Point(33, 352);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4);
            this.txtPassword.Multiline = false;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.NormalImageTint = System.Drawing.Color.Silver;
            this.txtPassword.OutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.txtPassword.Padding = new System.Windows.Forms.Padding(82, 6, 82, 0);
            this.txtPassword.PasswordChar = true;
            this.txtPassword.PlaceholderColor = System.Drawing.Color.Black;
            this.txtPassword.PlaceholderText = "Password";
            this.txtPassword.Rounding = new System.Windows.Forms.Padding(8);
            this.txtPassword.Size = new System.Drawing.Size(400, 45);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.TextOffset = new System.Drawing.Size(50, 0);
            this.txtPassword.UnderlinedStyle = true;
            // 
            // LoginForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackgroundImage = global::RocketOdyssey.Properties.Resources.HomeBase;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(698, 664);
            this.Controls.Add(this.pnLogin);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rocket Odyssey";
            this.pnLogin.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbR_Logo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CuoreUI.Components.cuiFormRounder cuiFormRounder1;
        private CuoreUI.Components.cuiFormDrag cuiFormDrag1;
        private CuoreUI.Components.cuiFormAnimator cuiFormAnimator1;
        private CuoreUI.Controls.cuiPanel pnLogin;
        private CuoreUI.Controls.cuiButton btnShowPassword;
        private System.Windows.Forms.LinkLabel lnRegister;
        private CuoreUI.Controls.cuiLabel lblLogin;
        private CuoreUI.Controls.cuiButton btnLogin;
        private CuoreUI.Controls.cuiTextBox txtUsername;
        private CuoreUI.Controls.cuiTextBox txtPassword;
        private System.Windows.Forms.PictureBox pbR_Logo;
        private System.Windows.Forms.Timer timer1;
    }
}

