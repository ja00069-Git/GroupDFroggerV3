using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Frogger.Model;

namespace Frogger.Controller
{
    /// <summary>
    ///     Manages the home landing spots
    /// </summary>
    public class LandingSpotManager
    {
        #region Properties

        /// <summary>
        ///     Gets the home landing spots.
        /// </summary>
        /// <value>
        ///     The home landing spots.
        /// </value>
        public IList<HomeLandingSpot> HomeLandingSpots { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LandingSpotManager" /> class.
        /// </summary>
        /// <param name="gameCanvas">The game canvas.</param>
        public LandingSpotManager(Canvas gameCanvas)
        {
            this.HomeLandingSpots = new List<HomeLandingSpot>();
            this.CreateHomeLandingSpots(gameCanvas);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the home landing spots.
        /// </summary>
        /// <param name="gameCanvas">The game canvas.</param>
        public void CreateHomeLandingSpots(Canvas gameCanvas)
        {
            var numPods = 5;
            const int podWidth = 60;
            var highShoulderWidth = (double)Application.Current.Resources["AppWidth"];
            var availableSpace = highShoulderWidth - numPods * podWidth;
            var podSpacing = availableSpace / (numPods - 1);

            for (var i = 0; i < numPods; i++)
            {
                var pod = new HomeLandingSpot();
                this.HomeLandingSpots.Add(pod);
                gameCanvas.Children.Add(pod.Sprite);
                pod.X = i * (podWidth + podSpacing);
                pod.Y = (double)Application.Current.Resources["HighShoulderYLocation"];
            }
        }

        /// <summary>
        ///     Alls the home landing spots occupied.
        /// </summary>
        /// <returns>True if all the landing spots are occupied</returns>
        public bool AllHomeLandingSpotsOccupied()
        {
            foreach (var spot in this.HomeLandingSpots)
            {
                if (!spot.PodOccupied)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Uns the occupy home landing spots.
        /// </summary>
        public void UnOccupyHomeLandingSpots()
        {
            foreach (var spot in this.HomeLandingSpots)
            {
                spot.UnoccupySpot();
            }
        }

        #endregion
    }
}