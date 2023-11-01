using System.Collections.Generic;
using Windows.UI.Xaml;
using Frogger.Model.Frogger.Model;
using Frogger.View.Sprites;

namespace Frogger.Model
{
    /// <summary>
    ///     Defines the lane model of the game
    /// </summary>
    /// <seealso cref="GameObject" />
    public class Lane : GameObject
    {
        #region Properties

        /// <summary>
        ///     Gets the type of lane vehicle.
        /// </summary>
        /// <value>
        ///     The lane vehicle.
        /// </value>
        public VehicleType LaneVehicle { get; }

        /// <summary>
        ///     Gets the vehicles.
        /// </summary>
        /// <value>
        ///     The vehicles.
        /// </value>
        public IList<Vehicle> Vehicles { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Lane" /> class.
        /// </summary>
        /// <param name="laneVehicle">The lane vehicle.</param>
        public Lane(VehicleType laneVehicle)
        {
            this.LaneVehicle = laneVehicle;
            Sprite = new LaneSprite();
            this.Vehicles = new List<Vehicle>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds the vehicle to the lane.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        public void Add(Vehicle vehicle)
        {
            this.Vehicles.Add(vehicle);
            foreach (var aVehicle in this.Vehicles)
            {
                aVehicle.Sprite.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        ///     Moves the vehicle to either left or right by lane speed.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        /// <param name="laneSpeed">The lane speed.</param>
        /// <param name="laneIndex">Index of the lane.</param>
        public void MoveVehicle(Vehicle vehicle, double laneSpeed, int laneIndex)
        {
            for (var i = 0; i < this.Vehicles.Count; i++)
            {
                if ((laneIndex == 1) | (laneIndex == 4))
                {
                    vehicle.SetSpeed(laneSpeed, 0);
                    vehicle.MoveRight();
                }
                else
                {
                    vehicle.SetSpeed(laneSpeed, 0);
                    vehicle.MoveLeft();
                }
            }

            wrapVehicles(vehicle);
        }

        private static void wrapVehicles(Vehicle vehicle)
        {
            if (vehicle != null)
            {
                var newX = vehicle.X + vehicle.SpeedX;
                var appWidth = (double)Application.Current.Resources["AppWidth"];

                if (newX > appWidth)
                {
                    vehicle.X = -vehicle.Width;
                }
                else if (newX + vehicle.Width < 0)
                {
                    vehicle.X = appWidth;
                }
                else
                {
                    vehicle.X = newX;
                }
            }
        }

        #endregion
    }
}