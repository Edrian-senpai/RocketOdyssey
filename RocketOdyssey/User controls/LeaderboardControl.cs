using System.Data;
using System.Drawing;
using System.Data.SQLite;
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
                string query = @"
                    SELECT Username, Score
                    FROM Users
                    ORDER BY Score DESC
                    LIMIT 10;";

                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(reader);
                    tblLeaderboards.DataSource = dt;
                }
            }

            // Optional styling
            tblLeaderboards.Columns["Username"].HeaderText = "Player";
            tblLeaderboards.Columns["Score"].HeaderText = "Score";

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

    }
}
