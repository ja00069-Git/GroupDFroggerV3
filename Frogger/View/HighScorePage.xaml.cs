

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Frogger.View
{
    /// <summary>
    ///     High Score view
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.Page" />
    public sealed partial class HighScorePage : Page
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HighScorePage" /> class.
        /// </summary>
        public HighScorePage()
        {
            this.InitializeComponent();
        }

        #endregion

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamePage));
        }

        private void Start_Screen(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage));
        }
    }
}