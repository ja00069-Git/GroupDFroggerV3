using System;
using System.Collections.Generic;
using System.Numerics;
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
        private IList<BaseSprite> deathFrameSprites;
        private IList<BaseSprite> movementAnimationSprites;
        private DispatcherTimer deathAnimationTimer;
        private DispatcherTimer movementAnimationTimer;
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

        private SpriteDirection Direction { get; set; } = SpriteDirection.Up;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="PlayerManager" /> class.</summary>
        /// <param name="gameCanvas">The game canvas.</param>
        public PlayerManager(Canvas gameCanvas)
        {
            this.GameCanvas = gameCanvas;

            this.setupDeathSprites();
            this.setupAnimationSprites();

            this.setupDeathTimer();
            this.setupMovementTimer();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        ///     Occurs when [death animation started].
        /// </summary>
        public event EventHandler AnimationStarted;

        /// <summary>
        ///     Occurs when [death animation is over].
        /// </summary>
        public event EventHandler AnimationOver;

        #endregion

        #region Methods

        /// <summary>
        ///     Handles the death.
        /// </summary>
        public void HandleDeath()
        {
            this.Lives--;

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
            this.turnSprite(SpriteDirection.Up, this.Player.Sprite);
            this.Player.X = this.backgroundWidth / 2 - this.Player.Width / 2;
            this.Player.Y = (double)Application.Current.Resources["LowShoulderYLocation"];
        }

        private void deathTimerOnTick(object sender, object e)
        {
            this.updateAnimationFrame(this.deathFrameSprites,
                this.deathAnimationTimer, this.finishDeathAnimation);
        }

        private void movementAnimationTimerOnTick(object sender, object e)
        {
            this.updateAnimationFrame(this.movementAnimationSprites,
                this.movementAnimationTimer, this.finishMovementAnimation);
        }

        private void updateAnimationFrame(IList<BaseSprite> frames, DispatcherTimer timer, Action finishAction)
        {
            this.Player.Sprite.Opacity = 0;
            
            if (this.currentFrame < frames.Count)
            {
                if (this.currentFrame > 0)
                {
                    frames[this.currentFrame - 1].Opacity = 0;
                }

                var currentSprite = frames[this.currentFrame];
                currentSprite.RenderAt(this.Player.X, this.Player.Y);
                this.turnSprite(this.Direction, currentSprite);
                currentSprite.Opacity = 1;

                this.currentFrame++;
            }
            else
            {
                finishAction();
            }
        }

        private void startMovementAnimation()
        {
            this.currentFrame = 0;
            this.movementAnimationTimer.Start();
        }

        private void updateFrameOpacity(int frameIndex)
        {
            var currentFrame = this.deathFrameSprites[frameIndex];

            if (frameIndex > 0)
            {
                this.deathFrameSprites[frameIndex - 1].Opacity = 0;
            }

            this.turnSprite(this.Direction, currentFrame);

            currentFrame.RenderAt(this.Player.X, this.Player.Y);
            currentFrame.Opacity = 1;
        }

        private void finishDeathAnimation()
        {
            foreach (var sprite in this.deathFrameSprites)
            {
                sprite.Opacity = 0; // Reset all death animation frames
            }

            this.deathAnimationTimer.Stop();
            this.currentFrame = 0;
            this.Player.Sprite.Opacity = 1; // Make the player sprite visible again
            this.SetPlayerToCenterOfBottomShoulder();
            this.AnimationOver?.Invoke(this, EventArgs.Empty);
        }

        private void finishMovementAnimation()
        {
            foreach (var sprite in this.movementAnimationSprites)
            {
                sprite.Opacity = 0; // Reset all movement animation frames
            }

            this.movementAnimationTimer.Stop();
            this.currentFrame = 0;
            this.Player.Sprite.Opacity = 1; // Make the player sprite visible again
        }

        private void addSpritesToCanvas(IList<BaseSprite> sprites)
        {
            foreach (var sprite in sprites)
            {
                sprite.Opacity = 0;
                this.GameCanvas.Children.Add(sprite);
            }
        }

        private void setupDeathSprites()
        {
            this.deathFrameSprites = new List<BaseSprite>
            {
                new DeathSpriteFrame1(),
                new DeathSpriteFrame2(),
                new DeathSpriteFrame3(),
                new DeathSpriteFrame4()
            };

            this.addSpritesToCanvas(this.deathFrameSprites);
        }

        private void setupAnimationSprites()
        {
            this.movementAnimationSprites = new List<BaseSprite>
            {
                new FrogAnimationSprite1(),
                new FrogAnimationSprite2()
            };

            this.addSpritesToCanvas(this.movementAnimationSprites);
        }

        private void setupDeathTimer()
        {
            this.deathAnimationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            this.deathAnimationTimer.Tick += this.deathTimerOnTick;
        }

        private void setupMovementTimer()
        {
            this.movementAnimationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            this.movementAnimationTimer.Tick += this.movementAnimationTimerOnTick;
        }

        #endregion

        #region Move Player

        /// <summary>
        ///     Moves the player to the left.
        ///     Precondition: none
        ///     Postcondition: player.X = player.X@prev - player.Width
        /// </summary>
        public void MovePlayerLeft()
        {
            if (this.Player.X > 0)
            {
                this.turnSprite(SpriteDirection.Left, this.Player.Sprite);
                this.Player.MoveLeft();
                this.startMovementAnimation();
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
                this.turnSprite(SpriteDirection.Right, this.Player.Sprite);
                this.Player.MoveRight();
                this.startMovementAnimation();
            }
        }

        /// <summary>
        ///     Moves the player up.
        ///     Precondition: none
        ///     Post condition: player.Y = player.Y@prev - player.Height
        /// </summary>
        public void MovePlayerUp()
        {
            if (this.Player.Y > (double)Application.Current.Resources["HighShoulderYLocation"])
            {
                this.turnSprite(SpriteDirection.Up, this.Player.Sprite);
                this.Player.MoveUp();
                this.startMovementAnimation();
            }
        }

        /// <summary>
        ///     Moves the player down.
        ///     Precondition: none
        ///     Post condition: player.Y = player.Y@prev + player.Height
        /// </summary>
        public void MovePlayerDown()
        {
            if (this.Player.Y < (double)Application.Current.Resources["LowShoulderYLocation"])
            {
                this.turnSprite(SpriteDirection.Down, this.Player.Sprite);
                this.Player.MoveDown();
                this.startMovementAnimation();
            }
        }

        #endregion

        #region Turn Sprites

        private void setSpriteCenterPoint(BaseSprite sprite)
        {
            var height = (float)sprite.Height / 2;
            var width = (float)sprite.Width / 2;

            var vector = new Vector3(height, width, 1000);

            sprite.CenterPoint = vector;
        }

        private void turnSprite(SpriteDirection direction, BaseSprite sprite)
        {
            this.setSpriteCenterPoint(sprite);
            this.Direction = direction;
            sprite.Rotation = (int)direction;
        }

        #endregion
    }

    /// <summary>
    ///     <br />
    /// </summary>
    public enum SpriteDirection
    {
        /// <summary>Up</summary>
        Up = 360,

        /// <summary>Down</summary>
        Down = 180,

        /// <summary>Left</summary>
        Left = -90,

        /// <summary>Right</summary>
        Right = 90
    }
}