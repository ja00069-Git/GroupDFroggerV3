﻿using Frogger.View.Sprites;

namespace Frogger.Model
{
    /// <summary>
    ///     Defines the frog model
    /// </summary>
    /// <seealso cref="GameObject" />
    public class Frog : GameObject
    {
        #region Data members

        private const int SpeedXDirection = 50;
        private const int SpeedYDirection = 50;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Frog" /> class.
        /// </summary>
        public Frog()
        {
            Sprite = new FrogSprite();
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion
    }
}