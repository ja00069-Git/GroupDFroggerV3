using Frogger.View.Sprites;

namespace Frogger.Model
{
    /// <summary>
    ///     Defines the plank model
    /// </summary>
    /// <seealso cref="GameObject" />
    public class Plank : GameObject
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Plank" /> class.
        /// </summary>
        public Plank()
        {
            Sprite = new PlankSprite();
        }

        #endregion
    }
}