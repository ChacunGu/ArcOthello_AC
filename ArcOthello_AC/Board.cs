using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthello_AC
{
    class Board
    {

        private Size size;
        private Piece[,] content; // ObservableCollection ?

        public Board(Size size)
        {
            this.size = size;
            this.Init();
        }

        public void Init()
        {
            content = new Piece[size.Width, size.Height];
            
        }

        public void PosePiece(Piece piece)
        {
            content[piece.Position.X, piece.Position.Y] = piece;
        }
    }
}
