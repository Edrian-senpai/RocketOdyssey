using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace RocketOdyssey.User_controls
{
    public partial class LeaderboardControl : UserControl
    {
        public LeaderboardControl()
        {
            InitializeComponent();
            LoadLeaderboard();
            AddRankColumn();
            pnLeaderboard.BackColor = Color.FromArgb(100, 0, 0, 0);
        }

        private void LoadLeaderboard()
        {
            using (var conn = RocketOdyssey.Database.DatabaseHelper.GetConnection())
            {
                conn.Open();

                // Use HighScore column instead of Score
                string query = @"
                    SELECT Username, HighScore
                    FROM Users
                    ORDER BY HighScore DESC
                    LIMIT 10;";

                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(reader);
                    tblLeaderboards.DataSource = dt;
                }
            }

            // Updated header to match correct column
            tblLeaderboards.Columns["Username"].HeaderText = "Player";
            tblLeaderboards.Columns["HighScore"].HeaderText = "High Score";

            tblLeaderboards.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tblLeaderboards.RowHeadersVisible = false;
            tblLeaderboards.ReadOnly = true;
            tblLeaderboards.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void AddRankColumn()
        {
            var dt = (DataTable)tblLeaderboards.DataSource;
            dt.Columns.Add("Rank", typeof(int));

            int rank = 1;
            foreach (DataRow row in dt.Rows)
            {
                row["Rank"] = rank++;
            }

            // Move Rank column to the first position
            tblLeaderboards.Columns["Rank"].DisplayIndex = 0;
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            // Get a reference to the parent form (GameForm)
            GameForm parentForm = (GameForm)this.FindForm();

            if (parentForm != null)
            {
                // Create a new MainMenuControl instance
                MainMenuControl mainMenu = new MainMenuControl();

                // Clear the current controls (removes UpgradeControl)
                parentForm.Controls.Clear();

                // Add the MainMenuControl
                parentForm.Controls.Add(mainMenu);

                // Make sure the main menu gets focus
                mainMenu.Focus();
            }
        }
    }
}
