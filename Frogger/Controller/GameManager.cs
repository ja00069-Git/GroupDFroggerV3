using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Media.Playback;
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
        public int TimeCountDown { get; private set; }

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
        public int Lives { get; private set; }

        /// <summary>
        ///     Gets the current score of the player
        /// </summary>
        /// <value>
        ///     The current score of the player.
        /// </value>
        public int Score { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameManager" /> class.
        /// </summary>
        /// <param name="lives"></param>
        /// <param name="score"></param>
        /// <param name="timeCountDown"></param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     backgroundHeight &lt;= 0
        ///     or
        ///     backgroundWidth &lt;= 0
        /// </exception>
        public GameManager(int lives, int score, int timeCountDown)
        {
            this.Lives = lives;
            this.Score = score;
            this.TimeCountDown = timeCountDown;
            this.setupGameTimer();
            this.setupLifeTimer();
            this.PlayerManager = new PlayerManager();
            this.laneManager = new LaneManager();
            this.soundEffects = new SoundEffects();
        }

        #endregion

        #region Methods

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

            this.createHomeLandingSPots(gameCanvas);
            this.laneManager.CreateAndPlaceLanes(gameCanvas);
            this.PlayerManager.CreateAndPlacePlayer(gameCanvas);
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

        private void timerOnTick(object sender, object e)
        {
            this.moveVehicle();
            this.updateScore();
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

        private async void onScoreUpdated()
        {
            this.ScoreUpdated?.Invoke(this, EventArgs.Empty);

            if (this.homeLandingSpots.All(spot => spot.PodOccupied))
            {
                await this.soundEffects.LevelUpSound();
                this.GameOver?.Invoke(this, EventArgs.Empty);
                this.timer.Stop();
                this.lifeDispatcherTimer.Stop();
            }
        }

        private async void onLivesUpdated()
        {
            this.LivesUpdated?.Invoke(this, EventArgs.Empty);

            if (this.Lives == 0)
            {
                await this.soundEffects.GameOverSound();
                this.GameOver?.Invoke(this, EventArgs.Empty);
                this.timer.Stop();
                this.lifeDispatcherTimer.Stop();
            }
        }

        private async void onTimeOutChanged()
        {
            this.TimeOut?.Invoke(this, EventArgs.Empty);
            if (this.TimeCountDown == 0)
            {
                await this.soundEffects.DyingSound();
                this.TimeCountDown = 20;
                this.Lives--;
                this.onLivesUpdated();
                this.lifeDispatcherTimer.Start();
            }
            this.onLivesUpdated();
        }

        #endregion
    }
}