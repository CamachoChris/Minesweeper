namespace Minesweeper
{
    class BasicField
    {
        public int surroundingMines;
        public bool isMine;
        public bool isVisible;

        public BasicField()
        {
            isMine = false;
            isVisible = false;
            surroundingMines = 0;
        }
    }
}