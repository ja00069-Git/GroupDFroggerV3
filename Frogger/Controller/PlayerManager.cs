using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Frogger.Model;
using Frogger.View.Sprites;
using Frogger.View.Sprites.DeathAnimationSprites;

namespace Frogger.Controller
{
    /// <summary>
    ///     Player manager
    /// </summary>
    public class PlayerManager
    {
        #region Data members

        private readonly double backgroundWidth = (double)Application.Current.Resources["AppWidth"];
        private readonly IList<BaseSprite> deathFrameSprites;
        private DispatcherTimer deathAnimationTimer;
        private int currentFrame;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        public Frog Player { get; } = new Frog();

        /// <summary>
        ///     Gets the game canvas.
        /// </summary>
        /// <value>
        ///     The game canvas.
        /// </value>
        private Canvas GameCanvas { get; }

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="PlayerManager" /> class.</summary>
        /// <param name="gameCanvas">The game canvas.</param>
        public PlayerManager(Canvas gameCanvas)
        {
            this.GameCanvas = gameCanvas;

            this.deathFrameSprites = new List<BaseSprite>
            {
                new DeathSpriteFrame1(),
                new DeathSpriteFrame2(),
                new DeathSpriteFrame3(),
                new DeathSpriteFrame4()
            };

            foreach (var deathFrameSprite in this.deathFrameSprites)
            {
                deathFrameSprite.Opacity = 0;
                this.GameCanvas.Children.Add(deathFrameSprite);
            }

            this.setupDeathTimer();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Occurs when [death animation started].
        /// </summary>
        public event EventHandler AnimationStarted;

        /// <summary>
        ///     Occurs when [death animation is over].
        /// </summary>
        public event EventHandler AnimationOver;

        private void setupDeathTimer()
        {
            this.deathAnimationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            this.deathAnimationTimer.Tick += this.deathTimerOnTick;
        }

        private void deathTimerOnTick(object sender, object e)
        {
            switch (this.currentFrame)
            {
                case 0:
                {
                    this.Player.Sprite.Opacity = 0;
                    this.deathFrameSprites[this.currentFrame].Opacity = 1;
                    this.deathFrameSprites[this.currentFrame].RenderAt(this.Player.X, this.Player.Y);
                    this.currentFrame++;
                    break;
                }
                case 1:
                {
                    this.turnOffLastFrameAndTurnOnCurrentFrame();
                    break;
                }
                case 2:
                {
                    this.turnOffLastFrameAndTurnOnCurrentFrame();
                    break;
                }
                case 3:
                {
                    this.turnOffLastFrameAndTurnOnCurrentFrame();
                    break;
                }
                case 4:
                {
                    this.deathFrameSprites[this.currentFrame - 1].Opacity = 0; //Turns off last frame
                    this.deathAnimationTimer.Stop();
                    this.currentFrame = 0;
                    this.Player.Sprite.Opacity = 1;
                    this.SetPlayerToCenterOfBottomShoulder();
                    this.AnimationOver?.Invoke(this, EventArgs.Empty);
                    break;
                }
            }
        }

        private void turnOffLastFrameAndTurnOnCurrentFrame()
        {
            this.deathFrameSprites[this.currentFrame - 1].Opacity = 0; //Turns off last frame
            this.deathFrameSprites[this.currentFrame].RenderAt(this.Player.X, this.Player.Y);
            this.deathFrameSprites[this.currentFrame].Opacity = 1;
            this.currentFrame++;
        }

        /// <summary>Handles the death.</summary>
        public void HandleDeath()
        {
            this.deathAnimationTimer.Start();
            this.AnimationStarted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Creates the and place player.
        /// </summary>
        public void CreateAndPlacePlayer()
        {
            this.GameCanvas.Children.Remove(this.Player.Sprite);

            this.GameCanvas.Children.Add(this.Player.Sprite);

            this.SetPlayerToCenterOfBottomShoulder();
        }

        /// <summary>
        ///     Sets the player to center of bottom shoulder.
        /// </summary>
        public void SetPlayerToCenterOfBottomShoulder()
        {
            this.Player.X = this.backgroundWidth / 2 - this.Player.Width / 2;
            this.Player.Y = (double)Application.Current.Resources["LowShoulderYLocation"];
        }

        /// <summary>
        ///     Moves the player to the left.
        ///     Precondition: none
        ///     Postcondition: player.X = player.X@prev - player.Width
        /// </summary>
        public void MovePlayerLeft()
        {
            if (this.Player.X > 0)
            {
                this.Player.MoveLeft();
            }
        }

        /// <summary>
        ///     Moves the player to the right.
        ///     Precondition: none
        ///     Postcondition: player.X = player.X@prev + player.Width
        /// </summary>
        public void MovePlayerRight()
        {
            if (this.Player.X + this.Player.Width < this.backgroundWidth)
            {
                this.Player.MoveRight();
            }
        }

        /// <summary>
        ///     Moves the player up.
        ///     Precondition: none
        ///     Postcondition: player.Y = player.Y@prev - player.Height
        /// </summary>
        public void MovePlayerUp()
        {
            if (this.Player.Y > (double)Application.Current.Resources["HighShoulderYLocation"])
            {
                //this.Player.changeSprite(1);
                this.Player.MoveUp();
            }
        }

        /// <summary>
        ///     Moves the player down.
        ///     Precondition: none
        ///     Postcondition: player.Y = player.Y@prev + player.Height
        /// </summary>
        public void MovePlayerDown()
        {
            if (this.Player.Y < (double)Application.Current.Resources["LowShoulderYLocation"])
            {
                this.Player.MoveDown();
            }
        }

        #endregion
    }
}