using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Frogger.Model;

namespace Frogger.Controller
{
    /// <summary>
    /// Player manager
    /// </summary>
    public class PlayerManager
    {
        #region Data members          
        /// <summary>
        /// Gets the player.
        /// </summary>
        /// <value>
        /// The player.
        /// </value>
        public Frog Player { get; } = new Frog();

        private readonly double backgroundWidth = (double)Application.Current.Resources["AppWidth"];

        #endregion

        #region Methods        
        /// <summary>
        /// Creates the and place player.
        /// </summary>
        /// <param name="gameCanvas">The game canvas.</param>
        public void CreateAndPlacePlayer(Canvas gameCanvas)
        {
            gameCanvas.Children.Remove(this.Player.Sprite);

            gameCanvas.Children.Add(this.Player.Sprite);

            this.SetPlayerToCenterOfBottomShoulder();
        }
        /// <summary>
        /// Sets the player to center of bottom shoulder.
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