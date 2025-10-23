using NAudio.Wave;
using RocketOdyssey.Database;
using RocketOdyssey.User_controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using System.Windows.Forms;

namespace RocketOdyssey
{
    public partial class GameControl : UserControl
    {
        // Game state
        private bool isGameOver = false;

        // Launch countdown display
        private Label lblLaunchCountdown;
        private Timer visibleCountdownTimer;
        private int visibleCountdown = 5;

        // Background scrolling
        private Timer scrollTimer;
        private Image backgroundImage;
        private int scrollSpeed = 1; // Background scroll speed

        // Player movement state
        private bool controlsLocked = true;
        private bool rocketControlDisabled = false; // disables player input but keeps world moving
        private bool upPressed = false;
        private bool downPressed = false;
        private bool leftPressed = false;
        private bool rightPressed = false;

        private int launchCountdown = 10;
        private Timer launchDelayTimer;

        // GIF sprites
        private Image idleGif;
        private Image upGif;
        private Image downGif;
        private Image landGif;

        // Movement settings
        private int moveStep = 2;
        private readonly int minX = 0;
        private readonly int minY = 140;
        private readonly int maxX = 650;
        private readonly int maxY = 550;

        // Active rocket sprite
        private Image currentRocketSprite;
        private float currentRotation = 0f;

        // Player data
        private string currentUser;
        private int score;
        private int coins = 0;
        private int sessionCoins = 0; // coins earned this game session only
        private int hp = 100;
        private int maxHP = 100;
        private int fuel = 100;
        private int rocketSpeed = 0;
        private int rocketWeapon = 0;

        // Weapon system
        private bool weaponCharged = false;
        private bool laserActive = false;
        private PictureBox laserBeam;
        private int laserDuration = 0;

        private Timer fuelTimer;
        private int fuelDrainRate = 1;
        private int fuelDrainInterval = 500;

        private Timer moveTimer;
        private bool isOutOfFuel = false;
        private PauseOverlayControl pauseOverlay;
        private bool isPaused = false;
        private Timer laserTimer;
        private Timer laserFollowTimer;
        private Timer laserSyncTimer;

        // === Laser Beam Music System ===
        private WaveOutEvent laserOutput;
        private WaveFileReader laserReader;
        private LoopStream laserLoop;

        // === Background Music System ===
        private WaveOutEvent bgOutput;
        private AudioFileReader bgReader;
        private bool bgMusicPlaying = false;
        private double bgMusicTime = 0;
        private double laserTimeLeft = 0;

        // === Tier 1 Obstacle System ===
        private Timer obstacleSpawnTimer;
        private Random obstacleRand = new Random();
        private DateTime lastSpawnTime = DateTime.MinValue;
        private int lastSpawnY = -1;

        // === Power-up System ===
        private Timer powerupSpawnTimer;
        private Random powerupRand = new Random();
        private List<PictureBox> activePowerups = new List<PictureBox>();

        private bool focusRecoveryEnabled = true;

