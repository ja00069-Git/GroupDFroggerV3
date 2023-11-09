using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Frogger.Model;

namespace Frogger.Controller
{
    /// <summary>
    ///     Lane manager
    /// </summary>
    public class LaneManager
    {
        #region Data members

        private DispatcherTimer vehicleAdditionTimer;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the vehicle manager.
        /// </summary>
        /// <value>
        ///     The vehicle manager.
        /// </value>
        public VehicleManager VehicleManager { get; }

        /// <summary>
        ///     Gets the lanes.
        /// </summary>
        /// <value>
        ///     The lanes.
        /// </value>
        public IList<Lane> Lanes { get; } = new List<Lane>();

        /// <summary>
        ///     lane speeds
        /// </summary>
        public static double[] LaneSpeeds { get; set; } = { 0.1, 0.2, 0.3, 0.4, 0.5 };

        /// <summary>
        ///     Gets or sets the vehicles per lane.
        /// </summary>
        /// <value>
        ///     The vehicles per lane.
        /// </value>
        public static int[] VehiclesPerLane { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LaneManager" /> class.
        /// </summary>
        public LaneManager()
        {
            this.VehicleManager = new VehicleManager(this);
            this.setupVehicleAdditionTimer();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the and place lanes.
        /// </summary>
        /// <param name="gameCanvas">The game canvas.</param>
        public void CreateAndPlaceLanes(Canvas gameCanvas)
        {
            var lowShoulderY = (double)Application.Current.Resources["LowShoulderYLocation"];

            const int maxLanes = 5;
            for (var laneIndex = 1; laneIndex <= maxLanes; laneIndex++)
            {
                var laneHeight = 50;
                this.determineLaneProperties(laneIndex, out var laneVehicle, out var numVehicles);
                var lane = this.createLane(gameCanvas, laneVehicle);
                lanePosition(lane, laneIndex, lowShoulderY, laneHeight);
                this.VehicleManager.PlaceVehiclesInLane(gameCanvas, lane, laneVehicle, numVehicles, laneIndex);
                this.Lanes.Add(lane);

                foreach (var unused in lane.Vehicles)
                {
                    lane.Vehicles[0].Sprite.Visibility = Visibility.Visible;
                }
            }
        }

        private void setupVehicleAdditionTimer()
        {
            this.vehicleAdditionTimer = new DispatcherTimer();
            this.vehicleAdditionTimer.Tick += this.vehicleAdditionTimerOnTick;
            this.vehicleAdditionTimer.Interval = new TimeSpan(0, 0, 4);
            this.vehicleAdditionTimer.Start();
        }

        private void vehicleAdditionTimerOnTick(object sender, object e)
        {
            foreach (var lane in this.Lanes)
            {
                foreach (var vehicle in lane.Vehicles)
                {
                    if (vehicle.Sprite.Visibility != Visibility.Visible &&
                        (vehicle.X > (double)Application.Current.Resources["AppWidth"] || vehicle.X < 0))
                    {
                        vehicle.Sprite.Visibility = Visibility.Visible;
                        break;
                    }
                }
            }
        }

        private void determineLaneProperties(int laneIndex, out VehicleType laneVehicle, out int numVehicles)
        {
            switch (laneIndex)
            {
                case 1:
                    laneVehicle = VehicleType.Car;
                    numVehicles = VehiclesPerLane[0];
                    break;
                case 2:
                    laneVehicle = VehicleType.Semi;
                    numVehicles = VehiclesPerLane[1];
                    break;
                case 3:
                    laneVehicle = VehicleType.Car;
                    numVehicles = VehiclesPerLane[2];
                    break;
                case 4:
                    laneVehicle = VehicleType.Semi;
                    numVehicles = VehiclesPerLane[3];
                    break;
                default:
                    laneVehicle = VehicleType.Car;
                    numVehicles = VehiclesPerLane[4];
                    break;
            }
        }

        private static void lanePosition(Lane lane, int laneIndex, double lowShoulderY, double laneHeight)
        {
            var laneTop = lowShoulderY - laneHeight * laneIndex;
            lane.Y = laneTop;
        }

        private Lane createLane(Canvas gameCanvas, VehicleType laneVehicle)
        {
            var lane = new Lane(laneVehicle);
            gameCanvas.Children.Add(lane.Sprite);
            return lane;
        }

        #endregion
    }
}