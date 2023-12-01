using System;
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
        private readonly Random random;

        #endregion

        #region Constructors


        /// <summary>Initializes a new instance of the <see cref="WaterCrossingManager" /> class.</summary>
        /// <param name="gameCanvas">The game canvas.</param>
        public WaterCrossingManager(Canvas gameCanvas)
        {
            this.gameCanvas = gameCanvas;
            this.waterCrossings = new List<WaterCrossing>();
            this.random = new Random();
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
            double yPosition = GameConstants.PlayableTopY;
            foreach (var direction in new[] { Direction.Right, Direction.Left, Direction.Right })
            {
                var crossing = new WaterCrossing(direction) { Y = yPosition };
                yPosition += GameConstants.LaneWidth;
                this.gameCanvas.Children.Add(crossing.Sprite);
                this.waterCrossings.Add(crossing);
            }
        }


        /// <summary>Adds the planks to water crossings.</summary>
        private void addPlanksToWaterCrossings()
        {
            const int numberOfPlanks = 5;
            const int minGap = 70;

            foreach (var waterCrossing in this.waterCrossings)
            {
                var lastPlankEndX = 0;
                for (var i = 0; i < numberOfPlanks; i++)
                {
                    var plank = new Plank { Y = waterCrossing.Y, X = lastPlankEndX + minGap};
                    waterCrossing.AddPlank(plank);
                    this.gameCanvas.Children.Add(plank.Sprite);

                    lastPlankEndX = (int)(plank.X + plank.Width);
                }
            }
        }

        #endregion
    }
}