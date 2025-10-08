using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RocketOdyssey.Database;

namespace RocketOdyssey
{
    public partial class GameControl : UserControl
    {
        // Background scrolling
        private Timer scrollTimer;
        private Image backgroundImage;
        private int scrollSpeed = 0;          // Background scroll speed
        private int scrollOffset = 0;

        // Player movement state
        // Add this with other fields
        private bool controlsLocked = true; // Rocket controls disabled at start
        private bool upPressed = false;
        private bool downPressed = false;
        private bool leftPressed = false;
        private bool rightPressed = false;

        // GIF sprites
        private Image idleGif;
        private Image upGif;
        private Image downGif;
        private Image landGif;

        // Movement settings
        private int moveStep = 0;             // Rocket movement speed
        private readonly int minX = 0;
        private readonly int minY = 140;
        private readonly int maxX = 650;
        private readonly int maxY = 550;

        // Holds whichever sprite is currently active (idle/up/down)
        private Image currentRocketSprite;
        private float currentRotation = 0f;   // in degrees

        // For saving user state later
        private string currentUser;

        // Player stats
        private int score = 0;
        private int coins = 0;
        private int hp = 100;             // will be overridden by DB
        private int fuel = 100;           // starts full, drains during play
        private int rocketSpeed = 0;
        private int rocketWeapon = 0;

        // Rocket fuel Timer
        private Timer fuelTimer;          // drains fuel periodically

        // Fuel drain
        private int fuelDrainRate = 1;       // how much fuel drains every tick
        private int fuelDrainInterval = 1000; // every 1 second

        // Movement timer
        private Timer moveTimer;

        // NEW: tracks that the rocket has run out of fuel (prevents sprite changes)
        private bool isOutOfFuel = false;

        public GameControl()
        {
            InitializeComponent();
            InitializeBackgrounds();

            // Load current user from SessionManager
            currentUser = SessionManager.CurrentUsername;

            // ---- Load player upgrades from DB ----
            var (speed, armor, weapon) = DatabaseHelper.GetPlayerUpgrades(currentUser);
            // Load coins from DB
            coins = DatabaseHelper.GetPlayerCoins(currentUser);

            rocketSpeed = speed;
            hp = armor;
            rocketWeapon = weapon;

            // === Initialize UI ===
            pbHP.MaxValue = 100;
            pbHP.Value = hp;                     // start from DB or default
            pbFuel.MaxValue = 100;
            pbFuel.Value = fuel;                 // full at start

            lblScore.Content = $"Score: {score}";
            coins += 5;  // example: collected 5 coins
            lblCoins.Content = $"{coins}";         // or load from DB if you have coins

            // Adjust movement speed based on upgrade
            moveStep += rocketSpeed; // default + upgrade bonus
            fuel = 100; // reset full tank

            // Disable controls for 10 seconds after launch
            Timer launchDelayTimer = new Timer();
            launchDelayTimer.Interval = 10000; // 10 seconds
            launchDelayTimer.Tick += (s, e) =>
            {
                controlsLocked = false;         // unlock movement
                launchDelayTimer.Stop();        // stop the timer
            };
            launchDelayTimer.Start();

            // ---- Background setup ----
            backgroundImage = panelBackground.BackgroundImage;
            panelBackground.Paint += panelBackground_Paint;
            panelBackground.BackgroundImage = null;
            SetDoubleBuffered(panelBackground);

            // ---- Player sprite setup ----
            idleGif = Properties.Resources.rocket_2_smol;
            upGif = Properties.Resources.rocket_2_smol_up;
            downGif = Properties.Resources.rocket_2_smol_down;
            landGif = Properties.Resources.rocket_2_land_smol;

            currentRocketSprite = idleGif;

            // register all gifs for animation (including landing GIF)
            ImageAnimator.Animate(idleGif, OnFrameChanged);
            ImageAnimator.Animate(upGif, OnFrameChanged);
            ImageAnimator.Animate(downGif, OnFrameChanged);
            ImageAnimator.Animate(landGif, OnFrameChanged);

            PlayerRocket.Image = null;
            PlayerRocket.Paint += PlayerRocket_Paint;
            PlayerRocket.BackColor = Color.Transparent;

            // Scroll timer (~60 FPS)
            scrollTimer = new Timer();
            scrollTimer.Interval = 16;
            scrollTimer.Tick += ScrollTimer_Tick;
            scrollTimer.Start();

            // ---- Input setup ----
            this.KeyDown += GameControl_KeyDown;
            this.KeyUp += GameControl_KeyUp;
            

            this.SetStyle(ControlStyles.Selectable, true);
            this.TabStop = true;
            this.Focus();

            // ---- Movement timer ----
            moveTimer = new Timer();
            moveTimer.Interval = 16;
            moveTimer.Tick += MoveTimer_Tick;
            moveTimer.Start();

            // ---- Fuel timer ----
            fuelTimer = new Timer();
            fuelTimer.Interval = fuelDrainInterval;
            fuelTimer.Tick += FuelTimer_Tick;
            fuelTimer.Start();
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
            if (backgroundStages == null || backgroundStages.Count == 0)
                return;

            currentStageOffset += scrollSpeed;

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

                if (leftPressed && rightPressed)
                    currentRotation = 0f;
                else if (leftPressed)
                    currentRotation = -15f;
                else if (rightPressed)
                    currentRotation = 15f;
            }

