using System;

namespace Minesweeper
{
    class MinesweeperMap
    {
        private readonly BasicField[,] _map;
        private readonly int _totalColumns;
        private readonly int _totalRows;
        private readonly int _totalMines;

        public MinesweeperMap(int totalColumns, int totalRows, int totalMines, int startColumn, int startRow)
        {
            _totalColumns = totalColumns;
            _totalRows = totalRows;
            _totalMines = totalMines;

            _map = new BasicField[totalColumns, totalRows];
            for (int row = 0; row < _totalRows; ++row)
                for (int column = 0; column < _totalColumns; ++column)
                {
                    _map[column, row] = new BasicField();
                }

            SetRandomBombs(startColumn, startRow);
            EnumerateSurroundingBombs();
        }

        /// <summary>
        /// The central method for game control
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns>false for mine. true for field with number or empty field which reveals adjacent fields</returns>
        public bool ActivateField(int column, int row)
        {
            GetHiddenFields();
            if (_map[column, row].isMine)
            {
                RevealMap();
                return false;
            }
            if (_map[column, row].surroundingMines == 0)
            {
                RevealOpenSea(column, row);
                return true;
            }
            _map[column, row].isVisible = true;
            return true;
        }

        /// <summary>
        /// For GUI request to redraw the playfield.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns>true if visible, false if not</returns>
        public bool IsFieldVisible(int column, int row)
        {
            return _map[column, row].isVisible;
        }

        /// <summary>
        /// For GUI request to set the number of surrounding bombs on a field.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns>number of surrounding bombs.</returns>
        public int GetSurroundingMines(int column, int row)
        {
            return _map[column, row].surroundingMines;
        }

        /// <summary>
        /// For GUI request to paint a bomb or not.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns>true is bomb. false is not bomb</returns>
        public bool IsMine(int column, int row)
        {
            return _map[column, row].isMine;
        }

        public bool IsEasyMode()
        {
            if (_totalColumns == 9 && _totalRows == 9 && _totalMines == 10)
                return true;
            else return false;
        }

        public bool IsMediumMode()
        {
            if (_totalColumns == 16 && _totalRows == 16 && _totalMines == 40)
                return true;
            else return false;
        }

        public bool IsHardMode()
        {
            if (_totalColumns == 30 && _totalRows == 16 && _totalMines == 99)
                return true;
            else return false;
        }

        /// <summary>
        /// Check, if coordinate is in the map area.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool IsInMap(int column, int row)
        {
            if ((column < 0) || (row < 0) || (column >= _totalColumns) || (row >= _totalRows))
                    return false;
            return true;
        }

        /// <summary>
        /// The game is won when only the mines remain hidden.
        /// </summary>
        /// <returns>Returns true if won. False does not implicitly mean the game is lost.</returns>
        public bool IsGameWon()
        {
            if (GetHiddenFields() - _totalMines == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Recursive method to reveal all adjacent fields with surrounding bombs = 0.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        private void RevealOpenSea(int column, int row)
        {
            if ((_map[column, row].surroundingMines == 0) && (_map[column, row].isVisible == false))
            {
                _map[column, row].isVisible = true;
                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                        if (IsInMap(column + i, row + j) && (!((i == 0) && (j == 0))))
                        {                        
                            if (_map[column + i, row + j].surroundingMines == 0)
                                RevealOpenSea(column + i, row + j);
                            else
                                _map[column + i, row + j].isVisible = true;
                        }
            }
        }        

        /// <summary>
        /// Sets all fields to <c>Visible.</c>
        /// </summary>
        private void RevealMap()
        {
            for (int row = 0; row < _totalRows; row++)
                for (int column = 0; column < _totalColumns; column++)
                    _map[column, row].isVisible = true;
        }

        /// <summary>
        /// Counts the amount of hidden fields.
        /// </summary>
        /// <returns>Returns the total amount of hidden fields.</returns>
        private int GetHiddenFields()
        {
            int counter = 0;
            for (int column = 0; column < _totalColumns; column++)
                for (int row = 0; row < _totalRows; row++)
                    if (_map[column, row].isVisible == false)
                        counter++;
            return counter;
        }

        /// <summary>
        /// Part of Initialization. Uses the random class to position bombs on the map.
        /// </summary>
        private void SetRandomBombs(int startColumn, int startRow)
        {
            Random rnd = new Random();
            for (int i = 0; i < _totalMines; i++)
            {
                int rndColumn;
                int rndRow;
                do
                {
                    rndColumn = rnd.Next(0, _totalColumns);
                    rndRow = rnd.Next(0, _totalRows);
                } while (_map[rndColumn, rndRow].isMine || ((startColumn == rndColumn) && (startRow == rndRow)));

                _map[rndColumn, rndRow].isMine = true;
            }
        }

        /// <summary>
        /// Part of Initializiation. Enumerates the surrounding bombs.
        /// </summary>
        private void EnumerateSurroundingBombs()
        {
            int counter;
            for (int row = 0; row < _totalRows; row++)            
                for (int column = 0; column < _totalColumns; column++)
                {
                    counter = 0;
                    for (int i = -1; i <= 1; i++)
                        for (int j = -1; j <= 1; j++)
                        {
                            if ( !((i == 0) && (j == 0)) && IsInMap(column + i, row + j) )
                                if (_map[column + i, row + j].isMine)
                                    ++counter;
                        }                    
                    _map[column, row].surroundingMines = counter;        
                }            
        }
    }
}