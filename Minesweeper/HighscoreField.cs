using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class HighscoreField
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public HighscoreField(){}
        public HighscoreField (string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}
