using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Frogger.Model;

namespace Frogger.Controller
{
    /// <summary>
    ///     Manages all aspects of the game play including the player,
    ///     the vehicles as well as lives and score.
    /// </summary>
    public class GameManager
    {
        #region Data members

        private DispatcherTimer timer;
        private DispatcherTimer lifeDispatcherTimer;

        private readonly LaneManager laneManager;

        private readonly IList<HomeLandingSpot> homeLandingSpots = new List<HomeLandingSpot>();
        private readonly SoundEffects soundEffects;
        private readonly PowerUp powerUp = new PowerUp();
        private int homeLandingCount;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the time count down.
        /// </summary>
        /// <value>
        ///     The time count down.
        /// </value>
        public int TimeCountDown { get; set; }

        /// <summary>
        ///     Gets the player manager.
        /// </summary>
        /// <value>
        ///     The player manager.
        /// </value>
        public PlayerManager PlayerManager { get; }

        /// <summary>
        ///     Gets the lives of the player.
        /// </summary>
        /// <value>
        ///     The lives of the player.
        /// </value>
        public int Lives { get; set; }

        /// <summary>
        ///     Gets the current score of the player
        /// </summary>
        /// <value>
        ///     The current score of the player.
        /// </value>
        public int Score { get; set; }

        /// <summary>
        ///     Gets or sets the level.
        /// </summary>
        /// <value>
        ///     The level.
        /// </value>
        public int Level { get; set; }

