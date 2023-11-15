using System.Collections.Generic;
using Frogger.Model;

namespace Frogger.Controller
{
    /// <summary>
    ///     High score manager
    /// </summary>
    public class HighScoreBoard
    {
        #region Data members        
        /// <summary>
        /// Gets the scores.
        /// </summary>
        /// <value>
        /// The scores.
        /// </value>
        public IList<HighScore> Scores { get; }

        #endregion        
        /// <summary>
        /// Initializes a new instance of the <see cref="HighScoreBoard"/> class.
        /// </summary>
        public HighScoreBoard()
        {
            this.Scores = new List<HighScore>();
        }

        /// <summary>
        /// Removes the specified high score.
        /// </summary>
        /// <param name="highScore">The high score.</param>
        /// <returns></returns>
        public bool Remove(HighScore highScore)
        {
            return this.Scores.Remove(highScore);
        }
    }
}