        public GameControl()
        {
            InitializeComponent();
            InitializeBackgrounds();
            // Ensure GameControl regains focus when any label/panel is clicked
            EnableAutomaticFocusRecovery();

            currentUser = SessionManager.CurrentUsername;

            // ---- Visible Launch Countdown Label ----
            lblLaunchCountdown = new Label
            {
                Text = "5",
                Font = new Font("Consolas", 72, FontStyle.Bold),
                ForeColor = Color.Gold,
                BackColor = Color.Transparent,
                AutoSize = true,
                Visible = false
            };

            panelBackground.Controls.Add(lblLaunchCountdown);
            lblLaunchCountdown.BringToFront();
            lblLaunchCountdown.Location = new Point(
                (panelBackground.Width - lblLaunchCountdown.Width) / 2,
                (panelBackground.Height / 2) - 100
            );

            // --- Load permanent upgrades ---
            var (dbSpeed, dbArmor, dbWeapon) = DatabaseHelper.GetPlayerUpgrades(currentUser);

            // --- Load temporary upgrades (from UpgradeControl screen) ---
            var (tempSpeed, tempArmor, tempWeapon) = DatabaseHelper.GetTempUpgrades(currentUser);

            // --- Prioritize TEMPORARY upgrades (even if they're base values) ---
            bool hasTempData = !(tempSpeed == 0 && tempArmor == 0 && tempWeapon == 0);

            if (hasTempData)
            {
                // Use whatever was last saved from the upgrade screen
                rocketSpeed = tempSpeed;
                maxHP = tempArmor > 0 ? tempArmor : dbArmor;  // if 0 accidentally saved, fallback
                rocketWeapon = tempWeapon;
            }
            else
            {
                // Fallback to permanent upgrades
                rocketSpeed = dbSpeed;
                maxHP = dbArmor;
                rocketWeapon = dbWeapon;
            }




            // ---- Hide weapon bar and logo if no weapon upgrade ----
            if (rocketWeapon <= 0)
            {
                pbWeapon.Visible = false;
                pbWeapon_Logo.Visible = false;
            }
            else
            {
                pbWeapon.Visible = true;
                pbWeapon_Logo.Visible = true;
            }

            // ---- Load coins and other state ----
            score = DatabaseHelper.LoadPlayerScore(currentUser);
            coins = DatabaseHelper.GetPlayerCoins(currentUser);
            hp = DatabaseHelper.LoadPlayerHP(currentUser);
            fuel = DatabaseHelper.LoadPlayerFuel(currentUser);

            //hp = Math.Min(DatabaseHelper.LoadPlayerHP(currentUser), maxHP);  // clamp to armor-based maxHP
            pbHP.MaxValue = maxHP;
            pbHP.Value = hp;
            UpdateHPBarColor();

            // ---- HP Scaling with Armor ----
            ApplyArmorUpgradeEffect();

            pbFuel.MaxValue = 100;
            pbFuel.Value = fuel;

            pbWeapon.MaxValue = 100;
            pbWeapon.Value = 0;
            coins += 5000; // TEMP for testing
            lblScore.Content = $"Score: {score}";
            lblCoins.Content = $"{FormatCoins(coins)}";

            // ---- Movement speed based on upgrade ----
            moveStep += rocketSpeed;

            // ---- Laser duration based on permanent or temp weapon ----
            laserDuration = rocketWeapon * 5; // 5s per weapon level

            // ---- Load player position & stage ----
            var (posX, posY, stageIdx, stageOffset) = DatabaseHelper.LoadPlayerState(currentUser);
            PlayerRocket.Location = new Point(posX, posY);
            currentStageIndex = stageIdx;
            currentStageOffset = stageOffset;

            // ---- Initialize rocket sprites ----
            idleGif = Properties.Resources.rocket_2_smol;
            upGif = Properties.Resources.rocket_2_smol_up;
            downGif = Properties.Resources.rocket_2_smol_down;
            landGif = Properties.Resources.rocket_2_land_smol;

            currentRocketSprite = idleGif;
            ImageAnimator.Animate(idleGif, OnFrameChanged);
            ImageAnimator.Animate(upGif, OnFrameChanged);
            ImageAnimator.Animate(downGif, OnFrameChanged);
            ImageAnimator.Animate(landGif, OnFrameChanged);

            PlayerRocket.Image = null;
            PlayerRocket.Paint += PlayerRocket_Paint;
            PlayerRocket.BackColor = Color.Transparent;

            // ---- Load sound state ----

            var (savedBgTime, savedLaserActive, savedLaserTimeLeft) = DatabaseHelper.LoadSoundState(currentUser);

            bgMusicTime = savedBgTime;
            laserTimeLeft = savedLaserTimeLeft;

            StartBackgroundMusic();

            // === Restore Laser Charge UI ===
            if (savedLaserTimeLeft > 0 && savedLaserTimeLeft <= rocketWeapon * 4)
            {
                // Continue from saved charge
                laserTimeLeft = savedLaserTimeLeft;
                double progress = savedLaserTimeLeft / (rocketWeapon * 4.0);
                pbWeapon.Value = (int)(pbWeapon.MaxValue * progress);
            }
            else if (savedLaserTimeLeft == 0 && DatabaseHelper.HasSoundState(currentUser))
            {
                // Saved record exists but laser was empty — keep it empty
                laserTimeLeft = 0;
                pbWeapon.Value = 0;
            }
            else
            {
                // No record found yet — default to full
                laserTimeLeft = rocketWeapon * 4;
                pbWeapon.Value = pbWeapon.MaxValue;
            }

            // === Set weapon charge flag based on saved state ===
            weaponCharged = (laserTimeLeft > 0);

            // Resume from saved time
            if (bgReader != null && savedBgTime > 0)
                bgReader.CurrentTime = TimeSpan.FromSeconds(savedBgTime);

            // If laser was active before leaving, resume it
            if (savedLaserActive && savedLaserTimeLeft > 0)
            {
                ActivateLaser();
                laserTimeLeft = savedLaserTimeLeft;
            }

            // ---- Background setup ----
            backgroundImage = panelBackground.BackgroundImage;
            panelBackground.Paint += panelBackground_Paint;
            panelBackground.BackgroundImage = null;
            SetDoubleBuffered(panelBackground);

            // ---- Rocket sprite setup ----
            idleGif = Properties.Resources.rocket_2_smol;
            upGif = Properties.Resources.rocket_2_smol_up;
            downGif = Properties.Resources.rocket_2_smol_down;
            landGif = Properties.Resources.rocket_2_land_smol;
            currentRocketSprite = idleGif;

            ImageAnimator.Animate(idleGif, OnFrameChanged);
            ImageAnimator.Animate(upGif, OnFrameChanged);
            ImageAnimator.Animate(downGif, OnFrameChanged);
            ImageAnimator.Animate(landGif, OnFrameChanged);

            PlayerRocket.Image = null;
            PlayerRocket.Paint += PlayerRocket_Paint;
            PlayerRocket.BackColor = Color.Transparent;

            // ---- Timers ----
            scrollTimer = new Timer { Interval = 16 };
            scrollTimer.Tick += ScrollTimer_Tick;
            scrollTimer.Start();

            moveTimer = new Timer { Interval = 16 };
            moveTimer.Tick += MoveTimer_Tick;
            moveTimer.Start();

            fuelTimer = new Timer { Interval = fuelDrainInterval };
            fuelTimer.Tick += FuelTimer_Tick;

            // --- Tier 1 Obstacle Spawner (spawn 3..6 obstacles each event) ---
            obstacleSpawnTimer = new Timer();
            obstacleSpawnTimer.Interval = obstacleRand.Next(3000, 6001); // random 3–6s
            obstacleSpawnTimer.Tick += (s, e) =>
            {
                // spawn a small group of obstacles at once (3..6)
                int batchCount = obstacleRand.Next(3, 6); // 3..6 inclusive
                for (int i = 0; i < batchCount; i++)
                {
                    // small delay between creation attempts helps spread them vertically,
                    // spacing attempt prevents close stacking.
                    SpawnRandomObstacle();
                }

                // Re-randomize the next spawn delay
                obstacleSpawnTimer.Interval = obstacleRand.Next(4000, 8001);
            };

            // === Power-up Spawner ===
            powerupSpawnTimer = new Timer { Interval = 5000 }; // spawn every 5s on average
            powerupSpawnTimer.Tick += (s, e) =>
            {
                if (controlsLocked || isGameOver || isPaused) return;

                SpawnPowerup();
                powerupSpawnTimer.Interval = powerupRand.Next(5000, 9000); // random 5–9s delay
            };
            powerupSpawnTimer.Start();

            // --- Launch countdown logic with re-entry pause system ---

            // Load launch countdown from DB
            launchCountdown = DatabaseHelper.LoadLaunchTimer(currentUser);
            bool hasProgress = fuel < 100 || hp < maxHP || currentStageIndex > 0 || currentStageOffset > 0;

            if (hasProgress && launchCountdown == 0)
            {
                // --- Player already finished the launch ---
                // Check if player left after finishing the launch and is returning later
                if (DatabaseHelper.LoadLaunchTimer(currentUser) == 0)
                {
                    // Show a 5-second re-entry countdown (game fully paused)
                    int reentryCountdown = 5;
                    lblLaunchCountdown.Text = reentryCountdown.ToString();
                    lblLaunchCountdown.Visible = true;
                    currentRocketSprite = landGif;

                    SetGamePaused(true);

                    Timer reentryTimer = new Timer { Interval = 1000 };
                    reentryTimer.Tick += (s, ev) =>
                    {
                        reentryCountdown--;

                        if (reentryCountdown > 0)
                        {
                            lblLaunchCountdown.Text = reentryCountdown.ToString();
                            lblLaunchCountdown.Left = (panelBackground.Width - lblLaunchCountdown.Width) / 2;
                            // Disable rocket tilt/rotation during countdown
                            currentRotation = 0f;
                            PlayerRocket.Invalidate();
                        }
                        else
                        {
                            reentryTimer.Stop();
                            lblLaunchCountdown.Visible = false;
                            currentRocketSprite = idleGif;
                            PlayerRocket.Invalidate();

                            SetGamePaused(false);

                        }
                    };
                    reentryTimer.Start();
                }

                controlsLocked = false;
            }
            else
            {
                if (launchCountdown == 0)
                    launchCountdown = 11;

                controlsLocked = true;

                // --- Prevent saving while countdown is active ---
                SessionManager.IsLaunchCountdownActive = true;

                // --- Launch countdown (10s total, first 5s visible) ---
                visibleCountdown = 5;
                lblLaunchCountdown.Text = visibleCountdown.ToString();
                lblLaunchCountdown.Visible = true;
                currentRocketSprite = landGif;

                scrollTimer?.Stop();

                visibleCountdownTimer = new Timer { Interval = 1000 };
                visibleCountdownTimer.Tick += (s, ev) =>
                {
                    visibleCountdown--;
                    if (visibleCountdown > 0)
                    {
                        lblLaunchCountdown.Text = visibleCountdown.ToString();
                        lblLaunchCountdown.Left = (panelBackground.Width - lblLaunchCountdown.Width) / 2;
                    }
                    else
                    {
                        visibleCountdownTimer.Stop();
                        lblLaunchCountdown.Visible = false;
                        currentRocketSprite = idleGif;
                        PlayerRocket.Invalidate();
                        scrollTimer?.Start();
                    }
                };
                visibleCountdownTimer.Start();

                // --- Full 10-second countdown ---
                launchDelayTimer = new Timer { Interval = 1000 };
                launchDelayTimer.Tick += (s, ev) =>
                {
                    launchCountdown--;
                    DatabaseHelper.SaveLaunchTimer(currentUser, launchCountdown);

                    if (launchCountdown <= 0)
                    {
                        launchDelayTimer.Stop();
                        controlsLocked = false;
                        SessionManager.IsLaunchCountdownActive = false;
                        SetGamePaused(false);
                    }
                };
                launchDelayTimer.Start();
            }


            // ---- Input setup ----
            this.KeyDown += GameControl_KeyDown;
            this.KeyUp += GameControl_KeyUp;
            this.PreviewKeyDown += GameControl_PreviewKeyDown;

            this.SetStyle(ControlStyles.Selectable, true);
            this.TabStop = true;
            this.Focus();
        }

        private string FormatCoins(int coins)
        {
            if (coins >= 1_000_000) return $"{coins / 1_000_000.0:F1}M";
            if (coins >= 1_000) return $"{coins / 1_000.0:F1}K";
            return coins.ToString();
        }

        // ----------------------------------------
        //   Dynamic HP Bar System
        // ----------------------------------------
        private void UpdateHPBarColor()
        {
            double ratio = (double)pbHP.Value / pbHP.MaxValue;
            if (ratio > 0.75)
                pbHP.Foreground = Color.LimeGreen;
            else if (ratio > 0.5)
                pbHP.Foreground = Color.YellowGreen;
            else if (ratio > 0.25)
                pbHP.Foreground = Color.Orange;
            else
                pbHP.Foreground = Color.Red;
        }

        // Call this whenever armor upgrade changes (optional if HP can increase mid-game)
        private void UpdateHPBarMax(int newMaxHP)
        {
            double fillRatio = (double)pbHP.Value / pbHP.MaxValue;
            pbHP.MaxValue = newMaxHP;
            pbHP.Value = Math.Min((int)(fillRatio * pbHP.MaxValue), pbHP.MaxValue);
            UpdateHPBarColor();
        }

        private void ApplyArmorUpgradeEffect()
        {
            // Each armor level beyond base (100 HP) adds +25 max HP
            int armorLevel = (maxHP - 100) / 25;
            int newMaxHP = 100 + (armorLevel * 25);

            // Preserve HP ratio relative to old max HP
            double hpRatio = pbHP.MaxValue > 0 ? (double)hp / pbHP.MaxValue : 1.0;
            hp = (int)(hpRatio * newMaxHP);

            // Clamp to ensure HP doesn’t exceed new max
            hp = Math.Min(hp, newMaxHP);

            pbHP.MaxValue = newMaxHP;
            pbHP.Value = hp;
            UpdateHPBarColor();

            // Save updated HP and maxHP to DB
            DatabaseHelper.SavePlayerHP(currentUser, hp);
            DatabaseHelper.SaveTempUpgrades(currentUser, rocketSpeed, newMaxHP, rocketWeapon);
        }


        // ----------------------------------------
        //   BACKGROUND SCROLLING (MULTI-STAGE)
        // ----------------------------------------

        private List<(Image img, int height)> backgroundStages;
        private int currentStageIndex = 0;
        private int currentStageOffset = 0;
        private int totalScrollDistance = 0;

