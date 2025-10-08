using System;
using System.Data.SQLite;
using System.IO;

namespace RocketOdyssey.Database
{
    public static class DatabaseHelper
    {
        private static readonly string dbFile = "RocketGame.db";
        private static readonly string connectionString = $"Data Source={dbFile};Version=3;";

        public static void InitializeDatabase()
        {
            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);
            }

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string createTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserID              INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username            TEXT NOT NULL UNIQUE,
                    PasswordHash        TEXT NOT NULL,
                    Score               INTEGER DEFAULT 0,
                    Coins               INTEGER DEFAULT 0,
                    RocketSpeed         INTEGER DEFAULT 0,
                    RocketArmor         INTEGER DEFAULT 100,
                    RocketWeapon        INTEGER DEFAULT 0
                );";

                using (var cmd = new SQLiteCommand(createTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }

        // Register new user
        public static bool RegisterUser(string username, string password)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    string insertQuery = @"
                        INSERT INTO Users (Username, PasswordHash)
                        VALUES (@username, @password);";

                    using (var cmd = new SQLiteCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                if (ex.ResultCode == SQLiteErrorCode.Constraint)
                    return false; // username already exists
                throw;
            }
        }

        // Validate login
        public static bool ValidateLogin(string username, string password)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT COUNT(*) FROM Users
                    WHERE Username = @username AND PasswordHash = @password;";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        // Fetch the current upgrades for a player
        public static (int speed, int armor, int weapon) GetPlayerUpgrades(string username)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT RocketSpeed, RocketArmor, RocketWeapon
                         FROM Users
                         WHERE Username = @username";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int speed = Convert.ToInt32(reader["RocketSpeed"]);
                            int armor = Convert.ToInt32(reader["RocketArmor"]);
                            int weapon = Convert.ToInt32(reader["RocketWeapon"]);
                            return (speed, armor, weapon);
                        }
                    }
                }
            }
            return (0, 100, 0); // defaults if user not found
        }

        // Update a single upgrade field
        public static void UpdateUpgrade(string username, string fieldName, int newValue)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = $"UPDATE Users SET {fieldName} = @value WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@value", newValue);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Get player's coin balance
        public static int GetPlayerCoins(string username)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT Coins FROM Users WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        // Update player's coin balance
        public static void UpdatePlayerCoins(string username, int newCoins)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"UPDATE Users SET Coins = @coins WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@coins", newCoins);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}
