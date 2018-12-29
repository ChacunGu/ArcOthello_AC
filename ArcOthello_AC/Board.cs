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
            if (!IsOccupied(row, col))
            {
                pieces[col][row].SetTeam(team);

                GetFlipPieceList(row, col, team).ForEach(p => p.Flip());

                return true;
            }

            return false;
        }

        private bool IsOccupied(int row, int col)
        {
            return pieces[col][row].Team == Team.Black || pieces[col][row].Team == Team.White;
        }

        public void ShowPossibleMove(Team team)
        {
            Team preview = team == Team.Black ? Team.BlackPreview : Team.WhitePreview;
            for (int i = 0; i < GridWidth; i++)
            {
                for (int j = 0; j < GridHeight; j++)
                {
                    if (GetFlipPieceList(j, i, team).Count() != 0)
                        pieces[i][j].Team = preview;

                }
            }
        }

        private List<Piece> GetFlipPieceList(int row, int col, Team team)
        {
            return GetFlipPieceList(row, col, team, 1, 0)
                .Concat(GetFlipPieceList(row, col, team, -1, 0))
                .Concat(GetFlipPieceList(row, col, team, 1, 1))
                .Concat(GetFlipPieceList(row, col, team, -1, -1))
                .Concat(GetFlipPieceList(row, col, team, -1, 1))
                .Concat(GetFlipPieceList(row, col, team, 1, -1))
                .Concat(GetFlipPieceList(row, col, team, 0, 1))
                .Concat(GetFlipPieceList(row, col, team, 0, -1)).ToList();
        }

        private List<Piece> GetFlipPieceList(int row, int col, Team team, int incX, int incY)
        {
            List<Piece> flipPiece = new List<Piece>();

            Team enemyTeam = team == Team.Black ? Team.White : Team.Black;

            row += incY;
            col += incX;

            while (IsValid(col, row) && pieces[col][row].Team == enemyTeam)
            {
                if (!IsValid(col, row))
                    return new List<Piece>();

                flipPiece.Add(pieces[col][row]);
                row += incY;
                col += incX;
            }

            if (!IsValid(col, row))
                return new List<Piece>();

            return flipPiece;
        }

        private bool IsValid(int col, int row)
        {
            if (col < 0 || col >= GridWidth)
                return false;
            if (row < 0 || row >= GridHeight)
                return false;
            if (pieces[col][row].Team == Team.None || pieces[col][row].Team == Team.BlackPreview || pieces[col][row].Team == Team.WhitePreview)
                return false;

            return true;
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
