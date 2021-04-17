using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    [Serializable]
    public class Highscore
    {
        public const int MAXENTRIES = 10;
        public HighscoreField[] _highscoreFields;
        public int _entries;

        public Highscore()
        {
            _highscoreFields = new HighscoreField[MAXENTRIES];
            for (int i = 0; i < MAXENTRIES; i++)
            {
                HighscoreField highscoreField = new HighscoreField("...", 999);
                _highscoreFields[i] = highscoreField;
            }
            _entries = 0;
        }

        public void Add(string name, int score)
        {
            HighscoreField highscoreField = new HighscoreField(name, score);
            if (_entries < MAXENTRIES && IsNewBetter(highscoreField))
            {
                _highscoreFields[_entries] = highscoreField;
                _entries++;
                if (_entries > 1)
                    Sort();
            }
            else
            {
                if (IsNewBetter(highscoreField))
                {
                    _highscoreFields[MAXENTRIES - 1] = highscoreField;
                    Sort();
                }
            }
            
        }

        private void Sort()
        {
            for (int i = 1; i < _entries; i++)
                for (int j = 0; j < _entries - i ; j++)
                {
                    if (_highscoreFields[j].Score > _highscoreFields[j + 1].Score)
                    {
                        HighscoreField tmp;
                        tmp = _highscoreFields[j];
                        _highscoreFields[j] = _highscoreFields[j + 1];
                        _highscoreFields[j + 1] = tmp;
                    }
                    

                }
        }

        public HighscoreField[] GetHighscoreList()
        {
            return _highscoreFields;
        }

        public bool IsNewBetter(HighscoreField newHighscoreField)
        {
            if (newHighscoreField.Score < _highscoreFields[MAXENTRIES - 1].Score)
                return true;
            else return false;
        }

        public bool IsNewBetter(int newScore)
        {
            if (newScore < _highscoreFields[MAXENTRIES - 1].Score)
                return true;
            else return false;
        }
    }
}
