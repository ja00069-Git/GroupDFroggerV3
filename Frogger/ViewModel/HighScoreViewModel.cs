using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Windows.Storage;
using Frogger.Command;
using Frogger.Controller;
using Frogger.Extensions;
using Frogger.Model;
using Frogger.View;

namespace Frogger.ViewModel
{
    /// <summary>
    ///     High score view model
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public sealed class HighScoreViewModel : INotifyPropertyChanged
    {
        #region Data members

        private readonly HighScoreBoard scoreBoard;

        private ObservableCollection<HighScore> highScores;

        /// <summary>
        /// Player's name
        /// </summary>
        public string PlayerName { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the high scores.
        /// </summary>
        public ObservableCollection<HighScore> HighScores
        {
            get => this.highScores;
            set
            {
                this.highScores = value;
                this.OnPropertyChanged();
            }
        }

        private HighScore selectedHighScore;
        private readonly GamePage gamePage;

        /// <summary>
        /// Gets or sets the selected high score.
        /// </summary>
        /// <value>
        /// The selected high score.
        /// </value>
        public HighScore SelectedHighScore
        {
            get => this.selectedHighScore;
            set
            {
                this.selectedHighScore = value;
                this.OnPropertyChanged();
                this.RemoveCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the remove command.
        /// </summary>
        /// <value>
        /// The remove command.
        /// </value>
        public RelayCommand RemoveCommand { get; set; }

        /// <summary>
        /// Gets or sets the add command.
        /// </summary>
        /// <value>
        /// The add command.
        /// </value>
        public RelayCommand AddCommand { get; set; }


        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HighScoreViewModel" /> class.
        /// </summary>
        public HighScoreViewModel()
        {
            this.gamePage = new GamePage();
            this.scoreBoard = new HighScoreBoard();
            this.HighScores = this.scoreBoard.Scores.ToObservableCollection();
            this.loadCommand();
            this.loadHighScores();
        }

        private void loadCommand()
        {
            this.RemoveCommand = new RelayCommand(this.removeScore, this.canRemoveScore);
            this.AddCommand = new RelayCommand(this.addScore, this.canAddScore);
        }

        private bool canAddScore(object obj)
        {
            return true;
        }

        private void addScore(object obj)
        {
            HighScore highScore = new HighScore
            {
                PlayerName = this.PlayerName,
                Score = this.gamePage.GameManager.Score,
                LevelCompleted = this.gamePage.GameManager.Level
            };

            this.scoreBoard.Scores.Add(highScore);

            this.HighScores = this.scoreBoard.Scores.ToObservableCollection();

            this.saveHighScores();
        }

        private bool canRemoveScore(object obj)
        {
            return this.selectedHighScore != null;
        }

        private void removeScore(object obj)
        {
            this.scoreBoard.Remove(this.selectedHighScore);
            this.HighScores = this.scoreBoard.Scores.ToObservableCollection();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;
      

        /// <summary>
        ///     Saves the high scores.
        /// </summary>
        private void saveHighScores()
        {
            var localFolder = ApplicationData.Current.LocalFolder;

            var json = JsonSerializer.Serialize(this.HighScores);
            var filePath = Path.Combine(localFolder.Path, "highScores.json");

            File.WriteAllText(filePath, json);
        }

        private void loadHighScores()
        {
            try
            {
                var json = File.ReadAllText("highScores.json");
                this.HighScores = JsonSerializer.Deserialize<ObservableCollection<HighScore>>(json);
            }
            catch (FileNotFoundException)
            {
                this.HighScores = new ObservableCollection<HighScore>();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool setField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}