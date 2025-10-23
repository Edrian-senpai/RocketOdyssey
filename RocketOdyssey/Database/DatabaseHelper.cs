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

                // Main Users table
                string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserID              INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username            TEXT NOT NULL UNIQUE,
                    PasswordHash        TEXT NOT NULL,
                    Score               INTEGER DEFAULT 0,
                    HighScore           INTEGER DEFAULT 0,
                    Coins               INTEGER DEFAULT 0,
                    RocketSpeed         INTEGER DEFAULT 0,
                    RocketArmor         INTEGER DEFAULT 100,
                    RocketWeapon        INTEGER DEFAULT 0,
                    RocketPosX          INTEGER DEFAULT 326,
                    RocketPosY          INTEGER DEFAULT 510,
                    BackgroundStage     INTEGER DEFAULT 0,
                    StageOffset         INTEGER DEFAULT 0,
                    FuelRemaining       INTEGER DEFAULT 100,
                    CurrentHP           INTEGER DEFAULT 100,
                    LaunchTimerRemaining INTEGER DEFAULT 0  
                );";
                using (var cmd = new SQLiteCommand(createUsersTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Temporary upgrades table
                string createTempUpgradesTable = @"
                CREATE TABLE IF NOT EXISTS TempUpgrades (
                    TempID        INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username      TEXT NOT NULL UNIQUE,
                    TempSpeed     INTEGER DEFAULT 0,
                    TempArmor     INTEGER DEFAULT 100,
                    TempWeapon    INTEGER DEFAULT 0,
                    FOREIGN KEY (Username) REFERENCES Users(Username)
                );";
                using (var cmd = new SQLiteCommand(createTempUpgradesTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // === Sound State Table ===
                string createSoundStateTable = @"
                CREATE TABLE IF NOT EXISTS SoundState (
                    Username TEXT PRIMARY KEY,
                    BgMusicTime REAL DEFAULT 0,
                    LaserActive INTEGER DEFAULT 0,
                    LaserTimeLeft REAL DEFAULT 0,
                    FOREIGN KEY (Username) REFERENCES Users(Username)
                );";
                using (var cmd = new SQLiteCommand(createSoundStateTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

            }
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }

        // Register new user + default TempUpgrades row
        public static bool RegisterUser(string username, string password)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        string insertUser = @"
                            INSERT INTO Users (Username, PasswordHash)
                            VALUES (@username, @password);";
                        using (var cmd = new SQLiteCommand(insertUser, conn))
                        {
                            cmd.Parameters.AddWithValue("@username", username);
                            cmd.Parameters.AddWithValue("@password", password);
                            cmd.ExecuteNonQuery();
                        }

                        string insertTemp = @"
                            INSERT INTO TempUpgrades (Username)
                            VALUES (@username);";
                        using (var cmd = new SQLiteCommand(insertTemp, conn))
                        {
                            cmd.Parameters.AddWithValue("@username", username);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }

                    return true;
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

        // === UPGRADE HANDLING ===

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
            return (0, 100, 0);
        }

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

        // === TEMPORARY UPGRADES ===

        public static void SaveTempUpgrades(string username, int tempSpeed, int tempArmor, int tempWeapon)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"
                    INSERT INTO TempUpgrades (Username, TempSpeed, TempArmor, TempWeapon)
                    VALUES (@username, @speed, @armor, @weapon)
                    ON CONFLICT(Username) DO UPDATE SET
                        TempSpeed = excluded.TempSpeed,
                        TempArmor = excluded.TempArmor,
                        TempWeapon = excluded.TempWeapon;";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@speed", tempSpeed);
                    cmd.Parameters.AddWithValue("@armor", tempArmor);
                    cmd.Parameters.AddWithValue("@weapon", tempWeapon);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static (int speed, int armor, int weapon) GetTempUpgrades(string username)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT TempSpeed, TempArmor, TempWeapon 
                                 FROM TempUpgrades WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int speed = Convert.ToInt32(reader["TempSpeed"]);
                            int armor = Convert.ToInt32(reader["TempArmor"]);
                            int weapon = Convert.ToInt32(reader["TempWeapon"]);
                            return (speed, armor, weapon);
                        }
                    }
                }
            }
            return (0, 100, 0);
        }

        // === PLAYER STATE ===

        public static void ResetPlayerProgress(string username)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    // --- Get the player's RocketArmor upgrade ---
                    int armorValue = 100;
                    string getArmorQuery = "SELECT RocketArmor FROM Users WHERE Username = @username;";
                    using (var cmd = new SQLiteCommand(getArmorQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        object result = cmd.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int armor))
                            armorValue = armor; // use the armor as max HP
                    }

                    // --- Reset player position, fuel, HP (based on armor), and launch timer ---
                    string resetUserQuery = @"
                UPDATE Users SET 
                    RocketPosX = 326, 
                    RocketPosY = 510,
                    BackgroundStage = 0,
                    StageOffset = 0,
                    FuelRemaining = 100,
                    CurrentHP = @armorValue,
                    LaunchTimerRemaining = 10
                WHERE Username = @username;";
                    using (var cmd = new SQLiteCommand(resetUserQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@armorValue", armorValue);
                        cmd.ExecuteNonQuery();
                    }

                    // --- Reset sound state table ---
                    string resetSoundQuery = @"
                INSERT INTO SoundState (Username, BgMusicTime, LaserActive, LaserTimeLeft)
                VALUES (@username, 0, 0, 0)
                ON CONFLICT(Username)
                DO UPDATE SET 
                    BgMusicTime = 0,
                    LaserActive = 0,
                    LaserTimeLeft = 0;";
                    using (var cmd = new SQLiteCommand(resetSoundQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        public static void UpdatePlayerScore(string username, int score)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"UPDATE Users SET Score = @score WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@score", score);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int LoadPlayerScore(string username)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT Score FROM Users WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }


        public static int GetHighScore(string username)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT HighScore FROM Users WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        public static void UpdateHighScore(string username, int newScore)
        {
            int currentHigh = GetHighScore(username);
            if (newScore > currentHigh)
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Users SET HighScore = @highScore WHERE Username = @username";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@highScore", newScore);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

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

        public static void SavePlayerState(string username, int posX, int posY, int stageIndex, int stageOffset)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"UPDATE Users 
                                 SET RocketPosX = @x, RocketPosY = @y, 
                                     BackgroundStage = @stage, StageOffset = @offset
                                 WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@x", posX);
                    cmd.Parameters.AddWithValue("@y", posY);
                    cmd.Parameters.AddWithValue("@stage", stageIndex);
                    cmd.Parameters.AddWithValue("@offset", stageOffset);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static (int posX, int posY, int stageIndex, int stageOffset) LoadPlayerState(string username)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT RocketPosX, RocketPosY, BackgroundStage, StageOffset 
                                 FROM Users WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int x = Convert.ToInt32(reader["RocketPosX"]);
                            int y = Convert.ToInt32(reader["RocketPosY"]);
                            int stage = Convert.ToInt32(reader["BackgroundStage"]);
                            int offset = Convert.ToInt32(reader["StageOffset"]);
                            return (x, y, stage, offset);
                        }
                    }
                }
            }
            return (326, 510, 0, 0);
        }

        public static void SavePlayerFuel(string username, int fuel)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"UPDATE Users SET FuelRemaining = @fuel WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@fuel", fuel);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int LoadPlayerFuel(string username)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT FuelRemaining FROM Users WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 100;
                }
            }
        }

        public static void SavePlayerHP(string username, int hp)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"UPDATE Users SET CurrentHP = @hp WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@hp", hp);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int LoadPlayerHP(string username)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT CurrentHP FROM Users WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 100;
                }
            }
        }

        public static void SaveLaunchTimer(string username, int secondsRemaining)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"UPDATE Users SET LaunchTimerRemaining = @seconds WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@seconds", secondsRemaining);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int LoadLaunchTimer(string username)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT LaunchTimerRemaining FROM Users WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        // === SOUND STATE HANDLING ===

        public static void SaveSoundState(string username, double bgMusicTime, bool laserActive, double laserTimeLeft)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"
            INSERT INTO SoundState (Username, BgMusicTime, LaserActive, LaserTimeLeft)
            VALUES (@u, @bg, @la, @lt)
            ON CONFLICT(Username)
            DO UPDATE SET 
                BgMusicTime = excluded.BgMusicTime,
                LaserActive = excluded.LaserActive,
                LaserTimeLeft = excluded.LaserTimeLeft;";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@bg", bgMusicTime);
                    cmd.Parameters.AddWithValue("@la", laserActive ? 1 : 0);
                    cmd.Parameters.AddWithValue("@lt", laserTimeLeft);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static (double bgMusicTime, bool laserActive, double laserTimeLeft) LoadSoundState(string username)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT BgMusicTime, LaserActive, LaserTimeLeft FROM SoundState WHERE Username = @u";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            double bg = Convert.ToDouble(reader["BgMusicTime"]);
                            bool laser = Convert.ToInt32(reader["LaserActive"]) == 1;
                            double timeLeft = Convert.ToDouble(reader["LaserTimeLeft"]);
                            return (bg, laser, timeLeft);
                        }
                    }
                }
            }
            return (0, false, 0);
        }

        public static bool HasSoundState(string username)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM SoundState WHERE Username = @u";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }


    }
}