            PlayerRocket.Invalidate();
        }

        private void GameControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (controlsLocked) return; // Ignore input during launch delay
            if (isOutOfFuel) return;    // Prevent resetting sprite to idle after fuel runs out

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
                if (leftPressed && rightPressed)
                    currentRotation = 0f;
                else if (leftPressed)
                    currentRotation = -15f;
                else if (rightPressed)
                    currentRotation = 15f;
                else
                    currentRotation = 0f;
            }

            PlayerRocket.Invalidate();
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

        private void FuelTimer_Tick(object sender, EventArgs e)
        {
            if (isOutOfFuel) return; // nothing further once out of fuel

            if (fuel > 0)
            {
                fuel -= fuelDrainRate;
                if (fuel < 0) fuel = 0;

                pbFuel.Value = fuel;

                if (fuel == 0)
                {
                    // Mark as out of fuel so input handlers stop changing sprite/rotation
                    isOutOfFuel = true;

                    // Switch sprite to landing (keep currentRotation as last set)
                    currentRocketSprite = landGif;
                    PlayerRocket.Invalidate();

                    // Stop movement immediately, but let background scroll for 5s
                    moveTimer.Stop();
                    StartGameOverCountdown();
                }
            }
        }

        private void StartGameOverCountdown()
        {
            int countdown = 5;

            Timer countdownTimer = new Timer();
            countdownTimer.Interval = 1000; // 1 second
            countdownTimer.Tick += (s, ev) =>
            {
                countdown--;
                if (countdown <= 0)
                {
                    countdownTimer.Stop();

                    // Stop scrolling AFTER the 5-second delay
                    scrollTimer.Stop();

                    MessageBox.Show("Out of fuel! Game Over.");
                    SavePlayerProgress();
                }
            };

            countdownTimer.Start();
        }

        private void SavePlayerProgress()
        {
            DatabaseHelper.UpdateUpgrade(currentUser, "RocketArmor", hp);
            DatabaseHelper.UpdateUpgrade(currentUser, "RocketSpeed", rocketSpeed);
            DatabaseHelper.UpdateUpgrade(currentUser, "RocketWeapon", rocketWeapon);

            DatabaseHelper.UpdateUpgrade(currentUser, "Score", score);
            DatabaseHelper.UpdatePlayerCoins(currentUser, coins);
        }

        private void GameControl_Enter(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {

        }
    }
}
