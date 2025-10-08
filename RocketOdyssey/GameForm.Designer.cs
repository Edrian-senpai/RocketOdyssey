namespace RocketOdyssey
{
    partial class GameForm
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
            this.panelGame = new CuoreUI.Controls.cuiPanel();
            this.cuiFormAnimator1 = new CuoreUI.Components.cuiFormAnimator(this.components);
            this.cuiFormDrag1 = new CuoreUI.Components.cuiFormDrag(this.components);
            this.panelMain = new CuoreUI.Controls.cuiPanel();
            this.SuspendLayout();
            // 
            // panelGame
            // 
            this.panelGame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGame.Location = new System.Drawing.Point(0, 0);
            this.panelGame.Name = "panelGame";
            this.panelGame.OutlineThickness = 1F;
            this.panelGame.PanelColor = System.Drawing.Color.Silver;
            this.panelGame.PanelOutlineColor = System.Drawing.Color.Black;
            this.panelGame.Rounding = new System.Windows.Forms.Padding(8);
            this.panelGame.Size = new System.Drawing.Size(698, 664);
            this.panelGame.TabIndex = 1;
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
            // cuiFormDrag1
            // 
            this.cuiFormDrag1.TargetForm = this;
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.Transparent;
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.OutlineThickness = 1F;
            this.panelMain.PanelColor = System.Drawing.Color.Transparent;
            this.panelMain.PanelOutlineColor = System.Drawing.Color.Black;
            this.panelMain.Rounding = new System.Windows.Forms.Padding(8);
            this.panelMain.Size = new System.Drawing.Size(698, 664);
            this.panelMain.TabIndex = 2;
            // 
            // GameForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(698, 664);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelGame);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rocket Odyssey";
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CuoreUI.Controls.cuiPanel panelGame;
        private CuoreUI.Components.cuiFormAnimator cuiFormAnimator1;
        private CuoreUI.Components.cuiFormDrag cuiFormDrag1;
        private CuoreUI.Controls.cuiPanel panelMain;
    }
}