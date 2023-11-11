using Frogger.Model.Frogger.Model;
using Frogger.View.Sprites;

namespace Frogger.Model
{
    namespace Frogger.Model
    {
        /// <summary>
        ///     Defines the vehicle model of the game
        /// </summary>
        public class Vehicle : GameObject
        {
            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Vehicle" /> class.
            /// </summary>
            /// <param name="initialSpeedX">The initial speed x.</param>
            /// <param name="initialSpeedY">The initial speed y.</param>
            protected Vehicle(int initialSpeedX, int initialSpeedY)
            {
                SetSpeed(initialSpeedX, initialSpeedY);
            }

            #endregion
        }
    }

    /// <summary>
    ///     Defines a semi ( a type of vehicle in the game )
    /// </summary>
    public class Semi : Vehicle
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Semi" /> class.
        /// </summary>
        public Semi() : base(0, 0)
        {
            Sprite = new SemiSprite();
        }

        #endregion
    }

    /// <summary>
    ///     Defines a car ( a type of vehicle in the game )
    /// </summary>
    public class Car : Vehicle
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Car" /> class.
        /// </summary>
        public Car() : base(0, 0)
        {
            Sprite = new CarSprite();
        }

        #endregion
    }


    /// <summary>
    ///   Defines a tank ( a type of vehicle in the game )
    /// </summary>
    public class Tank : Vehicle
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Tank" /> class.
        /// </summary>
        public Tank() : base(0, 0)
        {
            Sprite = new TankSprite();
        }
    }
}