        private void InitializeBackgrounds()
        {
            backgroundStages = new List<(Image, int)>
            {
                (Properties.Resources.HomeBase, 1000),               // 1x Homebase
                (Properties.Resources.Atmosphere_long, 3280),
                /*(Properties.Resources.Atmosphere, 820),            // 4x Atmosphere (uncomment if you want loopung background instead of stretched)
                (Properties.Resources.Atmosphere, 820),
                (Properties.Resources.Atmosphere, 820),
                (Properties.Resources.Atmosphere, 820),*/
                (Properties.Resources.Orbit, 800),                   // 1x Orbit
                (Properties.Resources.OuterSpace_long, 6900),
                /*(Properties.Resources.OuterSpace, 1150),           // 6x OuterSpace (uncomment if you want loopung background instead of stretched)
                (Properties.Resources.OuterSpace, 1150),
                (Properties.Resources.OuterSpace, 1150),
                (Properties.Resources.OuterSpace, 1150),
                (Properties.Resources.OuterSpace, 1150),
                (Properties.Resources.OuterSpace, 1150),*/
                (Properties.Resources.MoonOrbit, 820)                // 1x MoonOrbit
            };

            totalScrollDistance = backgroundStages.Sum(b => b.height);
            backgroundImage = backgroundStages[0].img;
        }

        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            if (isPaused) return;

            if (backgroundStages == null || backgroundStages.Count == 0)
                return;

            currentStageOffset += scrollSpeed;
            if (!bgMusicPlaying)
                StartBackgroundMusic();

            var (currentImg, _) = backgroundStages[currentStageIndex];
            float scale = (float)panelBackground.Width / currentImg.Width;
            int scaledHeight = (int)(currentImg.Height * scale);

            if (currentStageOffset >= scaledHeight)
            {
                currentStageOffset = 0;
                currentStageIndex++;

                if (currentStageIndex >= backgroundStages.Count)
                {
                    currentStageIndex = backgroundStages.Count - 1;
                }
            }

            if (currentStageIndex == backgroundStages.Count - 1)
            {
                float lastScale = (float)panelBackground.Width / backgroundStages[currentStageIndex].img.Width;
                int lastScaledHeight = (int)(backgroundStages[currentStageIndex].img.Height * lastScale);

                if (currentStageOffset >= lastScaledHeight - panelBackground.Height)
                {
                    currentStageOffset = lastScaledHeight - panelBackground.Height;
                    scrollTimer.Stop();
                    StopBackgroundMusic(); // <--- stop when scrolling reaches top
                }
            }

            panelBackground.Invalidate();
        }

        private void panelBackground_Paint(object sender, PaintEventArgs e)
        {
            if (backgroundStages == null || backgroundStages.Count == 0)
                return;

            Graphics g = e.Graphics;
            g.Clear(Color.Black);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            int currentIndex = Math.Min(currentStageIndex, backgroundStages.Count - 1);
            var (currentImg, _) = backgroundStages[currentIndex];

            float scaleCurrent = (float)panelBackground.Width / currentImg.Width;
            int scaledHeightCurrent = (int)(currentImg.Height * scaleCurrent);

            int currentY = panelBackground.Height - scaledHeightCurrent + currentStageOffset;
            g.DrawImage(currentImg, new Rectangle(0, currentY, panelBackground.Width, scaledHeightCurrent));

            if (currentIndex + 1 < backgroundStages.Count)
            {
                var (nextImg, _) = backgroundStages[currentIndex + 1];
                float scaleNext = (float)panelBackground.Width / nextImg.Width;
                int scaledHeightNext = (int)(nextImg.Height * scaleNext);
                int nextY = currentY - scaledHeightNext + 5;
                g.DrawImage(nextImg, new Rectangle(0, nextY, panelBackground.Width, scaledHeightNext));
            }
        }

        private void SetDoubleBuffered(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        // ----------------------------------------
        //   PLAYER INPUT HANDLING
        // ----------------------------------------
        private void GameControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (controlsLocked) return; // Ignore input during launch delay
            if (isOutOfFuel) return;    // Prevent changing sprite when out of fuel

            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
            {
                upPressed = true;
                currentRocketSprite = upGif;
            }

            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
            {
                downPressed = true;
                currentRocketSprite = downGif;
            }

            if (fuel > 0)
            {
                if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
                    leftPressed = true;

                if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
                    rightPressed = true;

                // Disable rotation while laser is active
                if (!laserActive)
                {
                    if (leftPressed && rightPressed)
                        currentRotation = 0f;
                    else if (leftPressed)
                        currentRotation = -15f;
                    else if (rightPressed)
                        currentRotation = 15f;
                }
                else
                {
                    currentRotation = 0f; // keep rocket steady
                }
            }

            PlayerRocket.Invalidate();

            // Simulate collecting a weapon core
            if (e.KeyCode == Keys.B && !weaponCharged && rocketWeapon > 0)
            {
                ChargeWeapon();
            }

            // Activate laser if charged
            if (e.KeyCode == Keys.L && weaponCharged && !laserActive && rocketWeapon > 0)
            {
                ActivateLaser();
            }
        }

        private void GameControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (controlsLocked) return;
            if (isOutOfFuel) return;

            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
                upPressed = false;

            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
                downPressed = false;

            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
                leftPressed = false;

            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
                rightPressed = false;

            if (upPressed)
                currentRocketSprite = upGif;
            else if (downPressed)
                currentRocketSprite = downGif;
            else
                currentRocketSprite = idleGif;

            if (fuel > 0)
            {
                // Keep rocket upright while laser is active
                if (!laserActive)
                {
                    if (leftPressed && rightPressed)
                        currentRotation = 0f;
                    else if (leftPressed)
                        currentRotation = -15f;
                    else if (rightPressed)
                        currentRotation = 15f;
                    else
                        currentRotation = 0f;
                }
                else
                {
                    currentRotation = 0f; // stay straight
                }
            }

