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
        // Background scrolling
        private Timer scrollTimer;
        private Image backgroundImage;
        private int scrollSpeed = 1; // Background scroll speed

        // Player movement state
        private bool controlsLocked = true;
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
        private int moveStep = 1;
        private readonly int minX = 0;
        private readonly int minY = 140;
        private readonly int maxX = 650;
        private readonly int maxY = 550;

        // Active rocket sprite
        private Image currentRocketSprite;
        private float currentRotation = 0f;

        // Player data
        private string currentUser;
        private int score = 0;
        private int coins = 0;
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
        private int fuelDrainInterval = 1000;

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

        private bool focusRecoveryEnabled = true;

        public GameControl()
        {
            InitializeComponent();
            InitializeBackgrounds();
            // Ensure GameControl regains focus when any label/panel is clicked
            EnableAutomaticFocusRecovery();

            currentUser = SessionManager.CurrentUsername;

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

            // ---- Load coins and other state ----
            coins = DatabaseHelper.GetPlayerCoins(currentUser);
            hp = DatabaseHelper.LoadPlayerHP(currentUser);
            fuel = DatabaseHelper.LoadPlayerFuel(currentUser);

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

            // ---- Launch countdown ----
            launchCountdown = DatabaseHelper.LoadLaunchTimer(currentUser);
            bool hasProgress = fuel < 100 || hp < maxHP || currentStageIndex > 0 || currentStageOffset > 0;
            if (hasProgress && launchCountdown == 0)
                controlsLocked = false;
            else
            {
                if (launchCountdown == 0)
                    launchCountdown = 10;

                controlsLocked = true;
                launchDelayTimer = new Timer();
                launchDelayTimer.Interval = 1000;
                launchDelayTimer.Tick += LaunchDelayTimer_Tick;
                launchDelayTimer.Start();
            }

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
            fuelTimer.Start();

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
                pbHP.ForeColor = Color.LimeGreen;
            else if (ratio > 0.5)
                pbHP.ForeColor = Color.YellowGreen;
            else if (ratio > 0.25)
                pbHP.ForeColor = Color.Orange;
            else
                pbHP.ForeColor = Color.Red;
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
            // Example: 100 (base), 125 (lv1), 150 (lv2), 175 (lv3), 200 (lv4)
            int armorLevel = (maxHP - 100) / 25;
            int newMaxHP = 100 + (armorLevel * 25);

            // Increase current HP by same armor bonus (no overheal past max)
            hp = Math.Min(hp + (armorLevel * 25), newMaxHP);

            pbHP.MaxValue = newMaxHP;
            pbHP.Value = Math.Min(hp, pbHP.MaxValue);
            lblHP1.Content = $"HP: {pbHP.Value}/{pbHP.MaxValue}";
            UpdateHPBarColor();

            // Save to DB for persistence
            DatabaseHelper.SavePlayerHP(currentUser, hp);
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
        //   Game Over Sequence
        // ----------------------------------------

        private void StartGameOverCountdown()
        {
            int countdown = 5;
            Timer countdownTimer = new Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += (s, ev) =>
            {
                countdown--;
                if (countdown <= 0)
                {
                    countdownTimer.Stop();
                    scrollTimer.Stop();

                    // 🔹 Stop all sounds before going back
                    StopAllSounds();

                    // 🔹 Reset background music timeline
                    bgMusicTime = 0;
                    DatabaseHelper.SaveSoundState(
                        currentUser,
                        bgMusicTime: 0,
                        laserActive: false,
                        laserTimeLeft: 0
                    );

                    MessageBox.Show("Out of fuel! Game Over.");

                    DatabaseHelper.UpdateHighScore(currentUser, score);

                    score = 0;
                    hp = 100;
                    fuel = 100;
                    currentStageIndex = 0;
                    currentStageOffset = 0;
                    PlayerRocket.Location = new Point(326, 510);
                    launchCountdown = 10;

                    DatabaseHelper.SavePlayerFuel(currentUser, fuel);
                    DatabaseHelper.SavePlayerHP(currentUser, hp);
                    DatabaseHelper.SavePlayerState(
                        currentUser,
                        PlayerRocket.Location.X,
                        PlayerRocket.Location.Y,
                        currentStageIndex,
                        currentStageOffset
                    );
                    DatabaseHelper.SaveLaunchTimer(currentUser, launchCountdown);
                    DatabaseHelper.UpdateUpgrade(currentUser, "Score", score);

                    // 🔹 Return to main menu (fresh state, fresh music)
                    MainMenuControl menu = new MainMenuControl();
                    GameForm parentForm = (GameForm)this.FindForm();
                    parentForm.Controls.Clear();
                    parentForm.Controls.Add(menu);
                }
            };
            countdownTimer.Start();
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
            // Save runtime data
            DatabaseHelper.SavePlayerFuel(currentUser, fuel);
            DatabaseHelper.SavePlayerHP(currentUser, hp);
            DatabaseHelper.SavePlayerState(currentUser, PlayerRocket.Location.X, PlayerRocket.Location.Y, currentStageIndex, currentStageOffset);
            DatabaseHelper.SaveLaunchTimer(currentUser, launchCountdown);

            // Save current temp upgrades for next play session
            DatabaseHelper.SaveTempUpgrades(currentUser, rocketSpeed, maxHP, rocketWeapon);

            // Coins and scores are permanent
            DatabaseHelper.UpdatePlayerCoins(currentUser, coins);
            DatabaseHelper.UpdateHighScore(currentUser, score);
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

            // 🔹 Reset music timeline to 0 before starting a new game
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
            // if countdown is active, save the remaining seconds
            if (launchDelayTimer != null && launchDelayTimer.Enabled)
            {
                launchDelayTimer.Stop();  // stop it so it doesn’t tick after leaving
                DatabaseHelper.SaveLaunchTimer(currentUser, launchCountdown);
            }

            // Save sound state before leaving or pausing
            if (bgReader != null)
                bgMusicTime = bgReader.CurrentTime.TotalSeconds;

            DatabaseHelper.SaveSoundState(
                currentUser,
                bgMusicTime,
                laserActive,
                laserTimeLeft
            );

            StopAllSounds(); // stop both bg and laser
            SavePlayerProgress(); // also saves fuel, hp, etc.

            // back to menu
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

                // --- 🔹 Immediately save sound state when pausing ---
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

                // --- 🔹 Restore laserTimeLeft from DB if available ---
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
                laserBeam.BringToFront();

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

    }
}
