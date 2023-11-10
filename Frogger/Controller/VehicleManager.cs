using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Frogger.Model;
using Frogger.Model.Frogger.Model;

namespace Frogger.Controller
{
    /// <summary>
    ///     Vehicle Manager
    /// </summary>
    public class VehicleManager
    {
        #region Data members

        private readonly LaneManager laneManager;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="VehicleManager" /> class.
        /// </summary>
        /// <param name="laneManager">The lane manager.</param>
        public VehicleManager(LaneManager laneManager)
        {
            this.laneManager = laneManager;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Places the vehicles in lane.
        /// </summary>
        /// <param name="gameCanvas">The game canvas.</param>
        /// <param name="lane">The lane.</param>
        /// <param name="laneVehicle">The lane vehicle.</param>
        /// <param name="numVehicles">The number vehicles.</param>
        /// <param name="laneIndex">Index of the lane.</param>
        public void PlaceVehiclesInLane(Canvas gameCanvas, Lane lane, VehicleType laneVehicle, int numVehicles,
            int laneIndex)
        {
            var laneWidth = (double)Application.Current.Resources["AppWidth"];
            var vehicleSpacing = laneWidth / numVehicles;
            for (var vehicleOnLine = 0; vehicleOnLine < numVehicles; vehicleOnLine++)
            {
                var vehicle = this.createVehicle(laneVehicle, laneIndex);
                lane.Add(vehicle);
                gameCanvas.Children.Add(vehicle.Sprite);
                vehiclePosition(vehicle, lane, vehicleOnLine, vehicleSpacing);
            }
        }

        private Vehicle createVehicle(VehicleType laneVehicle, int laneIndex)
        {
            var vehicle = laneVehicle == VehicleType.Car ? (Vehicle)new Car() : new Semi();
            determineIfVehicleTransform(vehicle, laneIndex);
            return vehicle;
        }

        private static void determineIfVehicleTransform(Vehicle vehicle, int laneIndex)
        {
            if (laneIndex == 2 || laneIndex == 5)
            {
                vehicle.Sprite.RenderTransformOrigin = new Point(0.5, 0.5);
                vehicle.Sprite.RenderTransform = new ScaleTransform { ScaleX = -1 };
            }
        }

        private static void vehiclePosition(Vehicle vehicle, Lane lane, int vehicleOnLine, double vehicleSpacing)
        {
            var laneWidth = (double)Application.Current.Resources["AppWidth"];
            var vehicleLeft = laneWidth - (vehicleOnLine + 1) * vehicleSpacing;
            vehicle.Y = Canvas.GetTop(lane.Sprite);
            vehicle.X = vehicleLeft;
        }

        /// <summary>
        ///     Moves the vehicles
        /// </summary>
        public void MoveVehicle()
        {
            for (var i = 0; i < this.laneManager.Lanes.Count; i++)
            {
                var lane = this.laneManager.Lanes[i];

                foreach (var vehicle in lane.Vehicles)
                {
                    lane.MoveVehicle(vehicle, LaneManager.LaneSpeeds[i], i);
                }
            }
        }

        #endregion
    }
}