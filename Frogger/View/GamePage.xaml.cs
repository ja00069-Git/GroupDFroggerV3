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

            int.TryParse(this.lives.Text, out var livesRemaining);
            int.TryParse(this.score.Text, out var currentScore);
            int.TryParse(this.timerTexBlock.Text, out var currentTimer);

            this.gameManager =
                new GameManager(livesRemaining, currentScore, currentTimer);

            this.gameManager.InitializeGame(this.canvas);
            this.gameManager.LivesUpdated += this.livesUpdated;
            this.gameManager.ScoreUpdated += this.scoreUpdated;
            this.gameManager.GameOver += this.OnGameOver;
            this.gameManager.TimeOut += this.decrementTime;
        }

        #endregion

        #region Methods
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

        #endregion
    }
}