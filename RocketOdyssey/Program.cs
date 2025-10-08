using System;
using System.Windows.Forms;
using RocketOdyssey.Database;

namespace RocketOdyssey
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create the database file and table if they don’t exist
            DatabaseHelper.InitializeDatabase();

            // Show the login form
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new GameForm()); // runs your main game window
                }
                else
                {
                    // Login canceled or failed — exit app
                    return;
                }
            }
        }
    }
}
