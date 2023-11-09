using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public GameManager()
        {
            this.setupGameTimer();
            this.setupLifeTimer();
            this.PlayerManager = new PlayerManager();
            this.laneManager = new LaneManager();
            this.soundEffects = new SoundEffects();
        }

        #endregion

        #region Methods

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

        private void setupGameTimer()
        {
            this.timer = new DispatcherTimer();
            this.timer.Tick += this.timerOnTick;
            this.timer.Interval = new TimeSpan(0, 0, 0, 0, 15);
            this.timer.Start();
        }

        private void setupLifeTimer()
        {
            this.lifeDispatcherTimer = new DispatcherTimer();
            this.lifeDispatcherTimer.Tick += this.lifeDispatcherTimerOnTick;
            this.lifeDispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            this.lifeDispatcherTimer.Start();
        }

        private void lifeDispatcherTimerOnTick(object sender, object e)
        {
            this.TimeCountDown--;
            this.onTimeOutChanged();
        }

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

            this.configureLevelParameters();

            this.createHomeLandingSPots(gameCanvas);
            this.laneManager.CreateAndPlaceLanes(gameCanvas);
            this.PlayerManager.CreateAndPlacePlayer(gameCanvas);
        }

        private void configureLevelParameters()
        {

            switch (this.Level)
            {
                case 1:
                    LaneManager.LaneSpeeds = new[] { 0.1, 0.2, 0.3, 0.4, 0.5 };
                    LaneManager.VehiclesPerLane = new[] { 2, 1, 3, 2, 4 };
                    break;

                case 2:
                    LaneManager.LaneSpeeds = new[] { 0.2, 0.3, 0.4, 0.5, 0.6 };
                    LaneManager.VehiclesPerLane = new[] { 3, 2, 4, 3, 5 };
                    Debug.WriteLine(LaneManager.VehiclesPerLane[1]);
                    break;

                case 3:
                    LaneManager.LaneSpeeds = new[] { 0.3, 0.4, 0.5, 0.6, 0.7 };
                    LaneManager.VehiclesPerLane = new[] { 4, 3, 5, 4, 6 };
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

        private async void timerOnTick(object sender, object e)
        {
            if (this.Lives <= 0 || this.Level >= 3)
            {
                this.GameOver?.Invoke(this, EventArgs.Empty);
                this.timer.Stop();
                this.lifeDispatcherTimer.Stop();
            }
            else if (this.allHomeLandingSpotsOccupied() && this.Level < 4)
            {
                foreach (var spot in this.homeLandingSpots)
                {
                    spot.UnoccupySpot();
                }

                this.Level++;
                await this.soundEffects.LevelUpSound();
                this.configureLevelParameters();
                this.LevelUpdated?.Invoke(this, EventArgs.Empty);
            }

            this.moveVehicle();
            this.updateScore();
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
            this.PlayerManager.SetPlayerToCenterOfBottomShoulder();
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
            this.Score += increaseScoreBy;
            this.onScoreUpdated();
            this.PlayerManager.SetPlayerToCenterOfBottomShoulder();
            this.TimeCountDown = 20;
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
        #endregion
    }
}