using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Minesweeper;
using System.Diagnostics;

namespace ViewMinesweeper
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// //Eventhandler (closing): Subscriber ruft diese Methode auf.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterHighscoreNameWindowClosed(object sender, EventArgs e)
        {
            HighscoreWindow highscoreWindow = new HighscoreWindow(rankType);
            highscoreWindow.Owner = this;
            this.Activate();
            highscoreWindow.Show();

        }

        private void LeftMouseButton_ButtonClick(object sender, RoutedEventArgs e)
        {
            CoordinateGrid coordinateGrid = (CoordinateGrid)(sender as Button).Parent;

            if (!gameRuns && gameResetted)
                StartGame(coordinateGrid.column, coordinateGrid.row);

            if (!afterGame)
            {
                FieldActivator(coordinateGrid.column, coordinateGrid.row);
                CheckGameStatus();
            }
        }

        private void RightMouseButton_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (gameRuns)
            {            
                if ((sender as Button).Content == null)
                {
                    PaintWarningOnButton(sender as Button);
                    if (mineCounter.Text == "0")
                    {
                        ActivateUnflaggedFields();
                        CheckGameStatus();
                    }
                }
                else
                RubWarningFromButton(sender as Button);
            }
        }

        private void ViewBox_Doubleclick(object sender, MouseButtonEventArgs e)
        {
            if (gameRuns && !afterGame)
            {
                if (e.ClickCount == 2)
                {
                    int column = ((CoordinateGrid)(sender as Viewbox).Parent).column;
                    int row = ((CoordinateGrid)(sender as Viewbox).Parent).row;
                    if (EnumerateSurroundingFlags(column, row) == _minesweeperMap.GetSurroundingMines(column, row))
                    {
                        ActivateSurroundingFields(column, row);
                        CheckGameStatus();
                    }
                }
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (seconds < 999)
            {
                seconds++;
                timer.Text = string.Format($"{seconds}");
                if (seconds == 999)
                    dispatcherTimer.Stop();
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            EndInititialization();
            InitializePlayfield();
        }

        private void Highscores_Click(object sender, RoutedEventArgs e)
        {
            HighscoreWindow highscoreWindow = new HighscoreWindow(0);
            highscoreWindow.Owner = this;
            highscoreWindow.Show();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
        }

        private void QuitGame_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DifficultyEasy_Click(object sender, RoutedEventArgs e)
        {
            textBoxColumns.Text = "9";
            textBoxRows.Text = "9";
            textBoxMines.Text = "10";
            InitializePlayfield();
        }

        private void DifficultyMedium_Click(object sender, RoutedEventArgs e)
        {
            textBoxColumns.Text = "16";
            textBoxRows.Text = "16";
            textBoxMines.Text = "40";
            InitializePlayfield();
        }

        private void DifficultyHard_Click(object sender, RoutedEventArgs e)
        {
            textBoxColumns.Text = "30";
            textBoxRows.Text = "16";
            textBoxMines.Text = "99";
            InitializePlayfield();
        }

        private void Manual_Click(object sender, RoutedEventArgs e)
        {

        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this, $"Minesweeper\n{version}\n{timeOfDevelopment} {developer}.\nNo rights reserved...", "About Minesweeper");
        }
    }
}