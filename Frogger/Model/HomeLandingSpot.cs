using Windows.UI.Xaml;
using Frogger.View.Sprites;

namespace Frogger.Model
{
    /// <summary>
    ///     Home Landing spot
    /// </summary>
    /// <seealso cref="GameObject" />
    public class HomeLandingSpot : GameObject
    {
        #region Data members

        /// <summary>
        ///     The pod occupied
        /// </summary>
        public bool PodOccupied;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HomeLandingSpot" /> class.
        /// </summary>
        public HomeLandingSpot()
        {
            Sprite = new HomeLandingSpotSprite();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Occupies the pod with frog.
        /// </summary>
        public void OccupyPodWithFrog()
        {
            this.PodOccupied = true;
            var frog = (HomeSprite)Sprite.FindName("homeSprite");
            if (frog != null)
            {
                frog.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        ///     Releases the pod.
        /// </summary>
        public void UnoccupySpot()
        {
            this.PodOccupied = false;
            var frog = (HomeSprite)Sprite.FindName("homeSprite");
            if (frog != null)
            {
                frog.Visibility = Visibility.Collapsed;
            }
        }

        #endregion
    }
}