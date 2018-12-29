using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthello_AC
{
    public class Board : INotifyPropertyChanged
    {
        #region Properties
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }

        private ObservableCollection<ObservableCollection<Piece>> pieces;

        public ObservableCollection<ObservableCollection<Piece>> Pieces
        {
            get { return pieces; }
        }
        #endregion

        #region Indexer
        public Piece this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= GridHeight)
                    throw new ArgumentOutOfRangeException("row", row, "Invalid Row Index");
                if (col < 0 || col >= GridWidth)
                    throw new ArgumentOutOfRangeException("col", col, "Invalid Column Index");
                return pieces[row][col];
            }
        }
        #endregion


        public Board(int width, int height)
        {
            this.GridWidth = width;
            this.GridHeight = height;
            this.Init();
        }

        public void Init()
        {
            pieces = new ObservableCollection<ObservableCollection<Piece>>();
            for (int i = 0; i < GridWidth; i++)
            {
                ObservableCollection<Piece> col = new ObservableCollection<Piece>();
                for (int j = 0; j < GridHeight; j++)
                {
                    Piece p = new Piece(Team.None, i, j);
                    col.Add(p);
                }
                pieces.Add(col);
            }
            pieces[3][2].SetTeam(Team.White);
            pieces[3][3].SetTeam(Team.Black);
            pieces[4][2].SetTeam(Team.Black);
            pieces[4][3].SetTeam(Team.White);

        }

        public bool PosePiece(int row, int col, Team team)
        {
            if (pieces[col][row].Team == Team.None)
            {
                pieces[col][row].SetTeam(team);

                FlipPiece(row, col, team, 1, 0).ForEach(p => p.Flip());
                FlipPiece(row, col, team, -1, 0).ForEach(p => p.Flip());
                FlipPiece(row, col, team, 1, 1).ForEach(p => p.Flip());
                FlipPiece(row, col, team, -1, -1).ForEach(p => p.Flip());
                FlipPiece(row, col, team, 0, 1).ForEach(p => p.Flip());
                FlipPiece(row, col, team, 0, -1).ForEach(p => p.Flip());

                return true;
            }

            return false;
        }

        private List<Piece> FlipPiece(int row, int col, Team team, int incX, int incY)
        {
            List<Piece> flipPiece = new List<Piece>();
            try
            {
                row += incY;
                col += incX;

                while (pieces[col][row].Team != team)
                {
                    if (pieces[col][row].Team == Team.None)
                        throw new ArgumentOutOfRangeException();
                    flipPiece.Add(pieces[col][row]);
                    row += incY;
                    col += incX;
                }
                return flipPiece;
            } catch (ArgumentOutOfRangeException)
            {
                return new List<Piece>();
            }
        }

        #region PropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
