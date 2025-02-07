﻿using System;
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

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the game manager.
        /// </summary>
        /// <value>
        ///     The game manager.
        /// </value>
        public GameManager GameManager { get; }

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
        public int Score => int.TryParse(this.score.Text, out var currentScore) ? currentScore : 0;

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
        public int Level => int.TryParse(this.level.Text, out var currLevel) ? currLevel : 0;

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

            if (this.GameManager == null)
            {
                this.GameManager = new GameManager(this.canvas)
                {
                    Lives = this.Lives,
                    Score = this.Score,
                    TimeCountDown = this.Timer,
                    Level = this.Level
                };

                this.GameManager.InitializeGame();
            }

            this.GameManager.LivesUpdated += this.livesUpdated;
            this.GameManager.ScoreUpdated += this.scoreUpdated;
            this.GameManager.GameOver += this.OnGameOver;
            this.GameManager.TimeOut += this.decrementTime;
            this.GameManager.LevelUpdated += this.onLevelUpdated;
        }

        #endregion

        #region Methods

        private void OnGameOver(object sender, EventArgs e)
        {
            this.gameOverTextBlock.Visibility = Visibility.Visible;
            Frame.Navigate(typeof(HighScorePage));
        }

        private void onLevelUpdated(object sender, EventArgs e)
        {
            this.level.Text = ((GameManager)sender).Level.ToString();
        }

        private void decrementTime(object sender, EventArgs e)
        {
            this.timerTexBlock.Text = ((GameManager)sender).TimeCountDown.ToString();
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
                    this.GameManager.PlayerManager.MovePlayerLeft();
                    break;
                case VirtualKey.Right:
                    this.GameManager.PlayerManager.MovePlayerRight();
                    break;
                case VirtualKey.Up:
                    this.GameManager.PlayerManager.MovePlayerUp();
                    break;
                case VirtualKey.Down:
                    this.GameManager.PlayerManager.MovePlayerDown();
                    break;
            }
        }

        private void playAgainButtonClick(object sender, RoutedEventArgs e)
        {
            this.GameManager.ResetGame();
            this.gameOverTextBlock.Visibility = Visibility.Collapsed;
            this.onLevelUpdated(this.GameManager, EventArgs.Empty);
        }

        #endregion
    }
}