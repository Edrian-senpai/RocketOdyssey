using System;
using System.Drawing;
using System.Windows.Forms;

namespace RocketOdyssey.User_controls
{
    public partial class PauseOverlayControl : UserControl
    {
        // Events so GameControl can react
        public event Action ResumeClicked;
        public event Action RestartClicked;
        public event Action MainMenuClicked;

        public PauseOverlayControl()
        {
            InitializeComponent();
            
        }


        private void btnResume_Click(object sender, EventArgs e)
        {
            ResumeClicked?.Invoke();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            RestartClicked?.Invoke();
        }

        private void btnMainMenu_Click(object sender, EventArgs e)
        {
            MainMenuClicked?.Invoke();
        }
    }
}
