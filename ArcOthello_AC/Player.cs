using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArcOthello_AC
{
    class Player
    {

        private int score = 0;
        private Team team;

        public Player(Team team)
        {
            this.team = team;
        }

        public void UpdateScore(int newScore)
        {
            score = newScore;
        }

        public Point Play(Board board)
        {
            return new Point(0, 0);
        }
    }
}
