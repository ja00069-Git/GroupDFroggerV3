using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Frogger.View
{
    /// <summary>
    ///     The game start page
    /// </summary>
    public sealed partial class StartPage
    {
        #region Data members

        private readonly double applicationHeight = (double)Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double)Application.Current.Resources["AppWidth"];

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="StartPage" /> class.</summary>
        public StartPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size
                { Width = this.applicationWidth, Height = this.applicationHeight };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView()
                .SetPreferredMinSize(new Size(this.applicationWidth, this.applicationHeight));
        }

        #endregion

        #region Methods

        private void startGameButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamePage));
        }

        private void scoreBoardButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HighScorePage));
        }

        #endregion
    }
}