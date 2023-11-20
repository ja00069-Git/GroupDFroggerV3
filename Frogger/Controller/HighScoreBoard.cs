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
        
        private IList<HighScore> scores;

        /// <summary>
        /// Gets the scores.
        /// </summary>
        /// <value>
        /// The scores.
        /// </value>
        public IList<HighScore> Scores => this.scores;

        /// <summary>Gets the <see cref="HighScore" /> at the specified index.</summary>
        /// <param name="index">The index.</param>
        /// <value>The <see cref="HighScore" />.</value>
        /// <returns>
        ///   The HighScore for the specified index
        /// </returns>
        public HighScore this[int index] => this.scores[index];

        #endregion        
        /// <summary>
        /// Initializes a new instance of the <see cref="HighScoreBoard"/> class.
        /// </summary>
        public HighScoreBoard()
        {
            this.scores = new List<HighScore>();
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