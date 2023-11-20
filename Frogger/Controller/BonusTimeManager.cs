using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Frogger.Model;

namespace Frogger.Controller
{

    /// <summary>
    ///   Manages the Bonus Time Power Up Object
    /// </summary>
    public class BonusTimeManager
    {
        #region Data Fields

        private DispatcherTimer repositionBonusTimeSpriteTimer;

        #endregion

        #region Properties
        
        /// <summary>Gets the bonus time.</summary>
        /// <value>The bonus time.</value>
        public BonusTimePowerUp BonusTime { get; private set; } = new BonusTimePowerUp();

        /// <summary>Gets the bonus in sec.</summary>
        /// <value>The bonus in sec.</value>
        public int BonusInSec { get; private set; } = 5;

        #endregion

        /// <summary>Initializes a new instance of the <see cref="BonusTimeManager" /> class.</summary>
        /// <param name="gameCanvas">The game canvas.</param>
        public BonusTimeManager(Canvas gameCanvas)
        {
            gameCanvas.Children.Add(this.BonusTime.Sprite);
            this.BonusTime.Sprite.IsEnabled = false;
            this.setupTimer();
        }

        #region Methods
        
        /// <summary>Places the bonus time sprite.</summary>
        public void PlaceBonusTimeSprite()
        {
            this.BonusTime.Sprite.IsEnabled = true;
            this.BonusTime.X = this.randomX();
            this.BonusTime.Y = this.randomY();
        }


        /// <summary>Disables the sprite.</summary>
        public void DisableSprite()
        {
            this.repositionBonusTimeSpriteTimer.Stop();
            this.BonusTime.Sprite.IsEnabled = false;
            this.BonusTime.Sprite.Visibility = Visibility.Collapsed;
        }


        /// <summary>Enables the sprite.</summary>
        public void EnableSprite()
        {
            this.repositionBonusTimeSpriteTimer.Start();
            this.BonusTime.Sprite.IsEnabled = true;
            this.BonusTime.Sprite.Visibility = Visibility.Visible;
        }

        /// <summary>Checks the player collision.</summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///   True if a collision with the passed in object is detected
        ///   False otherwise
        /// </returns>
        public bool CheckPlayerCollision(GameObject obj)
        {
            return this.BonusTime.CheckCollision(obj);
        }

        private void setupTimer()
        {
            this.repositionBonusTimeSpriteTimer = new DispatcherTimer();
            this.repositionBonusTimeSpriteTimer.Tick += (sender, e) => this.timerOnTick();
            this.repositionBonusTimeSpriteTimer.Interval = TimeSpan.FromSeconds(5);
            this.repositionBonusTimeSpriteTimer.Start();
        }

        private void timerOnTick()
        {
            this.BonusTime.X = this.randomX();
            this.BonusTime.Y = this.randomY();
        }

        private int randomX()
        {
            var random = new Random();
            var appWidth = Convert.ToInt32((double)Application.Current.Resources["AppWidth"]);
            return random.Next(1, appWidth);
        }

        private int randomY()
        {
            var random = new Random();
            var highShoulder = Convert.ToInt32((double)Application.Current.Resources["HighShoulderYLocation"]);
            var lowShoulder = Convert.ToInt32((double)(Application.Current.Resources["LowShoulderYLocation"]) - this.BonusTime.Sprite.Height);
            return random.Next(highShoulder, lowShoulder);
        }
        #endregion

    }
}
