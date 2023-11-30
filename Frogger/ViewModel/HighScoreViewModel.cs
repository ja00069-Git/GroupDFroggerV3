using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
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

        private string playerName;

        private ObservableCollection<HighScore> highScores;

        private readonly HighScore highScore;
        private HighScore selectedHighScore;
        private bool hasAddedScore;


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
        ///     Gets or sets the start screen command.
        /// </summary>
        /// <value>
        ///     The start screen command.
        /// </value>
        public RelayCommand StartScreenCommand { get; set; }

        /// <summary>
        ///     Gets or sets the play again command.
        /// </summary>
        /// <value>
        ///     The play again command.
        /// </value>
        public RelayCommand PlayAgainCommand { get; set; }

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

        /// <summary>
        ///     Gets or sets the clear board command.
        /// </summary>
        /// <value>
        ///     The clear board command.
        /// </value>
        public RelayCommand ClearBoardCommand { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HighScoreViewModel" /> class.
        /// </summary>
        public HighScoreViewModel()
        {
            this.scoreBoard = new HighScoreBoard();
            this.updateListView();
            this.loadCommand();
            this.highScore = new HighScore();

            GameManager.GameOverVm += this.onGameOver;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        private void onGameOver(object sender, GameOverVmEventArgs e)
        {
            this.highScore.Score = e.Score;
            this.highScore.LevelCompleted = e.Level;
        }

        private void loadCommand()
        {
            this.RemoveCommand = new RelayCommand(this.removeScore, this.canRemoveScore);
            this.AddCommand = new RelayCommand(this.addScore, this.canAddScore);
            this.SortByScoreCommand = new RelayCommand(this.sortByScore, this.canSort);
            this.SortByNameCommand = new RelayCommand(this.sortByName, this.canSort);
            this.SortByLevelCommand = new RelayCommand(this.sortByLevel, this.canSort);
            this.EditNameCommand = new RelayCommand(this.editName, this.canEditName);
            this.PlayAgainCommand = new RelayCommand(this.playAgain, this.canPlayAgain);
            this.StartScreenCommand = new RelayCommand(this.startScreen, this.canStartScreen);
            this.ClearBoardCommand = new RelayCommand(this.clearBoard, this.canClearBoard);
        }

        private bool canClearBoard(object obj)
        {
            return this.scoreBoard.Scores.Count > 0;
        }

        private void clearBoard(object obj)
        {
            this.scoreBoard.ClearScores();
            this.updateListView();
            this.updateCommandsStatusOnClear();
        }

        private void updateCommandsStatusOnClear()
        {
            this.SortByScoreCommand.OnCanExecuteChanged();
            this.SortByLevelCommand.OnCanExecuteChanged();
            this.SortByNameCommand.OnCanExecuteChanged();
            this.ClearBoardCommand.OnCanExecuteChanged();
            this.ClearBoardCommand.OnCanExecuteChanged();

        }

        private bool canStartScreen(object obj)
        {
            return true;
        }

        private bool canPlayAgain(object obj)
        {
            return true;
        }

        private void startScreen(object obj)
        {
            if (Window.Current.Content is Frame rootFrame)
            {
                rootFrame.Navigate(typeof(StartPage));
            }
        }

        private void playAgain(object obj)
        {
            if (Window.Current.Content is Frame rootFrame)
            {
                rootFrame.Navigate(typeof(GamePage));
                this.hasAddedScore = false;
            }
        }

        private bool canAddScore(object obj)
        {
            return !this.hasAddedScore && this.highScore.Score != 0 | this.highScore.LevelCompleted > 0;
        }

        private void addScore(object obj)
        {
            this.highScore.PlayerName = this.PlayerName;
            this.scoreBoard.AddScore(this.highScore);

            this.updateListView();
            this.PlayerName = string.Empty;
            this.hasAddedScore = true;
            this.updateCommandsStatusOnAdd();
        }

        private void updateCommandsStatusOnAdd()
        {
            this.SortByScoreCommand.OnCanExecuteChanged();
            this.SortByLevelCommand.OnCanExecuteChanged();
            this.SortByNameCommand.OnCanExecuteChanged();
            this.ClearBoardCommand.OnCanExecuteChanged();
            this.AddCommand.OnCanExecuteChanged();
        }

        private void updateListView()
        {
            this.HighScores = this.scoreBoard.Scores.ToObservableCollection();
        }

        private bool canRemoveScore(object obj)
        {
            return this.selectedHighScore != null;
        }

        private void removeScore(object obj)
        {
            this.scoreBoard.Remove(this.selectedHighScore);
            this.updateListView();
        }

        private void sortByScore(object obj)
        {
            this.HighScores =
                new ObservableCollection<HighScore>(this.HighScores.OrderByDescending(score => score.Score));
        }

        private void sortByName(object obj)
        {
            this.HighScores =
                new ObservableCollection<HighScore>(this.HighScores.OrderBy(score => score.PlayerName));
        }

        private void sortByLevel(object obj)
        {
            this.HighScores =
                new ObservableCollection<HighScore>(
                    this.HighScores.OrderByDescending(score => score.LevelCompleted));
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
            textBox.Text = this.selectedHighScore.PlayerName;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                var name = textBox.Text;

                this.scoreBoard.EditName(this.selectedHighScore, name);

                this.updateListView();
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