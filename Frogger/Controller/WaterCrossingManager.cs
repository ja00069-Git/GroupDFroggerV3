using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Frogger.Model;

namespace Frogger.Controller
{
    /// <summary>
    ///     Water Crossing Manager
    /// </summary>
    public class WaterCrossingManager
    {
        #region Data members

        private const int PlankGap = 150;
        private const int InitialPlankX = 30;
        private readonly IList<Plank> planks;
        private readonly Canvas gameCanvas;
        private int nextPlankX = InitialPlankX;

        #endregion

        #region Properties

        public WaterCrossing WaterCrossing { get; }

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="WaterCrossingManager" /> class.</summary>
        /// <param name="gameCanvas">The game canvas.</param>
        public WaterCrossingManager(Canvas gameCanvas)
        {
            this.WaterCrossing = new WaterCrossing();
            this.gameCanvas = gameCanvas;
            this.gameCanvas.Children.Add(this.WaterCrossing.Sprite);

            this.planks = new List<Plank>
            {
                new Plank(),
                new Plank()
            };

            this.placePlanks();
        }

        #endregion

        #region Methods

        private void placePlanks()
        {
            foreach (var plank in this.planks)
            {
                this.gameCanvas.Children.Add(plank.Sprite);
                plank.Y = this.WaterCrossing.Y;
                plank.SetSpeed(1, 0);
                this.positionPlanks(plank);
            }
        }

        private void positionPlanks(Plank plank)
        {
            plank.X = this.nextPlankX;
            this.nextPlankX += PlankGap;
        }

        /// <summary>Moves the planks to the right.</summary>
        public void MovePlanks()
        {
            foreach (var plank in this.planks)
            {
                plank.MoveRight();
                if (plank.X > (double)Application.Current.Resources["AppWidth"])
                {
                    plank.X = 0 - plank.Width;
                }
            }
        }

        public (bool, Plank) canPlayerLand(GameObject player)
        {
            (bool, Plank) canLand = (false, null);

            foreach (var plank in this.planks)
            {
                if (plank.CheckCollision(player))
                {
                    canLand = (true, plank);
                    return canLand;
                }
            }

            return canLand;
        }

        #endregion
    }
}