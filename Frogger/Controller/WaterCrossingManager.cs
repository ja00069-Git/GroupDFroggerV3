using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Frogger.Model;
using Frogger.Util;

namespace Frogger.Controller
{
    /// <summary>
    ///     Water Crossing Manager
    /// </summary>
    public class WaterCrossingManager
    {
        #region Data members

        private readonly Canvas gameCanvas;
        private readonly List<WaterCrossing> waterCrossings;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="WaterCrossingManager" /> class.</summary>
        /// <param name="gameCanvas">The game canvas.</param>
        public WaterCrossingManager(Canvas gameCanvas)
        {
            this.gameCanvas = gameCanvas;
            this.waterCrossings = new List<WaterCrossing>();
            this.initializeWaterCrossings();
            this.addPlanksToWaterCrossings();
        }

        #endregion

        #region Methods

        /// <summary>Moves the planks.</summary>
        public void MovePlanks()
        {
            foreach (var waterCrossing in this.waterCrossings)
            {
                waterCrossing.MovePlanks();
            }
        }

        /// <summary>Initializes the water crossings.</summary>
        private void initializeWaterCrossings()
        {
            double yPosition = GameConstants.WaterCrossingTopY;
            foreach (var direction in new[] { Direction.Right, Direction.Left, Direction.Right })
            {
                var crossing = new WaterCrossing(direction) { Y = yPosition };
                yPosition += GameConstants.LaneHeight;
                this.gameCanvas.Children.Add(crossing.Sprite);
                this.waterCrossings.Add(crossing);
            }
        }

        /// <summary>Adds the planks to water crossings.</summary>
        private void addPlanksToWaterCrossings()
        {
            const int numberOfPlanks = 4;
            const int minGap = 70;

            foreach (var waterCrossing in this.waterCrossings)
            {
                var lastPlankEndX = 0;
                for (var i = 0; i < numberOfPlanks; i++)
                {
                    var plank = new Plank { Y = waterCrossing.Y, X = lastPlankEndX + minGap };
                    waterCrossing.AddPlank(plank);
                    this.gameCanvas.Children.Add(plank.Sprite);

                    lastPlankEndX = (int)(plank.X + plank.Width);
                }
            }
        }

        /// <summary>Determines whether this instance [can player land] the specified player.</summary>
        /// <param name="player">The player.</param>
        /// <returns>
        ///     <c>true</c> if this instance [can player land] the specified player; otherwise, <c>false</c>.
        /// </returns>
        public (bool, Plank) CanPlayerLand(GameObject player)
        {
            foreach (var waterCrossing in this.waterCrossings)
            {
                foreach (var plank in waterCrossing.Planks)
                {
                    if (player.CheckCollision(plank))
                    {
                        return (true, plank);
                    }
                }
            }

            // If no collision with any plank, the player will land in water
            return (false, null);
        }

        #endregion
    }
}