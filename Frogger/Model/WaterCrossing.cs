using System.Collections.Generic;
using Windows.UI.Xaml;
using Frogger.Util;
using Frogger.View.Sprites;

namespace Frogger.Model
{
    /// <summary>
    ///     Defines the water crossing model
    /// </summary>
    /// <seealso cref="GameObject" />
    public class WaterCrossing : GameObject
    {
        #region Properties


        /// <summary>Gets the planks.</summary>
        /// <value>The planks.</value>
        public List<Plank> Planks { get; }


        /// <summary>Gets the water direction.</summary>
        /// <value>The water direction.</value>
        public Direction WaterDirection { get; }

        #endregion

        #region Constructors


        /// <summary>Initializes a new instance of the <see cref="WaterCrossing" /> class.</summary>
        /// <param name="direction">The direction.</param>
        public WaterCrossing(Direction direction)
        {
            Sprite = new WaterCrossingSprite();
            this.WaterDirection = direction;
            this.Planks = new List<Plank>();
        }

        #endregion

        #region Methods


        /// <summary>Adds the plank.</summary>
        /// <param name="plank">The plank.</param>
        public void AddPlank(Plank plank)
        {
            plank.SetSpeed(GameConstants.PlankBaseSpeed, GameConstants.PlankBaseSpeed);
            this.Planks.Add(plank);
        }


        /// <summary>Moves the planks.</summary>
        public void MovePlanks()
        {
            var screenWidth = (double)Application.Current.Resources["AppWidth"];

            foreach (var plank in this.Planks)
            {
                if (this.WaterDirection == Direction.Right)
                {
                    plank.MoveRight();
                    if (plank.X > screenWidth)
                    {
                        plank.X = 0 - plank.Width;
                    }
                }
                else
                {
                    plank.MoveLeft();
                    if (plank.X < 0 - plank.Width)
                    {
                        plank.X = screenWidth;
                    }
                }
            }
        }

        #endregion
    }


    /// <summary>
    ///   Possible Water Crossing directions
    /// </summary>
    public enum Direction
    {

        /// <summary>The right</summary>
        Right,

        /// <summary>The left</summary>
        Left
    }
}