            PlayerRocket.Invalidate();
        }

        private void GameControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Ignore input during non-playable states
            if (isPaused || controlsLocked || isOutOfFuel || fuel <= 0)
                return;

            // Treat arrow keys as input keys
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    e.IsInputKey = true;
                    break;
            }
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            PlayerRocket.Invalidate();      // redraw each time a frame changes
        }

        private void PlayerRocket_Paint(object sender, PaintEventArgs e)
        {
            if (currentRocketSprite == null) return;

            // update current frame of all gifs
            ImageAnimator.UpdateFrames();

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // Move origin to center of control
            g.TranslateTransform(PlayerRocket.Width / 2f, PlayerRocket.Height / 2f);

            // Apply rotation (keeps the last rotation even after out of fuel)
            g.RotateTransform(currentRotation);

            // Draw sprite centered
            g.DrawImage(
                currentRocketSprite,
                -currentRocketSprite.Width / 2f,
                -currentRocketSprite.Height / 2f,
                currentRocketSprite.Width,
                currentRocketSprite.Height);

            // Reset transform
            g.ResetTransform();
        }

        // ----------------------------------------
        //   MOVEMENT TIMER (continuous movement)
        // ----------------------------------------

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            if (isPaused) return;

            int newX = PlayerRocket.Location.X;
            int newY = PlayerRocket.Location.Y;

            // Vertical movement
            if (upPressed && !downPressed) newY -= moveStep;
            else if (downPressed && !upPressed) newY += moveStep;

            // Horizontal movement
            if (leftPressed && !rightPressed) newX -= moveStep;
            else if (rightPressed && !leftPressed) newX += moveStep;

            // Clamp to boundaries
            newX = Math.Max(minX, Math.Min(maxX, newX));
            newY = Math.Max(minY, Math.Min(maxY, newY));

            PlayerRocket.Location = new Point(newX, newY);
        }

        // ----------------------------------------
        //   Fuel System
        // ----------------------------------------

        private void FuelTimer_Tick(object sender, EventArgs e)
        {
            if (isPaused) return;

            if (isOutOfFuel) return;

            if (fuel > 0)
            {
                fuel -= fuelDrainRate;
                if (fuel < 0) fuel = 0;

                pbFuel.Value = fuel;

                if (fuel == 0)
                {
                    isOutOfFuel = true;
                    currentRocketSprite = landGif;
                    PlayerRocket.Invalidate();
                    moveTimer.Stop();
                    StartGameOverCountdown();
                }
            }
        }

        // ----------------------------------------
        //   Game Over Sequence (Updated for Power-Ups)
        // ----------------------------------------

        private void StartGameOverCountdown()
        {

            if (isGameOver) return;

            bool hpDepleted = (hp <= 0);
            bool fuelDepleted = (fuel <= 0);

            if (!hpDepleted && !fuelDepleted) return;

            isGameOver = true;

            // =========================
            // 💥 HP = 0 (Instant Game Over)
            // =========================
            if (hpDepleted)
            {
                controlsLocked = true; // freeze all controls

                PictureBox explosion = null;

                try
                {
                    PlayerRocket.Visible = false;

                    // Create explosion sprite (200x200)
                    explosion = new PictureBox
                    {
                        Image = Properties.Resources.Explosion,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Size = new Size(200, 200),
                        BackColor = Color.Transparent,
                        Location = new Point(
                            PlayerRocket.Left + PlayerRocket.Width / 2 - 100,
                            PlayerRocket.Top + PlayerRocket.Height / 2 - 100
                        )
                    };

                    panelBackground.Controls.Add(explosion);
                    explosion.BringToFront();
                    explosion.Refresh();

                    // --- Play explosion sound ---
                    string explosionSoundPath = Path.Combine(
                        Application.StartupPath, "Images&Animations", "sfx", "Explosion_Sound.wav"
                    );
                    if (File.Exists(explosionSoundPath))
                        new System.Media.SoundPlayer(explosionSoundPath).Play();

                    // Stop GIF loop after ~2 seconds
                    Timer stopGifTimer = new Timer { Interval = 2000 };
                    stopGifTimer.Tick += (s2, e2) =>
                    {
                        stopGifTimer.Stop();
                        stopGifTimer.Dispose();
                        if (explosion != null) explosion.Image = null;
                    };
                    stopGifTimer.Start();
                }
                catch { }

                // --- Short delay for explosion visual (2.5 sec) ---
                Timer explosionEndTimer = new Timer { Interval = 2500 };
                explosionEndTimer.Tick += (s, e) =>
                {
                    explosionEndTimer.Stop();
                    explosionEndTimer.Dispose();

                    // Clean up explosion
                    if (explosion != null)
                    {
                        panelBackground.Controls.Remove(explosion);
                        explosion.Dispose();
                    }

                    // Stop everything
                    StopAllSounds();
                    scrollTimer?.Stop();
                    fuelTimer?.Stop();
                    obstacleSpawnTimer?.Stop();
                    powerupSpawnTimer?.Stop();
                    laserTimer?.Stop();

                    // Reset sound state
                    bgMusicTime = 0;
                    DatabaseHelper.SaveSoundState(currentUser, 0, false, 0);

                    // Save final progress
                    DatabaseHelper.UpdateHighScore(currentUser, score);
                    DatabaseHelper.UpdatePlayerCoins(currentUser, coins);
                    DatabaseHelper.SavePlayerFuel(currentUser, fuel);
                    DatabaseHelper.SavePlayerHP(currentUser, hp);
                    DatabaseHelper.SavePlayerState(currentUser,
                        PlayerRocket.Location.X, PlayerRocket.Location.Y, currentStageIndex, currentStageOffset);
                    DatabaseHelper.SaveLaunchTimer(currentUser, launchCountdown);
                    DatabaseHelper.SaveTempUpgrades(currentUser, rocketSpeed, maxHP, rocketWeapon);

                    string summaryMessage =
                        $"💥 GAME OVER 💥\n\n" +
                        $"• Score: {score}\n" +
                        $"• Coins Collected This Run: {sessionCoins}\n" +
                        $"• Distance Reached: Stage {currentStageIndex + 1}\n\n" +
                        $"Your progress has been saved.";

                    MessageBox.Show(summaryMessage, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset player
                    DatabaseHelper.ResetPlayerProgress(currentUser);


                    // Return to Main Menu
                    MainMenuControl menu = new MainMenuControl();
                    GameForm parentForm = (GameForm)this.FindForm();
                    parentForm.Controls.Clear();
                    parentForm.Controls.Add(menu);
                };
                explosionEndTimer.Start();
                return; // stop here, explosion handles its own flow
            }

            // =========================
            // ⛽ FUEL = 0 (5-sec grace)
            // =========================
            if (fuelDepleted)
            {
                rocketControlDisabled = true;



                int timeLeft = 5;
                Timer fuelEndTimer = new Timer { Interval = 1000 };
                fuelEndTimer.Tick += (s, ev) =>
                {
                    timeLeft--;

                    // Cancel Game Over if refueled
                    if (fuel > 0)
                    {
                        fuelEndTimer.Stop();
                        fuelEndTimer.Dispose();

                        // Restore all state flags
                        isGameOver = false;
                        rocketControlDisabled = false;
                        controlsLocked = false;
                        isOutOfFuel = false; // <-- THIS fixes the main problem

                        // Restart movement
                        if (!moveTimer.Enabled)
                            moveTimer.Start();

                        return;
                    }

                    if (timeLeft <= 0)
                    {
                        fuelEndTimer.Stop();
                        fuelEndTimer.Dispose();

                        // --- Proceed to Game Over ---
                        StopAllSounds();
                        scrollTimer?.Stop();
                        obstacleSpawnTimer?.Stop();
                        powerupSpawnTimer?.Stop();
                        laserTimer?.Stop();

                        bgMusicTime = 0;
                        DatabaseHelper.SaveSoundState(currentUser, 0, false, 0);

                        DatabaseHelper.UpdateHighScore(currentUser, score);
                        DatabaseHelper.UpdatePlayerCoins(currentUser, coins);
                        DatabaseHelper.SavePlayerFuel(currentUser, fuel);
                        DatabaseHelper.SavePlayerHP(currentUser, hp);
                        DatabaseHelper.SavePlayerState(currentUser,
                            PlayerRocket.Location.X, PlayerRocket.Location.Y, currentStageIndex, currentStageOffset);
                        DatabaseHelper.SaveLaunchTimer(currentUser, launchCountdown);
                        DatabaseHelper.SaveTempUpgrades(currentUser, rocketSpeed, maxHP, rocketWeapon);

                        string summaryMessage =
                            $"💥 GAME OVER 💥\n\n" +
                            $"• Score: {score}\n" +
                            $"• Coins Collected This Run: {sessionCoins}\n" +
                            $"• Distance Reached: Stage {currentStageIndex + 1}\n\n" +
                            $"Your progress has been saved.";

                        MessageBox.Show(summaryMessage, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Reset player state
                        DatabaseHelper.ResetPlayerProgress(currentUser);

                        MainMenuControl menu = new MainMenuControl();
                        GameForm parentForm = (GameForm)this.FindForm();
                        parentForm.Controls.Clear();
                        parentForm.Controls.Add(menu);
                    }
                };
                fuelEndTimer.Start();
            }
        }

        // ----------------------------------------
        //   Launch Timer
        // ----------------------------------------

        private void LaunchDelayTimer_Tick(object sender, EventArgs e)
        {
            launchCountdown--;

            if (launchCountdown <= 0)
            {
                controlsLocked = false;
                launchDelayTimer.Stop();
                DatabaseHelper.SaveLaunchTimer(currentUser, 0);
            }
            else
            {
                DatabaseHelper.SaveLaunchTimer(currentUser, launchCountdown);
            }
        }

        // ----------------------------------------
        //   Save Player Progress
        // ----------------------------------------
        private void SavePlayerProgress()
        {
            // Skip saving if countdown still active
            if (SessionManager.IsLaunchCountdownActive)
                return;

            // Save runtime data
            DatabaseHelper.SavePlayerFuel(currentUser, fuel);
            DatabaseHelper.SavePlayerHP(currentUser, hp);
            DatabaseHelper.SavePlayerState(currentUser, PlayerRocket.Location.X, PlayerRocket.Location.Y, currentStageIndex, currentStageOffset);
            DatabaseHelper.SaveLaunchTimer(currentUser, launchCountdown);

            // Save current temp upgrades for next play session
            DatabaseHelper.SaveTempUpgrades(currentUser, rocketSpeed, maxHP, rocketWeapon);

            // Coins and scores are permanent
            DatabaseHelper.UpdatePlayerScore(currentUser, score);
            DatabaseHelper.UpdateHighScore(currentUser, score);
            DatabaseHelper.UpdatePlayerCoins(currentUser, coins);
        }

        // ----------------------------------------
        //  Automatic Focus Recovery
        // ----------------------------------------

        private void EnableAutomaticFocusRecovery()
        {
            // Attach to any existing children
            AttachFocusHandlersRecursively(this);

            // Re-attach when controls are added later at runtime
            this.ControlAdded += (s, e) => AttachFocusHandlersRecursively(e.Control);
        }

        private void AttachFocusHandlersRecursively(Control root)
        {
            // If root is a container, attach for each child (including the root itself optionally)
            AttachHandlers(root);

            foreach (Control child in root.Controls)
            {
                AttachFocusHandlersRecursively(child);
            }
        }

        private void AttachHandlers(Control ctrl)
        {
            // Avoid attaching multiple times
            // (store a flag in Tag or use an extension; simplest: remove handlers first to avoid duplicates)
            ctrl.MouseDown -= ChildControl_RequestGameFocus;
            ctrl.MouseDown += ChildControl_RequestGameFocus;

            ctrl.Click -= ChildControl_RequestGameFocus;
            ctrl.Click += ChildControl_RequestGameFocus;

            // If control can receive focus and gets focus via keyboard, redirect it
            ctrl.GotFocus -= ChildControl_RequestGameFocus;
            ctrl.GotFocus += ChildControl_RequestGameFocus;
        }

        private void ChildControl_RequestGameFocus(object sender, EventArgs e)
        {
            if (!focusRecoveryEnabled) return;
            try
            {
                var f = this.FindForm();
                if (f != null)
                {
                    // Skip focus recovery if the form isn't active (e.g., MessageBox or modal dialog open)
                    if (!f.Focused && !f.ContainsFocus)
                        return;
                }

                // Safe to take focus back now
                this.Select();
                this.Focus();

                if (f != null)
                {
                    try
                    {
                        f.ActiveControl = this;
                    }
                    catch
                    {
                        // ignore invalid focus set attempts
                    }
                }
            }
            catch
            {
                // ignore exceptions
            }
        }


        // ----------------------------------------
        //   Pause Menu Handling
        // ----------------------------------------

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (isPaused) return;

            // Prevent pausing during the launch countdown
            if (launchCountdown > 0 && SessionManager.IsLaunchCountdownActive)
                return;

            // Stop game timers
            SetGamePaused(true);

            focusRecoveryEnabled = false; // when opening a modal or overlay

            // pause audio
            bgOutput?.Pause();
            laserOutput?.Pause();

            // Create PauseOverlayControl with size 700x700
            pauseOverlay = new PauseOverlayControl
            {
                Size = new Size(400, 400),
                BackColor = Color.Transparent
            };

            // add it over panelBackground so it inherits that as its parent
            panelBackground.Controls.Add(pauseOverlay);

            pauseOverlay.Location = new Point(
                (panelBackground.Width - pauseOverlay.Width) / 2,
                (panelBackground.Height - pauseOverlay.Height) / 2
            );

            pauseOverlay.BringToFront();

            pauseOverlay.BackColor = Color.FromArgb(200, 0, 0, 0);


            // Hook up events
            pauseOverlay.ResumeClicked += OnResumeClicked;
            pauseOverlay.RestartClicked += OnRestartClicked;
            pauseOverlay.MainMenuClicked += OnMainMenuClicked;
        }

        private void OnResumeClicked()
        {
            // remove pause overlay
            panelBackground.Controls.Remove(pauseOverlay);
            pauseOverlay.Dispose();
            pauseOverlay = null;

            SetGamePaused(false);

            focusRecoveryEnabled = true;

            // Resume countdown only if it's still active
            if (launchCountdown > 0)
            {
                launchDelayTimer?.Start();
                controlsLocked = true;
                // Disable rocket tilt/rotation during countdown
                currentRotation = 0f;
                PlayerRocket.Invalidate();
            }
            else
            {
                controlsLocked = false;
            }

            isPaused = false;
        }

        private void OnRestartClicked()
        {
            // remove overlay
            this.Controls.Remove(pauseOverlay);
            pauseOverlay.Dispose();
            pauseOverlay = null;

            // stop both bg and laser
            StopAllSounds();

            // Reset music timeline to 0 before starting a new game
            bgMusicTime = 0;
            DatabaseHelper.SaveSoundState(
                currentUser,
                bgMusicTime: 0,
                laserActive: false,
                laserTimeLeft: 0
            );

            // reset player progress in DB before starting a new game
            DatabaseHelper.ResetPlayerProgress(currentUser);

            // start new game (fresh music and all)
            GameControl newGame = new GameControl();
            GameForm parentForm = (GameForm)this.FindForm();
            parentForm.Controls.Clear();
            parentForm.Controls.Add(newGame);

            // 🔹 Force focus on the new game control after it's loaded
            parentForm.ActiveControl = newGame;
            newGame.Select();
            newGame.Focus();
        }

        private void OnMainMenuClicked()
        {
            // If countdown is still active (not yet finished)
            if (launchCountdown > 0 && launchCountdown < 10)
            {
                // reset player progress in DB before starting a new game
                DatabaseHelper.ResetPlayerProgress(currentUser);

                // stop both bg and laser
                StopAllSounds();
            }
            else if (launchDelayTimer != null && launchDelayTimer.Enabled)
            {
                // Otherwise, just save the remaining seconds normally
                launchDelayTimer.Stop();
                DatabaseHelper.SaveLaunchTimer(currentUser, launchCountdown);
            }

            // Save sound state before leaving
            if (bgReader != null)
                bgMusicTime = bgReader.CurrentTime.TotalSeconds;

            DatabaseHelper.SaveSoundState(
                currentUser,
                bgMusicTime,
                laserActive,
                laserTimeLeft
            );

            StopAllSounds();         // stop both bg and laser
            SavePlayerProgress();    // save HP, fuel, etc.

            // Back to menu
            MainMenuControl menu = new MainMenuControl();
            GameForm parentForm = (GameForm)this.FindForm();
            parentForm.Controls.Clear();
            parentForm.Controls.Add(menu);
        }

        private void SetGamePaused(bool pause)
        {
            isPaused = pause;
            controlsLocked = pause;

            if (pause)
            {
                // --- Stop all timers ---
                moveTimer?.Stop();
                scrollTimer?.Stop();
                fuelTimer?.Stop();
                launchDelayTimer?.Stop();
                laserTimer?.Stop();
                laserFollowTimer?.Stop();
                laserSyncTimer?.Stop();

                // --- Pause audio ---
                bgOutput?.Pause();
                laserOutput?.Pause();

                // --- Immediately save sound state when pausing ---
                try
                {
                    double bgMusicTime = bgReader?.CurrentTime.TotalSeconds ?? 0;
                    DatabaseHelper.SaveSoundState(
                        currentUser,
                        bgMusicTime,
                        laserActive,
                        laserTimeLeft
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to save sound state on pause: " + ex.Message);
                    Application.Exit();
                }
            }
            else
            {
                // --- Resume focus ---
                this.Select();
                this.Focus();
                this.TabStop = true;

                upPressed = false;
                downPressed = false;
                leftPressed = false;
                rightPressed = false;

                // --- Resume timers ---
                moveTimer?.Start();
                scrollTimer?.Start();
                fuelTimer?.Start();

                if (launchCountdown > 0)
                    launchDelayTimer?.Start();

                if (laserActive)
                    laserTimer?.Start();
                laserFollowTimer?.Start();
                laserSyncTimer?.Start();

                // --- Resume audio ---
                bgOutput?.Play();
                if (laserActive)
                    laserOutput?.Play();

                // --- Resume spawning ---
                obstacleSpawnTimer.Start();

                // --- Restore laserTimeLeft from DB if available ---
                try
                {
                    var soundState = DatabaseHelper.LoadSoundState(currentUser);
                    if (soundState.laserActive && soundState.laserTimeLeft > 0)
                    {
                        laserTimeLeft = soundState.laserTimeLeft;
                        // Optionally: re-sync laser duration here if needed
                    }

                    if (bgReader != null && soundState.bgMusicTime > 0)
                        bgReader.CurrentTime = TimeSpan.FromSeconds(soundState.bgMusicTime);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to load sound state on resume: " + ex.Message);
                    Application.Exit();
                }
            }
        }

        // ----------------------------------------
        //   WEAPON SYSTEM
        // ----------------------------------------

        private void ChargeWeapon()
        {
            // Prevent charging if the weapon bar is not empty
            if (pbWeapon.Value > 0)
            {
                return;
            }

            // Only allow charging when empty
            weaponCharged = true;
            pbWeapon.Value = pbWeapon.MaxValue;

            // Compute the max laser time based on the current weapon level
            laserDuration = rocketWeapon * 4;
            laserTimeLeft = laserDuration;

            // Immediately save this full charge to the DB
            DatabaseHelper.SaveSoundState(
                currentUser,
                bgMusicTime: bgReader?.CurrentTime.TotalSeconds ?? 0,
                laserActive: false,
                laserTimeLeft: laserTimeLeft
            );

            MessageBox.Show("Weapon charged!");
        }

        private void ActivateLaser()
        {
            if (rocketWeapon <= 0) return; // Weapon not unlocked

            // --- Initialize laser state ---
            laserDuration = rocketWeapon * 4; // seconds total

            if (laserTimeLeft <= 0 || laserTimeLeft > laserDuration)
            {
                // Only reset if there's truly no saved value
                laserTimeLeft = laserDuration;
            }
            else
            {
                // Continue from saved charge
                pbWeapon.Value = (int)(pbWeapon.MaxValue * (laserTimeLeft / (double)laserDuration));
            }

            laserActive = true;
            weaponCharged = false;

            // === Play looping laser sound ===
            PlayLaserSound();

            int totalDurationMs = laserDuration * 1000;
            int updateInterval = 50; // 20fps

            // Compute elapsed time from saved remaining seconds
            int elapsed = (int)((laserDuration - laserTimeLeft) * 1000);

            // --- Create laser beam sprite if not already ---
            if (laserBeam == null)
            {
                laserBeam = new PictureBox
                {
                    Image = Properties.Resources.laser_beam,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.Transparent,
                    Width = 50,
                    Height = 300,
                    Visible = false
                };
                panelBackground.Controls.Add(laserBeam);
                laserBeam.SendToBack();

                ImageAnimator.Animate(laserBeam.Image, OnFrameChanged);
            }

            laserBeam.Visible = true;

            // === Use persistent timer ===
            laserTimer?.Stop();
            laserTimer?.Dispose();

            laserTimer = new Timer { Interval = updateInterval };
            laserTimer.Tick += (s, e) =>
            {
                if (isPaused) return;

                elapsed += updateInterval;
                laserTimeLeft = Math.Max(0, laserDuration - (elapsed / 1000.0));

                if (elapsed >= totalDurationMs)
                {
                    laserTimer.Stop();
                    laserBeam.Visible = false;
                    laserActive = false;
                    StopLaserSound();
                    pbWeapon.Value = 0;
                    laserTimeLeft = 0;
                    DatabaseHelper.SaveSoundState(currentUser, bgMusicTime: 0, laserActive: false, laserTimeLeft: 0);
                }
            };
            laserTimer.Start();

            UpdateLaserPosition(laserBeam);

            // --- Follow rocket fast ---
            laserFollowTimer?.Stop();
            laserFollowTimer = new Timer { Interval = 10 };
            laserFollowTimer.Tick += (s, e) =>
            {
                if (isPaused) return;
                if (!laserActive)
                {
                    laserFollowTimer.Stop();
                    return;
                }
                UpdateLaserPosition(laserBeam);
            };
            laserFollowTimer.Start();

            // --- Unified Timer for both beam & bar ---
            laserSyncTimer?.Stop();
            laserSyncTimer = new Timer { Interval = updateInterval };
            laserSyncTimer.Tick += (s, e) =>
            {
                if (isPaused) return;

                elapsed += updateInterval;
                laserTimeLeft = Math.Max(0, laserDuration - (elapsed / 1000.0));

                double progress = Math.Min(1.0, (double)elapsed / totalDurationMs);
                pbWeapon.Value = (int)(pbWeapon.MaxValue * (1 - progress));
                pbWeapon.Refresh();

                if (progress >= 1.0)
                {
                    pbWeapon.Value = 0;
                    pbWeapon.Refresh();

                    laserBeam.Visible = false;
                    laserActive = false;
                    StopLaserSound();
                    laserSyncTimer.Stop();
                    laserFollowTimer.Stop();
                    laserTimer.Stop();
                    laserTimeLeft = 0;
                }
            };
            laserSyncTimer.Start();
        }

        public class LoopStream : WaveStream
        {
            private readonly WaveStream sourceStream;
            private const float FadeDurationSeconds = 0.15f; // duration of fade-in/out (adjust as desired)

            private readonly float[] fadeBuffer;
            private readonly int fadeSampleCount;

            public LoopStream(WaveStream sourceStream)
            {
                this.sourceStream = sourceStream ?? throw new ArgumentNullException(nameof(sourceStream));
                EnableLooping = true;

                fadeSampleCount = (int)(sourceStream.WaveFormat.SampleRate * FadeDurationSeconds * sourceStream.WaveFormat.Channels);
                fadeBuffer = new float[fadeSampleCount];
            }

            public bool EnableLooping { get; set; }

            public override WaveFormat WaveFormat => sourceStream.WaveFormat;
            public override long Length => sourceStream.Length;

            public override long Position
            {
                get => sourceStream.Position;
                set => sourceStream.Position = value;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int totalBytesRead = 0;

                while (totalBytesRead < count)
                {
                    int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);

                    if (bytesRead == 0)
                    {
                        if (!EnableLooping)
                            break;

                        // === Smooth transition ===
                        ApplyFadeOut(buffer, offset, totalBytesRead);

                        // loop back to start
                        sourceStream.Position = 0;

                        // optional: skip ahead to prevent harsh start (you can remove this)
                        if (sourceStream is AudioFileReader afr)
                            afr.CurrentTime = TimeSpan.FromSeconds(3.5);

                        // === Fade in on restart ===
                        int fadeInBytes = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                        ApplyFadeIn(buffer, offset + totalBytesRead, fadeInBytes);

                        totalBytesRead += fadeInBytes;
                        continue;
                    }

                    totalBytesRead += bytesRead;
                }

                return totalBytesRead;
            }

            private void ApplyFadeIn(byte[] buffer, int offset, int bytesRead)
            {
                int bytesPerSample = sourceStream.WaveFormat.BitsPerSample / 8;
                int sampleCount = bytesRead / bytesPerSample;

                for (int i = 0; i < sampleCount && i < fadeSampleCount; i++)
                {
                    float multiplier = (float)i / fadeSampleCount;

                    if (sourceStream.WaveFormat.BitsPerSample == 16)
                    {
                        short sample = BitConverter.ToInt16(buffer, offset + i * bytesPerSample);
                        float floatSample = sample / 32768f;
                        floatSample *= multiplier;
                        short newSample = (short)(floatSample * 32768f);
                        Array.Copy(BitConverter.GetBytes(newSample), 0, buffer, offset + i * bytesPerSample, bytesPerSample);
                    }
                    else if (sourceStream.WaveFormat.BitsPerSample == 32)
                    {
                        float sample = BitConverter.ToSingle(buffer, offset + i * bytesPerSample);
                        sample *= multiplier;
                        Array.Copy(BitConverter.GetBytes(sample), 0, buffer, offset + i * bytesPerSample, bytesPerSample);
                    }
                }
            }

            private void ApplyFadeOut(byte[] buffer, int offset, int bytesRead)
            {
                int bytesPerSample = sourceStream.WaveFormat.BitsPerSample / 8;
                int sampleCount = bytesRead / bytesPerSample;

                for (int i = 0; i < fadeSampleCount && i < sampleCount; i++)
                {
                    float multiplier = 1f - (float)i / fadeSampleCount;
                    int byteIndex = offset + (sampleCount - i - 1) * bytesPerSample;

                    if (sourceStream.WaveFormat.BitsPerSample == 16)
                    {
                        short sample = BitConverter.ToInt16(buffer, byteIndex);
                        float floatSample = sample / 32768f;
                        floatSample *= multiplier;
                        short newSample = (short)(floatSample * 32768f);
                        Array.Copy(BitConverter.GetBytes(newSample), 0, buffer, byteIndex, bytesPerSample);
                    }
                    else if (sourceStream.WaveFormat.BitsPerSample == 32)
                    {
                        float sample = BitConverter.ToSingle(buffer, byteIndex);
                        sample *= multiplier;
                        Array.Copy(BitConverter.GetBytes(sample), 0, buffer, byteIndex, bytesPerSample);
                    }
                }
            }
        }

        private void UpdateLaserPosition(PictureBox beam)
        {
            if (PlayerRocket == null || beam == null)
                return;

            int rocketCenterX = PlayerRocket.Left + (PlayerRocket.Width / 2) - (beam.Width / 2);
            int rocketTipY = PlayerRocket.Top;
            int beamHeight = Math.Max(10, rocketTipY);

            beam.Height = beamHeight;
            beam.Location = new Point(rocketCenterX, 0);
        }

        // ----------------------------------------
        //   SOUND MANAGEMENT
        // ----------------------------------------

        private void PlayLaserSound()
        {
            try
            {
                StopLaserSound(); // Stop any existing playback

                // === Locate external laser sound file ===
                string laserPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Images&Animations", "sfx", "Laser_Sound.wav");

                if (!File.Exists(laserPath))
                {
                    MessageBox.Show("Laser sound file not found:\n" + laserPath);
                    return;
                }

                // === Read WAV file ===
                var waveReader = new WaveFileReader(laserPath);

                // === Create looping stream with smooth transition ===
                laserLoop = new LoopStream(waveReader)
                {
                    EnableLooping = true
                };

                // === Start playback ===
                laserOutput = new WaveOutEvent();
                laserOutput.Init(laserLoop);
                laserOutput.Play();

                // Keep references alive
                laserReader = waveReader;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Laser sound error: {ex.Message}");
            }
        }

        private void StopLaserSound()
        {
            try
            {
                laserOutput?.Stop();
                laserOutput?.Dispose();
                laserOutput = null;

                laserLoop?.Dispose();
                laserLoop = null;

                laserReader?.Dispose();
                laserReader = null;
            }
            catch { }
        }

        private void StartBackgroundMusic()
        {
            try
            {
                if (bgMusicPlaying) return;

                string bgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Images&Animations", "sfx", "bg_music_game.wav");

                if (!File.Exists(bgPath))
                {
                    MessageBox.Show("Background music file not found:\n" + bgPath);
                    return;
                }

                bgReader = new AudioFileReader(bgPath);
                bgOutput = new WaveOutEvent();
                bgOutput.Init(bgReader);
                bgOutput.Play();
                bgMusicPlaying = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error playing background music: " + ex.Message);
            }
        }

        private void StopBackgroundMusic()
        {
            try
            {
                bgOutput?.Stop();
                bgOutput?.Dispose();
                bgOutput = null;

                bgReader?.Dispose();
                bgReader = null;

                bgMusicPlaying = false;
            }
            catch { }
        }

        private void StopAllSounds()
        {
            try
            {
                if (bgOutput != null)
                {
                    bgOutput.Stop();
                    bgOutput.Dispose();
                    bgOutput = null;
                }

                if (bgReader != null)
                {
                    bgReader.Dispose();
                    bgReader = null;
                }

                if (laserOutput != null)
                {
                    laserOutput.Stop();
                    laserOutput.Dispose();
                    laserOutput = null;
                }

                if (laserReader != null)
                {
                    laserReader.Dispose();
                    laserReader = null;
                }

                bgMusicPlaying = false;
            }
            catch { /* Ignore disposal errors */ }
        }

        // =====================================================
        //  OBSTACLE SYSTEM (Tier 1 Atmosphere to orbit)
        // =====================================================

        private void ObstacleSpawnTimer_Tick(object sender, EventArgs e)
        {
            // Pause spawning while controls locked or launch countdown active (adjust flag name if you used a different one)
            // If you don't have SessionManager.IsLaunchCountdownActive, just omit that check.
            if (controlsLocked)
                return;

            SpawnRandomObstacle();

            // randomize next spawn interval 3-6s
            obstacleSpawnTimer.Interval = obstacleRand.Next(3000, 6000);
        }


        // Global tracking list (place near your other fields)
        private List<PictureBox> activeObstacles = new List<PictureBox>();

        // ===================== OBSTACLE SPAWN =====================
        private void SpawnRandomObstacle()
        {
            if (panelBackground == null) return;

            // Limit maximum visible obstacles
            activeObstacles.RemoveAll(o => o == null || o.IsDisposed);
            if (activeObstacles.Count >= 15)
                return;

            string[] obstacleTypes = { "Airplane_smol", "AirBalloon_smol" };
            string chosen = obstacleTypes[obstacleRand.Next(obstacleTypes.Length)];
            Image sprite = (Image)Properties.Resources.ResourceManager.GetObject(chosen);
            if (sprite == null) return;

            PictureBox obstacle = new PictureBox
            {
                BackColor = Color.Transparent,
                Image = sprite
            };

            if (chosen == "AirBalloon_smol")
            {
                obstacle.Size = new Size(50, 150);
                obstacle.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else
            {
                obstacle.Size = new Size(150, 50);
                obstacle.SizeMode = PictureBoxSizeMode.StretchImage;
            }

            // Randomly pick side
            bool fromLeft = obstacleRand.Next(2) == 0;
            int panelH = panelBackground.Height;
            int minY = (int)(panelH * 0.2);
            int maxY = (int)(panelH * 0.7);

            int startY;
            int attempt = 0;
            do
            {
                startY = obstacleRand.Next(minY, maxY);
                attempt++;
            } while (Math.Abs(startY - lastSpawnY) < 150 && attempt < 8);
            lastSpawnY = startY;

            // --- Direction setup ---
            int direction = fromLeft ? 1 : -1; // right = +1, left = -1
            int speed = obstacleRand.Next(1, 4);
            int angleDeg = obstacleRand.Next(5, 11); // always 5–10° downward
            double angleRad = angleDeg * Math.PI / 180.0;

            // --- Position and facing ---
            if (fromLeft)
            {
                obstacle.Left = -obstacle.Width;
                obstacle.Top = startY;

                // Flip airplane (faces left by default)
                if (chosen == "Airplane_smol")
                {
                    try
                    {
                        if (Properties.Resources.ResourceManager.GetObject(chosen) is Image img)
                        {
                            Image flipped = (Image)img.Clone();
                            flipped.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            obstacle.Image = flipped;
                        }
                    }
                    catch { }
                }
            }
            else
            {
                obstacle.Left = panelBackground.Width;
                obstacle.Top = startY;
                direction = -1; // move left across screen

                // Airplane faces left by default — no flip needed
                if (chosen == "Airplane_smol")
                {
                    try
                    {
                        if (Properties.Resources.ResourceManager.GetObject(chosen) is Image img)
                            obstacle.Image = (Image)img.Clone();
                    }
                    catch { }
                }
            }

            // store movement data
            obstacle.Tag = new ObstacleData(speed, angleRad, direction)
            {
                LaserHitTime = 0
            };

            panelBackground.Controls.Add(obstacle);
            obstacle.SendToBack();
            activeObstacles.Add(obstacle);


            // === Altitude change timer ===
            Timer altitudeChangeTimer = new Timer { Interval = obstacleRand.Next(2000, 4000) };
            altitudeChangeTimer.Tick += (s, e) =>
            {
                if (!(obstacle.Tag is ObstacleData data)) return;
                int newAngleDeg = 50 + obstacleRand.Next(-26, 26);
                if (!fromLeft) newAngleDeg = -newAngleDeg;
                data.AngleRad = newAngleDeg * Math.PI / 180.0;
                altitudeChangeTimer.Interval = obstacleRand.Next(2000, 4000);
            };
            altitudeChangeTimer.Start();

            // === Movement & laser interaction timer ===
            Timer moveTimer = new Timer { Interval = 20 };
            moveTimer.Tick += (s, ev) =>
            {
                if (controlsLocked) return;
                if (!(obstacle.Tag is ObstacleData data)) return;

                obstacle.Left += data.Direction * data.Speed;
                obstacle.Top += (int)(Math.Sin(data.AngleRad) * (data.Speed * 0.9));

                // Clamp vertical movement
                if (obstacle.Top < minY) obstacle.Top = minY;
                if (obstacle.Bottom > panelH - 30) obstacle.Top = panelH - obstacle.Height - 30;

                // === Collision with rocket ===
                if (PlayerRocket != null && PixelPerfectCollision(obstacle, PlayerRocket))
                {
                    int damage = data.Speed * 5;

                    // Apply damage to HP
                    hp = Math.Max(0, hp - damage);

                    // Update HP bar safely
                    if (pbHP != null)
                    {
                        pbHP.Value = Math.Max(0, hp);
                        UpdateHPBarColor();
                    }

                    // Save current HP state immediately
                    DatabaseHelper.SavePlayerHP(currentUser, hp);

                    // If HP is zero, trigger the delayed Game Over sequence
                    if (hp <= 0 && !isGameOver)
                    {
                        StartGameOverCountdown();
                        return; // stop further processing for this tick
                    }

                    // Remove the obstacle (no score)
                    RemoveObstacle(obstacle, moveTimer, altitudeChangeTimer, destroyedByLaser: false, giveScore: false);
                    return;
                }



                // === Laser collision check (uses actual laser beam) ===
                if (laserBeam != null && laserBeam.Visible && laserActive)
                {
                    if (LaserCollision(obstacle))
                    {
                        data.LaserHitTime += 60; // +60 ms per tick

                        // Optional: flicker to show hit effect
                        if (data.LaserHitTime % 200 < 100)
                            obstacle.BackColor = Color.FromArgb(80, Color.Red);
                        else
                            obstacle.BackColor = Color.Transparent;

                        if (data.LaserHitTime >= 1000)
                        {
                            // 🔹 Destroy after 1s continuous contact — give double score
                            RemoveObstacle(obstacle, moveTimer, altitudeChangeTimer, destroyedByLaser: true);
                            return;
                        }
                    }
                    else
                    {
                        data.LaserHitTime = 0;
                        obstacle.BackColor = Color.Transparent;
                    }
                }

                // === Remove when off-screen ===
                if (obstacle.Right < -120 || obstacle.Left > panelBackground.Width + 120)
                {
                    RemoveObstacle(obstacle, moveTimer, altitudeChangeTimer);
                }
            };
            moveTimer.Start();
        }

        // ===================== HELPER METHODS =====================
        private void RemoveObstacle(PictureBox obstacle, Timer moveTimer, Timer altitudeTimer, bool destroyedByLaser = false, bool giveScore = true)
        {
            // Skip scoring if the game is already over or paused/locked
            if (giveScore && !controlsLocked && !isPaused && pbHP.Value > 0 && !isGameOver)
            {
                if (obstacle?.Tag is ObstacleData data)
                {
                    // --- Base score based on obstacle speed ---
                    int scoreGain = data.Speed * 5; // 1→5, 2→10, 3→15, 4→20

                    // --- Double score if destroyed by laser ---
                    if (destroyedByLaser)
                        scoreGain *= 2;

                    score += scoreGain;

                    // Update on-screen score label
                    if (lblScore != null)
                        lblScore.Content = $"Score: {score}";

                    // Save immediately
                    SavePlayerProgress();
                }
            }

            // --- Remove obstacle safely ---
            if (panelBackground.Controls.Contains(obstacle))
                panelBackground.Controls.Remove(obstacle);

            activeObstacles.Remove(obstacle);

            try { obstacle.Dispose(); } catch { }

            moveTimer.Stop();
            moveTimer.Dispose();
            altitudeTimer.Stop();
            altitudeTimer.Dispose();
        }

        private bool LaserCollision(PictureBox obstacle)
        {
            // Ensure laser is active and visible
            if (laserBeam == null || !laserBeam.Visible || !laserActive)
                return false;

            // Simple bounding-box check for now (fast)
            return obstacle.Bounds.IntersectsWith(laserBeam.Bounds);
        }

        private bool PixelPerfectCollision(PictureBox a, PictureBox b)
        {
            if (a == null || b == null)
                return false;

            // Quick bounding-box reject
            Rectangle rectA = a.Bounds;
            Rectangle rectB = b.Bounds;
            Rectangle intersect = Rectangle.Intersect(rectA, rectB);
            if (intersect.IsEmpty)
                return false;

            // Helper to build a bitmap that matches how the PictureBox is drawn on screen.
            Bitmap BuildDisplayBitmap(PictureBox pb)
            {
                Image img = pb.Image;

                // If PictureBox has no Image and it's the PlayerRocket, use the currently animated sprite.
                if (img == null && pb == PlayerRocket && currentRocketSprite != null)
                    img = currentRocketSprite;

                if (img == null)
                    return null;

                // Create bitmap that matches the drawn size of the picturebox
                try
                {
                    Bitmap bmp = new Bitmap(img, pb.Size);
                    return bmp;
                }
                catch
                {
                    // fallback: try original image sized
                    try { return new Bitmap(img); } catch { return null; }
                }
            }

            using (Bitmap bmpA = BuildDisplayBitmap(a))
            using (Bitmap bmpB = BuildDisplayBitmap(b))
            {
                if (bmpA == null || bmpB == null)
                    return false;

                // Intersection coordinates are in parent/control coordinates.
                // We'll sample pixels inside the intersection and map them to the local bitmap coordinates.
                // Use a small step to improve performance; set to 1 for perfect accuracy if you need.
                int step = 2;
                const int alphaThreshold = 40; // pixels with alpha <= threshold are considered transparent

                for (int x = intersect.Left; x < intersect.Right; x += step)
                {
                    for (int y = intersect.Top; y < intersect.Bottom; y += step)
                    {
                        int ax = x - rectA.Left;
                        int ay = y - rectA.Top;
                        int bx = x - rectB.Left;
                        int by = y - rectB.Top;

                        // Bounds sanity checks
                        if (ax < 0 || ay < 0 || bx < 0 || by < 0 ||
                            ax >= bmpA.Width || ay >= bmpA.Height ||
                            bx >= bmpB.Width || by >= bmpB.Height)
                            continue;

                        Color ca = bmpA.GetPixel(ax, ay);
                        Color cb = bmpB.GetPixel(bx, by);

                        if (ca.A > alphaThreshold && cb.A > alphaThreshold)
                            return true;
                    }
                }
            }

            return false;
        }

        private class ObstacleData
        {
            public int Speed { get; set; }
            public double AngleRad { get; set; }
            public int Direction { get; set; }
            public double LaserHitTime { get; set; } // seconds of laser contact

            public ObstacleData(int speed, double angleRad, int direction)
            {
                Speed = speed;
                AngleRad = angleRad;
                Direction = direction;
                LaserHitTime = 0;
            }
        }


        // =====================================================
        //  Power up system (with rarity and scaling)
        // =====================================================

        private void SpawnPowerup()
        {
            if (panelBackground == null || PlayerRocket == null) return;

            // --- prevent laser battery spawn if laser is active or full ---
            bool canSpawnLaser = !(laserActive || (pbWeapon != null && pbWeapon.Value >= pbWeapon.MaxValue));

            // Weighted type probabilities
            // Total = 100
            // Coin (10%), Fuel (40%), Repair (40%), LaserBattery (10%)
            string chosen;
            int roll = powerupRand.Next(100); // 0–99

            if (roll < 10)
                chosen = "Coin";
            else if (roll < 50)
                chosen = "Fuel";       // 40% (10–49)
            else if (roll < 90)
                chosen = "Repair";     // 40% (50–89)
            else
                chosen = "LaserBattery"; // 10% (90–99)


            // Avoid LaserBattery when disallowed
            if (chosen == "LaserBattery" && !canSpawnLaser)
            {
                // reroll between others (weighted again)
                int reroll = powerupRand.Next(90);
                if (reroll < 10) chosen = "Coin";
                else if (reroll < 85) chosen = "Fuel";
                else chosen = "Repair";
            }

            PictureBox p = new PictureBox
            {
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            int fallSpeed = 2;
            PowerupData tag = null;

            // --- COIN with rarity and scaling ---
            if (chosen == "Coin")
            {
                // Weighted rarity for coin values
                int coinRoll = powerupRand.Next(100);
                int coinValue;
                if (coinRoll < 50) coinValue = 1;      // 50%
                else if (coinRoll < 75) coinValue = 5; // 25%
                else if (coinRoll < 90) coinValue = 10; // 15%
                else coinValue = 20;                    // 10%

                p.Image = Properties.Resources.coin;
                tag = new PowerupData("Coin", coinValue);

                // Size and fall speed scale with value
                if (coinValue == 1)
                {
                    p.Size = new Size(35, 35);
                    fallSpeed = 2;
                }
                else if (coinValue == 5)
                {
                    p.Size = new Size(45, 45);
                    fallSpeed = 3;
                }
                else if (coinValue == 10)
                {
                    p.Size = new Size(55, 55);
                    fallSpeed = 4;
                }
                else // 20 coin
                {
                    p.Size = new Size(65, 65);
                    fallSpeed = 5;
                }
            }
            else if (chosen == "Fuel")
            {
                p.Image = Properties.Resources.Fuel;
                p.Size = new Size(55, 55);
                tag = new PowerupData("Fuel", 0);
                fallSpeed = 2;
            }
            else if (chosen == "Repair")
            {
                p.Image = Properties.Resources.repair_tool;
                p.Size = new Size(50, 50);
                tag = new PowerupData("Repair", 0);
                fallSpeed = 2;
            }
            else // LaserBattery (rare)
            {
                p.Image = Properties.Resources.laser_battery;
                p.Size = new Size(50, 70);
                tag = new PowerupData("LaserBattery", 0);
                fallSpeed = 3;
            }

            p.Tag = tag;

            // --- Spawn from random X position ---
            int left = powerupRand.Next(30, Math.Max(100, panelBackground.Width - 80));
            p.Left = left;
            p.Top = -p.Height;

            panelBackground.Controls.Add(p);
            p.SendToBack();
            activePowerups.Add(p);

            // --- Animate falling ---
            Timer fallTimer = new Timer { Interval = 20 };
            fallTimer.Tick += (s, e) =>
            {
                if (controlsLocked || isPaused) return;
                if (p == null || p.IsDisposed)
                {
                    fallTimer.Stop();
                    fallTimer.Dispose();
                    return;
                }

                p.Top += fallSpeed;

                // --- Collision with player rocket ---
                if (PixelPerfectCollision(p, PlayerRocket))
                {
                    if (p.Tag is PowerupData data) ApplyPowerupEffect(data);
                    RemovePowerup(p, fallTimer);
                    return;
                }

                // --- Off-screen cleanup ---
                if (p.Top > panelBackground.Height)
                {
                    RemovePowerup(p, fallTimer);
                    return;
                }
            };
            fallTimer.Start();
        }

        private void ApplyPowerupEffect(PowerupData data)
        {
            if (data == null) return;

            if (data.Type == "Coin")
            {
                coins += data.Value;
                sessionCoins += data.Value;   // this run only
                if (lblCoins != null) lblCoins.Content = FormatCoins(coins);
                DatabaseHelper.UpdatePlayerCoins(currentUser, coins);
            }
            else if (data.Type == "Fuel")
            {
                fuel = 100;
                if (pbFuel != null) pbFuel.Value = fuel;
                DatabaseHelper.SavePlayerFuel(currentUser, fuel);
            }
            else if (data.Type == "Repair")
            {
                // ✅ Heal +25 HP, capped at armor-based maxHP
                hp = Math.Min(hp + 25, maxHP);

                if (pbHP != null)
                {
                    pbHP.Value = hp;
                    UpdateHPBarColor();
                }

                DatabaseHelper.SavePlayerHP(currentUser, hp);
            }
            else if (data.Type == "LaserBattery")
            {
                if (!laserActive && pbWeapon != null && pbWeapon.Value < pbWeapon.MaxValue)
                {
                    pbWeapon.Value = pbWeapon.MaxValue;
                    weaponCharged = true;
                    laserTimeLeft = rocketWeapon * 4;
                    DatabaseHelper.SaveSoundState(currentUser, bgReader?.CurrentTime.TotalSeconds ?? 0, false, laserTimeLeft);
                }
            }
        }

        private void RemovePowerup(PictureBox p, Timer fallTimer)
        {
            if (panelBackground.Controls.Contains(p))
                panelBackground.Controls.Remove(p);
            activePowerups.Remove(p);
            try { p.Dispose(); } catch { }
            fallTimer.Stop();
            fallTimer.Dispose();
        }

        private class PowerupData
        {
            public string Type { get; private set; }
            public int Value { get; private set; }
            public PowerupData(string type, int value)
            {
                Type = type;
                Value = value;
            }
        }


    }
}