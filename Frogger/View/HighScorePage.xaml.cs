// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Frogger.View
{
    /// <summary>
    ///     High Score view
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.Page" />
    public sealed partial class HighScorePage
    {
        #region Data members

        private readonly double applicationHeight = (double)Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double)Application.Current.Resources["AppWidth"];

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HighScorePage" /> class.
        /// </summary>
        public HighScorePage()
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamePage));
        }

        private void Start_Screen(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage));
        }

        #endregion
    }
}