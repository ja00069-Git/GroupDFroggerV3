using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Frogger.Model;
using Windows.Storage;

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

            this.loadHighScores();
        }


        /// <summary>Adds the score.</summary>
        /// <param name="highScore">The high score.</param>
        public void AddScore(HighScore highScore)
        {
            this.scores.Add(highScore);
            this.saveHighScores();
        }

        /// <summary>
        /// Removes the specified high score.
        /// </summary>
        /// <param name="highScore">The high score.</param>
        /// <returns></returns>
        public bool Remove(HighScore highScore)
        {   
            var wasRemoved = this.Scores.Remove(highScore);
            this.saveHighScores();
            return wasRemoved;
        }

        private void saveHighScores()
        {
            var localFolder = ApplicationData.Current.LocalFolder;

            var json = JsonSerializer.Serialize(this.Scores);
            var filePath = Path.Combine(localFolder.Path, "highScores.json");

            File.WriteAllText(filePath, json);
        }

        private void loadHighScores()
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var filePath = Path.Combine(localFolder.Path, "highScores.json");

                var json = File.ReadAllText(filePath);
                this.scores = JsonSerializer.Deserialize<ObservableCollection<HighScore>>(json);
            }
            catch (FileNotFoundException)
            {
                this.scores = new ObservableCollection<HighScore>();
            }
        }


        /// <summary>Edits the name.</summary>
        /// <param name="selectedHighScore">The selected high score.</param>
        /// <param name="name">The name.</param>
        public void EditName(HighScore selectedHighScore, string name)
        {
            var index = this.Scores.IndexOf(selectedHighScore);
            this.scores[index].PlayerName = name;

            this.saveHighScores();
        }
    }
}