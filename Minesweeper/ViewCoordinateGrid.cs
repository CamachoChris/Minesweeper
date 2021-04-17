using System.Windows.Controls;

namespace ViewMinesweeper
{
    /// <summary>
    /// Inherit of <c>Grid</c> that remembers coordinates.
    /// </summary>
    class CoordinateGrid : Grid
    {
        public int column;
        public int row;
        public bool flagged;

        public CoordinateGrid()
        {
            flagged = false;
        }
    }
}
