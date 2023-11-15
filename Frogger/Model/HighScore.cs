namespace Frogger.Model
{
    /// <summary>
    ///     Single high score entry
    /// </summary>
    public class HighScore
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the name of the player.
        /// </summary>
        /// <value>
        ///     The name of the player.
        /// </value>
        public string PlayerName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the score.
        /// </summary>
        /// <value>
        ///     The score.
        /// </value>
        public int Score { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the level completed.
        /// </summary>
        /// <value>
        ///     The level completed.
        /// </value>
        public int LevelCompleted { get; set; } = 0;

        #endregion

        #region Methods

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{this.PlayerName} {this.Score} {this.LevelCompleted}";
        }

        #endregion
    }
}