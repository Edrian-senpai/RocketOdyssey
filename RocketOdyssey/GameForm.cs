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
    public partial class GameForm : Form
    {
        private MainMenuControl mainMenuControl;
        public GameForm()
        {
            InitializeComponent();


        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            mainMenuControl = new MainMenuControl();
            panelMain.Controls.Add(mainMenuControl);
            mainMenuControl.Focus();

        }
    }
}
