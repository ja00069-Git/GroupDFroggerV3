using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Frogger.Controller;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Frogger.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage
    {
        #region Data members

        private readonly double applicationHeight = (double)Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double)Application.Current.Resources["AppWidth"];
        private readonly GameManager gameManager;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the lives.
        /// </summary>
        /// <value>
        ///     The lives.
        /// </value>
        private int Lives => int.TryParse(this.lives.Text, out var livesRemaining) ? livesRemaining : 0;

        /// <summary>
        ///     Gets or sets the score.
        /// </summary>
        /// <value>
        ///     The score.
        /// </value>
        private int Score => int.TryParse(this.score.Text, out var currentScore) ? currentScore : 0;

        /// <summary>
        ///     Gets or sets the timer.
        /// </summary>
        /// <value>
        ///     The timer.
        /// </value>
        private int Timer => int.TryParse(this.timerTexBlock.Text, out var currentTimer) ? currentTimer : 0;

        /// <summary>
        ///     Gets or sets the level.
        /// </summary>
        /// <value>
        ///     The level.
        /// </value>
        private int Level => int.TryParse(this.level.Text, out var currLevel) ? currLevel : 0;

        #endregion

        #region Constructors

        /// <summary>
        ///     The "main" page for the Frogger game.
        /// </summary>
        public GamePage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size
                { Width = this.applicationWidth, Height = this.applicationHeight };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView()
                .SetPreferredMinSize(new Size(this.applicationWidth, this.applicationHeight));

            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;

            this.gameManager = new GameManager(this.canvas)
            {
                Lives = this.Lives,
                Score = this.Score,
                TimeCountDown = this.Timer,
                Level = this.Level
            };

            this.gameManager.InitializeGame(this.canvas);
            this.gameManager.LivesUpdated += this.livesUpdated;
            this.gameManager.ScoreUpdated += this.scoreUpdated;
            this.gameManager.GameOver += this.OnGameOver;
            this.gameManager.TimeOut += this.decrementTime;
            this.gameManager.LevelUpdated += this.onLevelUpdated;
        }

        #endregion

        #region Methods

        private void onLevelUpdated(object sender, EventArgs e)
        {
            this.level.Text = ((GameManager)sender).Level.ToString();
        }

        private void decrementTime(object sender, EventArgs e)
        {
            this.timerTexBlock.Text = ((GameManager)sender).TimeCountDown.ToString();
        }

        private void OnGameOver(object sender, EventArgs e)
        {
            this.gameOverTextBlock.Visibility = Visibility.Visible;
        }

        private void scoreUpdated(object sender, EventArgs e)
        {
            this.score.Text = ((GameManager)sender).Score.ToString();
        }

        private void livesUpdated(object sender, EventArgs e)
        {
            this.lives.Text = ((GameManager)sender).Lives.ToString();
        }

        private void coreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Left:
                    this.gameManager.PlayerManager.MovePlayerLeft();
                    break;
                case VirtualKey.Right:
                    this.gameManager.PlayerManager.MovePlayerRight();
                    break;
                case VirtualKey.Up:
                    this.gameManager.PlayerManager.MovePlayerUp();
                    break;
                case VirtualKey.Down:
                    this.gameManager.PlayerManager.MovePlayerDown();
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.gameManager.ResetGame(this.canvas);
            this.gameOverTextBlock.Visibility = Visibility.Collapsed;
            this.onLevelUpdated(this.gameManager, EventArgs.Empty);
        }

        #endregion
    }
}