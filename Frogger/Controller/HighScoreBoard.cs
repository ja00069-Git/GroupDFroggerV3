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

        /// <summary>
        /// Gets the <see cref="HighScore"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="HighScore"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public HighScore this[int index] => this.Scores[index];

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