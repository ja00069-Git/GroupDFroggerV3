using Windows.UI.Xaml;
using Frogger.View.Sprites;

namespace Frogger.Model
{
    /// <summary>
    ///     Defines the water crossing model
    /// </summary>
    /// <seealso cref="GameObject" />
    public class WaterCrossing : GameObject
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaterCrossing" /> class.
        /// </summary>
        public WaterCrossing()
        {
            Sprite = new WaterCrossingSprite();
            Y = (double)Application.Current.Resources["HighShoulderYLocation"] + 50;
        }

        #endregion
    }
}