        private Canvas GameCanvas { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameManager" /> class.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     backgroundHeight &lt;= 0
        ///     or
        ///     backgroundWidth &lt;= 0
        /// </exception>
        public GameManager(Canvas gameCanvas)
        {
            this.GameCanvas = gameCanvas;

            this.setupGameTimer(gameCanvas);
            this.setupLifeTimer();
            this.PlayerManager = new PlayerManager(gameCanvas);
            this.laneManager = new LaneManager();
            this.soundEffects = new SoundEffects();

            this.PlayerManager.AnimationOver += this.startGame;
            this.PlayerManager.AnimationStarted += this.stopGame;
        }


        #endregion

        #region Methods

        private void stopGame(object sender, EventArgs e)
        {
            this.timer.Stop();
        }

        private void startGame(object sender, EventArgs e)
        {
            this.timer.Start();
        }

        /// <summary>
        ///     Occurs when [level updated].
        /// </summary>
        public event EventHandler LevelUpdated;

        /// <summary>
        ///     Occurs when [time out].
        /// </summary>
        public event EventHandler TimeOut;

        /// <summary>
        ///     Event Occurs when [lives updated].
        /// </summary>
        public event EventHandler LivesUpdated;

        /// <summary>
        ///     Event Occurs when [score updated].
        /// </summary>
        public event EventHandler ScoreUpdated;

        /// <summary>
        ///     Event Occurs when [game over].
        /// </summary>
        public event EventHandler GameOver;

        /// <summary>
        ///     Initializes the game working with appropriate classes to place frog
        ///     and vehicle on game screen.
        ///     Precondition: gameCanvas != null
        ///     Postcondition: Game is initialized and ready for play.
        /// </summary>
        /// <param name="gameCanvas">The game canvas.</param>
        /// <exception cref="ArgumentNullException">gameCanvas</exception>
        public void InitializeGame(Canvas gameCanvas)
        {
            if (gameCanvas == null)
            {
                throw new ArgumentNullException(nameof(gameCanvas));
            }

            this.configureLevelParameters(gameCanvas);
            this.createHomeLandingSPots(gameCanvas);
        }

        /// <summary>
        ///     Resets the game.
        /// </summary>
        /// <param name="gameCanvas">The game canvas.</param>
        public void ResetGame(Canvas gameCanvas)
        {
            this.timer.Stop();
            this.lifeDispatcherTimer.Stop();

            this.laneManager.ClearLanesAndVehicles(gameCanvas);

            this.unOccupyHomeLandingSpots();

            this.resetGameStats();

            this.timer.Start();
            this.lifeDispatcherTimer.Start();

            this.configureLevelParameters(gameCanvas);
        }

        private void configureLevelParameters(Canvas gameCanvas)
        {
            switch (this.Level)
            {
                case 1:
                    LaneManager.LaneSpeeds = new[] { 0.1, 0.2, 0.3, 0.4, 0.5 };
                    LaneManager.VehiclesPerLane = new[] { 2, 1, 3, 2, 4 };

                    this.laneManager.CreateAndPlaceLanes(gameCanvas);
                    this.PlayerManager.CreateAndPlacePlayer();
                    break;

                case 2:
                    LaneManager.LaneSpeeds = new[] { 0.3, 0.4, 0.5, 0.6, 0.7 };
                    LaneManager.VehiclesPerLane = new[] { 3, 2, 4, 3, 5 };
                    this.laneManager.CreateAndPlaceLanes(gameCanvas);
                    this.PlayerManager.CreateAndPlacePlayer();
                    break;

                case 3:
                    LaneManager.LaneSpeeds = new[] { 0.1, 0.2, 0.3, 0.4, 0.5 };
                    LaneManager.VehiclesPerLane = new[] { 4, 3, 5, 4, 6 };
                    this.laneManager.CreateAndPlaceLanes(gameCanvas);
                    this.PlayerManager.CreateAndPlacePlayer();
                    break;
            }
        }

        private void createHomeLandingSPots(Canvas gameCanvas)
        {
            var numPods = 5;
            const int podWidth = 60;
            var highShoulderWidth = (double)Application.Current.Resources["AppWidth"];
            var availableSpace = highShoulderWidth - numPods * podWidth;
            var podSpacing = availableSpace / (numPods - 1);

            for (var i = 0; i < numPods; i++)
            {
                var pod = new HomeLandingSpot();
                this.homeLandingSpots.Add(pod);
                gameCanvas.Children.Add(pod.Sprite);
                pod.X = i * (podWidth + podSpacing);
                pod.Y = (double)Application.Current.Resources["HighShoulderYLocation"];
            }
        }

        private async void timerOnTick(Canvas gameCanvas)
        {
            if (this.Lives <= 0)
            {
                this.GameOver?.Invoke(this, EventArgs.Empty);
                await this.soundEffects.GameOverSound();
                this.timer.Stop();
                this.lifeDispatcherTimer.Stop();
            }
            else if (this.allHomeLandingSpotsOccupied() && this.Level < 4)
            {
                this.unOccupyHomeLandingSpots();

                this.Level++;
                await this.soundEffects.LevelUpSound();
                this.deactivatePowerUp();
                this.homeLandingCount = 0;
                this.configureLevelParameters(gameCanvas);
                this.LevelUpdated?.Invoke(this, EventArgs.Empty);

                if (this.Level <= 3)
                {
                    this.configureLevelParameters(gameCanvas);
                    this.LevelUpdated?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    this.GameOver?.Invoke(this, EventArgs.Empty);
                    await this.soundEffects.GameOverSound();
                    this.timer.Stop();
                    this.lifeDispatcherTimer.Stop();
                }
            }

            this.moveVehicle();
            this.updateScore();
        }

        private void setupGameTimer(Canvas gameCanvas)
        {
            this.timer = new DispatcherTimer();
            this.timer.Tick += (sender, e) => this.timerOnTick(gameCanvas);
            this.timer.Interval = TimeSpan.FromMilliseconds(15);
            this.timer.Start();
        }

        private void setupLifeTimer()
        {
            this.lifeDispatcherTimer = new DispatcherTimer();
            this.lifeDispatcherTimer.Tick += this.lifeDispatcherTimerOnTick;
            this.lifeDispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            this.lifeDispatcherTimer.Start();
        }

        private void lifeDispatcherTimerOnTick(object sender, object e)
        {
            this.TimeCountDown--;
            this.onTimeOutChanged();
        }

        private bool allHomeLandingSpotsOccupied()
        {
            foreach (var spot in this.homeLandingSpots)
            {
                if (!spot.PodOccupied)
                {
                    return false;
                }
            }

            return true;
        }

        private void moveVehicle()
        {
            this.laneManager.VehicleManager.MoveVehicle();

            foreach (var lane in this.laneManager.Lanes)
            {
                this.collusionManagement(lane);
            }
        }

        private void collusionManagement(Lane lane)
        {
            foreach (var vehicle in lane.Vehicles)
            {
                if (vehicle.CheckCollision(this.PlayerManager.Player))
                {
                    this.handleCollision();
                }
            }
        }

        private async void handleCollision()
        { 
            await this.soundEffects.DyingSound();
            this.Lives--;
            this.onLivesUpdated();
            this.TimeCountDown = 20;
        }

        private void updateScore()
        {
            var landedHome = false;
            var dintLandHome = false;

            foreach (var spot in this.homeLandingSpots)
            {
                if (spot.CheckCollision(this.PlayerManager.Player) &&
                    !spot.PodOccupied)
                {
                    spot.OccupyPodWithFrog();
                    landedHome = true;
                    break;
                }

                if (this.PlayerManager.Player.Y <= (double)Application.Current.Resources["HighShoulderYLocation"])
                {
                    dintLandHome = true;
                }
            }

            if (landedHome)
            {
                this.handleLandingHome();
            }
            else if (dintLandHome || this.TimeCountDown == 0)
            {
                this.handleDintLandingHome();
            }
        }

        private void handleDintLandingHome()
        {
            this.TimeCountDown = 20;
            this.Lives--;
            this.onLivesUpdated();
            this.PlayerManager.SetPlayerToCenterOfBottomShoulder();
        }

        private async void handleLandingHome()
        {
            await this.soundEffects.LandingHomeSounds();
            var increaseScoreBy = this.TimeCountDown;

            if (this.powerUp.IsActive)
            {
                increaseScoreBy *= this.powerUp.HasDoubleScoreEffect ? 2 : 1;
            }

            this.Score += increaseScoreBy;

            if (++this.homeLandingCount == 3)
            {
                await this.soundEffects.LandingHomeSounds();
                this.activatePowerUp();
            }

            this.onScoreUpdated();
            this.PlayerManager.SetPlayerToCenterOfBottomShoulder();
            this.TimeCountDown = 20;
        }

        private async void activatePowerUp()
        {
            this.powerUp.Activate(true);
            await this.soundEffects.PowerUpSound();
        }

        private void deactivatePowerUp()
        {
            this.powerUp.Deactivate();
        }

        private void onScoreUpdated()
        {
            this.ScoreUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void onLivesUpdated()
        {
            this.LivesUpdated?.Invoke(this, EventArgs.Empty);
        }

        private async void onTimeOutChanged()
        {
            this.TimeOut?.Invoke(this, EventArgs.Empty);
            if (this.TimeCountDown == 0)
            {
                this.TimeCountDown = 20;
                this.Lives--;
                this.onLivesUpdated();
                await this.soundEffects.DyingSound();
                this.lifeDispatcherTimer.Start();
            }

            this.onLivesUpdated();
        }

        private void unOccupyHomeLandingSpots()
        {
            foreach (var spot in this.homeLandingSpots)
            {
                spot.UnoccupySpot();
            }
        }

        private void resetGameStats()
        {
            this.Lives = 4;
            this.onLivesUpdated();
            this.Score = 0;
            this.onScoreUpdated();
            this.Level = 1;
            this.TimeCountDown = 20;
            this.onTimeOutChanged();
            this.deactivatePowerUp();
        }

        #endregion
    }
}