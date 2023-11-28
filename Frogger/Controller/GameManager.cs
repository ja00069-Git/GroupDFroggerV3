using System;
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

        private const int MaxLevel = 3;
        private const int LandHomeIn = 20;

        private DispatcherTimer gameTimer;
        private DispatcherTimer lifeDispatcherTimer;

        private readonly LaneManager laneManager;
        private readonly WaterCrossingManager waterCrossingManager;
        private readonly BonusTimeManager bonusTimeManager;

        private readonly LandingSpotManager landingSpotManager;

        private readonly PowerUp powerUp = new PowerUp();
        private readonly SoundEffects soundEffects;

        private bool canAddBonus = true;
        private int homeLandingCount;
        private bool isGameOver;

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
        ///     Gets the lives of the player from the player manager.
        /// </summary>
        /// <value>
        ///     The lives of the player.
        /// </value>
        public int Lives
        {
            get => this.PlayerManager.Lives;
            set => this.PlayerManager.Lives = value;
        }

        /// <summary>
        ///     Gets the current score of the player from the player manager
        /// </summary>
        /// <value>
        ///     The current score of the player.
        /// </value>
        public int Score
        {
            get => this.PlayerManager.Score;
            set => this.PlayerManager.Score = value;
        }

        /// <summary>
        ///     Gets or sets the level from the player manager.
        /// </summary>
        /// <value>
        ///     The level.
        /// </value>
        public int Level
        {
            get => this.PlayerManager.Level;
            set => this.PlayerManager.Level = value;
        }

        private Canvas GameCanvas { get; }

        private bool PlayerHasNoLives => this.Lives <= 0;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="GameManager" /> class.</summary>
        /// <param name="gameCanvas">The game canvas.</param>
        public GameManager(Canvas gameCanvas)
        {
            this.GameCanvas = gameCanvas ?? throw new ArgumentNullException(nameof(gameCanvas));

            this.setupGameTimer();
            this.setupLifeTimer();

            this.PlayerManager = new PlayerManager(gameCanvas);
            this.laneManager = new LaneManager();
            this.waterCrossingManager = new WaterCrossingManager(gameCanvas);
            this.soundEffects = new SoundEffects();
            this.bonusTimeManager = new BonusTimeManager(gameCanvas);
            this.landingSpotManager = new LandingSpotManager(gameCanvas);

            this.PlayerManager.AnimationOver += this.startGameAfterDeathAnimation;
            this.PlayerManager.AnimationStarted += this.stopGameBeforeDeathAnimation;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes the game working with appropriate classes to place frog
        ///     and vehicle on game screen.
        ///     Precondition: gameCanvas != null
        ///     Postcondition: Game is initialized and ready for play.
        /// </summary>
        /// <exception cref="ArgumentNullException">gameCanvas</exception>
        public void InitializeGame()
        {
            this.configureLevelParameters();
            this.landingSpotManager.CreateHomeLandingSpots(this.GameCanvas);
            this.bonusTimeManager.PlaceBonusTimeSprite();
        }

        /// <summary>
        ///     Resets the game.
        /// </summary>
        public void ResetGame()
        {
            this.gameTimer.Stop();
            this.lifeDispatcherTimer.Stop();

            this.laneManager.ClearLanesAndVehicles(this.GameCanvas);

            this.landingSpotManager.UnOccupyHomeLandingSpots();

            this.resetGameStats();

            this.gameTimer.Start();
            this.lifeDispatcherTimer.Start();

            this.configureLevelParameters();
        }

        private void startGameAfterDeathAnimation(object sender, EventArgs e)
        {
            this.startAllTimers();
        }

        private void stopGameBeforeDeathAnimation(object sender, EventArgs e)
        {
            this.stopAllTimers();
        }

        private async void gameOver()
        {
            await this.soundEffects.GameOverSound();

            this.isGameOver = true;
            this.stopAllTimers();

            var args = new GameOverVmEventArgs
            {
                Score = this.Score,
                Level = this.Level
            };

            this.GameOver?.Invoke(this, EventArgs.Empty);
            GameOverVm?.Invoke(this, args);
        }

        private async void timerOnTick()
        {
            if (this.PlayerHasNoLives)
            {
                this.gameOver();
            }
            else if (this.landingSpotManager.AllHomeLandingSpotsOccupied() && this.Level < 4)
            {
                this.landingSpotManager.UnOccupyHomeLandingSpots();

                this.Level++;
                await this.soundEffects.LevelUpSound();
                this.deactivatePowerUp();
                this.homeLandingCount = 0;
                this.configureLevelParameters();
                this.LevelUpdated?.Invoke(this, EventArgs.Empty);

                if (this.Level <= MaxLevel)
                {
                    this.configureLevelParameters();
                    this.LevelUpdated?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    this.gameOver();
                }
            }

            if (!this.isGameOver)
            {
                this.checkCollisionWithMushroom();
                this.moveVehicle();
                this.waterCrossingManager.MovePlanks();
                this.playerCanLandInPlank();
                this.updateScore();
            }
        }

        private void playerCanLandInPlank()
        {
            var canLand = this.waterCrossingManager.canPlayerLand(this.PlayerManager.Player).Item1;
            var plankLandedOn = this.waterCrossingManager.canPlayerLand(this.PlayerManager.Player).Item2;

            switch (canLand)
            {
                case true:
                    this.PlayerManager.Player.X = plankLandedOn.X;
                    break;
                case false:
                    if (this.PlayerManager.Player.Y <= this.waterCrossingManager.WaterCrossing.Y + 50)
                    {

                    }
                    break;
            }
        }

        private void configureLevelParameters()
        {
            switch (this.Level)
            {
                case 1:
                    LaneManager.LaneSpeeds = new[] { 0.1, 0.2, 0.3, 0.4, 0.5 };
                    LaneManager.VehiclesPerLane = new[] { 2, 1, 3, 2, 4 };

                    this.laneManager.CreateAndPlaceLanes(this.GameCanvas);
                    this.PlayerManager.CreateAndPlacePlayer();
                    this.canAddBonus = true;
                    this.bonusTimeManager.EnableSprite();

                    break;

                case 2:
                    LaneManager.LaneSpeeds = new[] { 0.3, 0.4, 0.5, 0.6, 0.7 };
                    LaneManager.VehiclesPerLane = new[] { 3, 2, 4, 3, 5 };

                    this.laneManager.CreateAndPlaceLanes(this.GameCanvas);
                    this.PlayerManager.CreateAndPlacePlayer();
                    this.canAddBonus = true;
                    this.bonusTimeManager.EnableSprite();

                    break;

                case MaxLevel:
                    LaneManager.LaneSpeeds = new[] { 0.1, 0.2, 0.3, 0.4, 0.5 };
                    LaneManager.VehiclesPerLane = new[] { 4, 3, 5, 4, 6 };

                    this.laneManager.CreateAndPlaceLanes(this.GameCanvas);
                    this.PlayerManager.CreateAndPlacePlayer();
                    this.canAddBonus = true;
                    this.bonusTimeManager.EnableSprite();

                    break;
            }
        }

        private void startAllTimers()
        {
            if (!this.isGameOver)
            {
                this.gameTimer.Start();
                this.lifeDispatcherTimer.Start();
            }
        }

        private void stopAllTimers()
        {
            this.gameTimer.Stop();
            this.lifeDispatcherTimer.Stop();
        }

        private void checkCollisionWithMushroom()
        {
            if (this.bonusTimeManager.CheckPlayerCollision(this.PlayerManager.Player) & this.canAddBonus)
            {
                this.gameTimer.Stop();
                this.canAddBonus = false;
                this.bonusTimeManager.DisableSprite();
                this.TimeCountDown += this.bonusTimeManager.BonusInSec;
                this.gameTimer.Start();
            }
        }

        private void setupGameTimer()
        {
            this.gameTimer = new DispatcherTimer();
            this.gameTimer.Tick += (sender, e) => this.timerOnTick();
            this.gameTimer.Interval = TimeSpan.FromMilliseconds(15);
            this.gameTimer.Start();
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

        private void moveVehicle()
        {
            this.laneManager.VehicleManager.MoveVehicle();

            foreach (var lane in this.laneManager.Lanes)
            {
                this.collisionManagement(lane);
            }
        }

        private void collisionManagement(Lane lane)
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
            this.PlayerManager.HandleDeath();
            await this.soundEffects.DyingSound();
            this.Lives--;
            this.onLivesUpdated();
            this.TimeCountDown = LandHomeIn;
        }

        private void updateScore()
        {
            var landedHome = false;
            var dintLandHome = false;

            foreach (var spot in this.landingSpotManager.HomeLandingSpots)
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
            this.TimeCountDown = LandHomeIn;
            this.Lives--;
            this.onLivesUpdated();
            this.PlayerManager.SetPlayerToCenterOfBottomShoulder();
        }

        private async void handleLandingHome()
        {
            var collisionDetected = false;
            HomeLandingSpot occupiedSpot = null;

            foreach (var spot in this.landingSpotManager.HomeLandingSpots)
            {
                if (spot.CheckCollision(this.PlayerManager.Player) && !spot.PodOccupied)
                {
                    collisionDetected = true;
                    occupiedSpot = spot;
                    break;
                }
            }

            if (collisionDetected)
            {
                await this.soundEffects.LandingHomeSounds();

                occupiedSpot.OccupyPodWithFrog();

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
                this.TimeCountDown = LandHomeIn;
            }
            else
            {
                this.handleDintLandingHome();
            }
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
                this.TimeCountDown = LandHomeIn;
                this.Lives--;
                this.onLivesUpdated();
                await this.soundEffects.DyingSound();
                this.lifeDispatcherTimer.Start();
            }

            this.onLivesUpdated();
        }

        private void resetGameStats()
        {
            this.Lives = 4;
            this.onLivesUpdated();
            this.Score = 0;
            this.onScoreUpdated();
            this.Level = 1;
            this.TimeCountDown = LandHomeIn;
            this.onTimeOutChanged();
            this.deactivatePowerUp();
            this.isGameOver = false;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        ///     Occurs when [level updated].
        /// </summary>
        public event EventHandler LevelUpdated;

        /// <summary>
        ///     Event Occurs when [lives updated].
        /// </summary>
        public event EventHandler LivesUpdated;

        /// <summary>
        ///     Event Occurs when [score updated].
        /// </summary>
        public event EventHandler ScoreUpdated;

        /// <summary>
        ///     Occurs when [time out].
        /// </summary>
        public event EventHandler TimeOut;

        /// <summary>
        ///     Event Occurs when [game over].
        /// </summary>
        public event EventHandler GameOver;

        /// <summary>Occurs when [game over vm].</summary>
        public static event EventHandler<GameOverVmEventArgs> GameOverVm;

        #endregion
    }

    /// <summary>
    ///     Defines the arguments for the View Model Game Over Event
    /// </summary>
    public class GameOverVmEventArgs : EventArgs
    {
        #region Properties

        /// <summary>Gets or sets the score.</summary>
        /// <value>The score.</value>
        public int Score { get; set; }

        /// <summary>Gets or sets the level.</summary>
        /// <value>The level.</value>
        public int Level { get; set; }

        #endregion
    }
}