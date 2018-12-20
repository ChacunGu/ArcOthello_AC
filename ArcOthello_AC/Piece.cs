using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthello_AC
{
    class Piece
    {

        private Team team;
        public Point Position { get; }

        public Piece(Team team, Point position)
        {
            this.team = team;
            this.Position = position;
        }

        private void Flip()
        {
            team = team == Team.Black ? Team.White : Team.Black;
        }
    }
}
