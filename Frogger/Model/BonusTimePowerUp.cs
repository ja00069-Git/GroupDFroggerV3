using Frogger.View.Sprites;

namespace Frogger.Model
{
    /// <summary>
    ///     Defines the Bonus Time Power Up model
    /// </summary>
    /// <seealso cref="GameObject" />
    public class BonusTimePowerUp : GameObject
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BonusTimePowerUp" /> class.
        /// </summary>
        public BonusTimePowerUp()
        {
            Sprite = new PowerUpSprite();
        }

        #endregion
    }
}