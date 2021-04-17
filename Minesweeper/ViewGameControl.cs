using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Minesweeper;


namespace ViewMinesweeper
{
    public partial class MainWindow : Window
    {
        private void CheckGameStatus()
        {            
            SynchronizeButtonVisibility();
            if (mineActivated)
            {
                EndInititialization();
                MessageBox.Show(this, "BOOOOM. You're dead.");
            }                
            else if (_minesweeperMap.IsGameWon())
            {
                EndInititialization();
                HighscoreHandler();
            }                
        }

        private void HighscoreHandler()
        {
            int timeNeeded = int.Parse(timer.Text);
            rankType = -1;
            Highscore highscore;

            if (_minesweeperMap.IsEasyMode())
            {
                highscore = HighscoreWindow.LoadEasyHighscoreFile();
                if (highscore.IsNewBetter(timeNeeded))
                {
                    rankType = 0;
                }
            }
            else if (_minesweeperMap.IsMediumMode())
            {
                highscore = HighscoreWindow.LoadMediumHighscoreFile();
                if (highscore.IsNewBetter(timeNeeded))
                {
                    rankType = 1;
                }
            }
            else if (_minesweeperMap.IsHardMode())
            {
                highscore = HighscoreWindow.LoadHardHighscoreFile();
                if (highscore.IsNewBetter(timeNeeded))
                {
                    rankType = 2;
                }
            }

            if (rankType == -1)
                MessageBox.Show(this, "Congratulations. You have found all mines.");
            else
            {
                EnterHighscoreNameWindow enterHighscoreName = new EnterHighscoreNameWindow();
                enterHighscoreName.ClosingHandler += new EventHandler(EnterHighscoreNameWindowClosed); //Eventhandler (closing) wird subscribed
                enterHighscoreName.Ranktype = rankType;
                enterHighscoreName.TimeNeeded = timeNeeded;
                enterHighscoreName.Owner = this;
                enterHighscoreName.Show();
            }
        }

        /// <summary>
        /// This allocates the memory for the MinesweeperModel and synchronizes it with the View.
        /// </summary>
        /// <param name="startColumn">The start field contains no mine.</param>
        /// <param name="startRow">The start field contains no mine.</param>
        private void StartGame(int startColumn, int startRow)
        {
            _minesweeperMap = new MinesweeperMap(usersColumns, usersRows, usersMines, startColumn, startRow);
            SynchronizeModelWithView();
            StartInititialization();
        }

        /// <summary>
        /// This method activates the field of the View, which means it implicates an action in the Model.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private void FieldActivator(int column, int row)
        {
            mineActivated = !_minesweeperMap.ActivateField(column, row);
            if (mineActivated)
            {
                Label label = GetLabelOfPlayfieldGrid(column, row);
                label.Background = Brushes.Red;                
            }
        }

        /// <summary>
        /// This method activates the surrounding fields using <c>FieldActivator</c>.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        private void ActivateSurroundingFields(int column, int row)
        {
            for (int i = -1; i <= 1; ++i)
                for (int j = -1; j <= 1; ++j)
                    if (!((i == 0) && (j == 0)) && _minesweeperMap.IsInMap(column + i, row + j) && (playfieldGrids[column + i, row + j].flagged == false) && !mineActivated)
                        FieldActivator(column + i, row + j);
        }

        private void StartInititialization()
        {
            gameRuns = true;
            gameResetted = false;
            dispatcherTimer.Start();
        }

        private void EndInititialization()
        {
            gameRuns = false;
            afterGame = true;
            dispatcherTimer.Stop();
        }

        private void ActivateUnflaggedFields()
        {
            for (int column = 0; column < usersColumns; column++)
                for (int row = 0; row < usersRows; row++)
                    if ((playfieldGrids[column, row].flagged == false) && !mineActivated)
                        FieldActivator(column, row);
        }

        /// <summary>
        /// Synchronizes the visibility of the button with the visibility of the playfieldGrid.
        /// </summary>
        private void SynchronizeButtonVisibility()
        {
            for (int column = 0; column < usersColumns; column++)
                for (int row = 0; row < usersRows; row++)
                    if (_minesweeperMap.IsFieldVisible(column, row))
                    {
                        GetButtonOfPlayfieldGrid(column, row).Visibility = Visibility.Hidden;
                        playfieldGrids[column, row].flagged = false;
                    }
        }

        /// <summary>
        /// Returns the amount of surrounding flagged fields.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private int EnumerateSurroundingFlags(int column, int row)
        {
            int counter = 0;
            for (int i = -1; i <= 1; ++i)
                for (int j = -1; j <= 1; ++j)
                {
                    if (!((i == 0) && (j == 0)) && _minesweeperMap.IsInMap(column + i, row + j) && (playfieldGrids[column + i, row + j].flagged == true))
                        counter++;
                }
            return counter;
        }

        /// <summary>
        /// This method must be called after initialization and before the game starts.
        /// </summary>
        private void SynchronizeModelWithView()
        {
            for (int row = 0; row < usersRows; ++row)
                for (int column = 0; column < usersColumns; ++column)
                {
                    Label label = GetLabelOfPlayfieldGrid(column, row);
                    if (_minesweeperMap.IsMine(column, row))
                        PaintMineOnField(label);
                    else if (_minesweeperMap.GetSurroundingMines(column, row) == 0)
                        label.Content = "";
                    else
                        label.Content = _minesweeperMap.GetSurroundingMines(column, row);
                }
        }

        /// <summary>
        /// The MineCounter is the Textbox showing the difference of flagged fields to total mines. This method changes the value.
        /// </summary>
        /// <param name="difference">This is not the absolute value, but the difference to change the value.</param>
        private void ChangeMineCounter(int difference)
        {
            int actualvalue = int.Parse(mineCounter.Text);
            actualvalue += difference;
            mineCounter.Text = $"{actualvalue}";
        }

        /// <summary>
        /// Paints a warning image on the current button.
        /// </summary>
        /// <param name="current"></param>
        private void PaintWarningOnButton(Button current)
        {
            Image imageWarning = new Image();
            imageWarning.Source = bitmapWarning;
            current.Content = imageWarning;
            (current.Parent as CoordinateGrid).flagged = true;
            ChangeMineCounter(-1);
        }

        /// <summary>
        /// Deletes the content of the current button.
        /// </summary>
        /// <param name="current"></param>
        private void RubWarningFromButton(Button current)
        {
            current.Content = null;
            (current.Parent as CoordinateGrid).flagged = false;
            ChangeMineCounter(1);
        }

        /// <summary>
        /// Paints a bomb image on the current label.
        /// </summary>
        /// <param name="current"></param>
        private void PaintMineOnField(Label current)
        {
            Image imageMine = new Image();
            imageMine.Source = bitmapMine;
            current.Content = imageMine;
        }
    }
}