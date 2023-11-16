using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
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

        private HighScore selectedHighScore;

        private readonly GamePage gamePage;

        private string playerName;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the name of the player.
        /// </summary>
        /// <value>
        ///     The name of the player.
        /// </value>
        public string PlayerName
        {
            get => this.playerName;
            set
            {
                if (this.setField(ref this.playerName, value))
                {
                    this.OnPropertyChanged();
                }
            }
        }

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

        /// <summary>
        ///     Gets or sets the selected high score.
        /// </summary>
        /// <value>
        ///     The selected high score.
        /// </value>
        public HighScore SelectedHighScore
        {
            get => this.selectedHighScore;
            set
            {
                this.selectedHighScore = value;
                this.OnPropertyChanged();
                this.RemoveCommand.OnCanExecuteChanged();
                this.EditNameCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the remove command.
        /// </summary>
        /// <value>
        ///     The remove command.
        /// </value>
        public RelayCommand RemoveCommand { get; set; }

        /// <summary>
        ///     Gets or sets the add command.
        /// </summary>
        /// <value>
        ///     The add command.
        /// </value>
        public RelayCommand AddCommand { get; set; }

        /// <summary>
        ///     Gets or sets the sort by score command.
        /// </summary>
        /// <value>
        ///     The sort by score command.
        /// </value>
        public RelayCommand SortByScoreCommand { get; set; }

        /// <summary>
        ///     Gets or sets the sort by name command.
        /// </summary>
        /// <value>
        ///     The sort by name command.
        /// </value>
        public RelayCommand SortByNameCommand { get; set; }

        /// <summary>
        ///     Gets or sets the sort by level command.
        /// </summary>
        /// <value>
        ///     The sort by level command.
        /// </value>
        public RelayCommand SortByLevelCommand { get; set; }

        /// <summary>
        ///     Gets or sets the edit name command.
        /// </summary>
        /// <value>
        ///     The edit name command.
        /// </value>
        public RelayCommand EditNameCommand { get; set; }

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

        #endregion

        #region Methods

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        private void loadCommand()
        {
            this.RemoveCommand = new RelayCommand(this.removeScore, this.canRemoveScore);
            this.AddCommand = new RelayCommand(this.addScore, this.canAddScore);
            this.SortByScoreCommand = new RelayCommand(this.sortByScore, this.canSort);
            this.SortByNameCommand = new RelayCommand(this.sortByName, this.canSort);
            this.SortByLevelCommand = new RelayCommand(this.sortByLevel, this.canSort);
            this.EditNameCommand = new RelayCommand(this.editName, this.canEditName);
        }

        private bool canAddScore(object obj)
        {
            return true;
        }

        private void addScore(object obj)
        {
            var highScore = new HighScore
            {
                PlayerName = this.PlayerName,
                Score = this.gamePage.GameManager.Score,
                LevelCompleted = this.gamePage.GameManager.Level
            };

            this.scoreBoard.Scores.Add(highScore);

            this.HighScores = this.scoreBoard.Scores.ToObservableCollection();
            this.SortByScoreCommand.OnCanExecuteChanged();
            this.SortByLevelCommand.OnCanExecuteChanged();
            this.SortByNameCommand.OnCanExecuteChanged();
            this.PlayerName = string.Empty;
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

        private void sortByScore(object obj)
        {
            this.HighScores = new ObservableCollection<HighScore>(this.HighScores.OrderByDescending(highScore => highScore.Score));
        }

        private void sortByName(object obj)
        {
            this.HighScores = new ObservableCollection<HighScore>(this.HighScores.OrderBy(highScore => highScore.PlayerName));
        }

        private void sortByLevel(object obj)
        {
            this.HighScores =
                new ObservableCollection<HighScore>(this.HighScores.OrderByDescending(highScore => highScore.LevelCompleted));
        }

        private bool canSort(object obj)
        {
            return this.HighScores.Any();
        }

        private bool canEditName(object obj)
        {
            return this.selectedHighScore != null;
        }

        private async void editName(object obj)
        {
            var dialog = new ContentDialog
            {
                Title = "Edit Player Name",
                Content = "Enter the new name:",
                PrimaryButtonText = "Save",
                SecondaryButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary
            };

            var textBox = new TextBox();
            dialog.Content = textBox;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                this.SelectedHighScore.PlayerName = textBox.Text;

                this.saveHighScores();

                this.HighScores = this.scoreBoard.Scores.ToObservableCollection();
            }
        }

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