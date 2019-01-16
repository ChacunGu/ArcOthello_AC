using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthello_AC
{
    [Serializable]
    public class Board : INotifyPropertyChanged, IPlayable.IPlayable
    {
        #region Properties
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }

        private ObservableCollection<ObservableCollection<Piece>> pieces;
        #endregion

        #region Indexer
        public ObservableCollection<ObservableCollection<Piece>> Pieces
        {
            get { return pieces; }
        }

        public Piece this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= GridHeight)
                    throw new ArgumentOutOfRangeException("row", row, "Invalid Row Index");
                if (col < 0 || col >= GridWidth)
                    throw new ArgumentOutOfRangeException("col", col, "Invalid Column Index");
                return pieces[col][row];
            }
        }
        #endregion

        public Board(int width = 9, int height = 7)
        {
            this.GridWidth = width;
            this.GridHeight = height;
            this.Init();
        }

        public Board(Board b)
        {
            this.GridWidth = b.GridWidth;
            this.GridHeight = b.GridHeight;
            Init(b.pieces);
        }

        public void Init()
        {
            pieces = new ObservableCollection<ObservableCollection<Piece>>();
            for (int x = 0; x < GridWidth; x++)
            {
                ObservableCollection<Piece> col = new ObservableCollection<Piece>();
                for (int y = 0; y < GridHeight; y++)
                {
                    Piece p = new Piece(Team.None, x, y);
                    col.Add(p);
                }
                pieces.Add(col);
            }
            pieces[3][2].SetTeam(Team.White);
            pieces[3][3].SetTeam(Team.Black);
            pieces[4][2].SetTeam(Team.Black);
            pieces[4][3].SetTeam(Team.White);
        }

        public void Init(ObservableCollection<ObservableCollection<Piece>> piecesToCopy)
        {
            pieces = new ObservableCollection<ObservableCollection<Piece>>();
            for (int x = 0; x < GridWidth; x++)
            {
                ObservableCollection<Piece> col = new ObservableCollection<Piece>();
                for (int y = 0; y < GridHeight; y++)
                {
                    Piece p = new Piece(piecesToCopy[x][y]);
                    col.Add(p);
                }
                pieces.Add(col);
            }
        }

        public void SetPiece(int row, int col, Piece p)
        {
            if (row < 0 || row >= GridHeight)
                throw new ArgumentOutOfRangeException("row", row, "Invalid Row Index");
            if (col < 0 || col >= GridWidth)
                throw new ArgumentOutOfRangeException("col", col, "Invalid Column Index");
            pieces[col][row] = p;
        }

        public bool PosePiece(int row, int col, Team team)
        {
            if (IsPositionValid(row, col))
            {
                pieces[col][row].SetTeam(team);

                GetFlipPieceList(row, col, team).ForEach(p => p.Flip());

                return true;
            }

            return false;
        }

        private bool IsPositionValid(int row, int col)
        {
            return pieces[col][row].Team == Team.BlackPreview || pieces[col][row].Team == Team.WhitePreview;
        }

        public void ShowPossibleMove(Team team)
        {
            Team preview = team == Team.Black ? Team.BlackPreview : Team.WhitePreview;
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    if (GetFlipPieceList(y, x, team).Count() != 0 && pieces[x][y].Team == Team.None)
                        pieces[x][y].Team = preview;
                }
            }
        }

        public int NumberPossibleMove(Team team)
        {
            int count = 0;
            Team preview = team == Team.Black ? Team.BlackPreview : Team.WhitePreview;
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    count += GetFlipPieceList(y, x, team).Count() != 0 && pieces[x][y].Team == Team.None ? 1 : 0;
                }
            }
            return count;
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

            while (IsValid(row, col) && pieces[col][row].Team == enemyTeam)
            {
                flipPiece.Add(pieces[col][row]);
                row += incY;
                col += incX;
            }

            if (IsValid(row, col) && pieces[col][row].Team == team)
                return flipPiece;
            return new List<Piece>();
        }

        public bool IsValid(int row, int col)
        {
            if (col < 0 || col >= GridWidth)
                return false;
            if (row < 0 || row >= GridHeight)
                return false;
            if (pieces[col][row].Team == Team.None || pieces[col][row].Team == Team.BlackPreview || pieces[col][row].Team == Team.WhitePreview)
                return false;

            return true;
        }

        public bool IsValid(int row, int col, Team team)
        {
            return col >= 0 && col < GridWidth && 
                   row >= 0 && row < GridHeight && 
                   pieces[col][row].Team == (team == Team.White ? Team.WhitePreview : Team.BlackPreview);
        }

        public void ClearPreview()
        {
            foreach (ObservableCollection<Piece> row in Pieces)
            {
                foreach (Piece p in row)
                {
                    if (IsPreview(p))
                        p.Team = Team.None;
                }
            }
        }

        private bool IsPreview(Piece p)
        {
            return p.Team == Team.BlackPreview || p.Team == Team.WhitePreview;
        }

        #region PropertyChanged implementation
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IPlayable Implementation
        /// <summary>
        /// Returns the IA's name
        /// </summary>
        /// <returns>IA's name</returns>
        public string GetName()
        {
            return "Jack";
        }

        /// <summary>
        /// Returns true if the move is valid for specified color
        /// </summary>
        /// <param name="column">value between 0 and 8</param>
        /// <param name="row">value between 0 and 6</param>
        /// <param name="isWhite"></param>
        /// <returns>true or false</returns>
        public bool IsPlayable(int column, int row, bool isWhite)
        {
            return IsValid(row, column, isWhite ? Team.White : Team.Black);
        }

        /// <summary>
        /// Will update the board status if the move is valid and return true
        /// Will return false otherwise (board is unchanged)
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="row">value between 0 and 7</param>
        /// <param name="isWhite">true for white move, false for black move</param>
        /// <returns></returns>
        public bool PlayMove(int column, int row, bool isWhite)
        {
            bool canPlay = PosePiece(row, column, isWhite ? Team.White : Team.Black);
            ClearPreview();
            ShowPossibleMove(isWhite ? Team.Black : Team.White);
            return canPlay;
        }

        /// <summary>
        /// Asks the game engine next (valid) move given a game position
        /// The board assumes following standard move notation:
        /// 
        ///             A B C D E F G H I
        ///         [ ][0 1 2 3 4 5 6 7 8]     (first index)
        ///        1 0
        ///        2 1
        ///        3 2        X
        ///        4 3            X
        ///        5 4
        ///        6 5
        ///        7 6
        ///       
        ///          Column Line
        ///  E.g.:    D3, F4 game notation will map to {3,2} resp. {5,3}
        /// </summary>
        /// <param name="game">a 2D board with integer values: 0 for white 1 for black and -1 for empty tiles. First index for the column, second index for the line</param>
        /// <param name="level">an integer value to set the level of the IA, 5 normally</param>
        /// <param name="whiteTurn">true if white players turn, false otherwise</param>
        /// <returns>The column and line indices. Will return {-1,-1} as PASS if no possible move </returns>
        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            // save board
            throw new NotImplementedException(); // TODO
            // restore board
        }

        /// <summary>
        /// Finds optimized move.
        /// </summary>
        /// <param name="gameRoot">a 2D board with integer values: 0 for white 1 for black and -1 for empty tiles. First index for the column, second index for the line</param>
        /// <param name="level">an integer value to set the level of the IA, 5 normally</param>
        /// <param name="minOrMax">1 for maximize -1 for minimize</param>
        /// <param name="parentScore">parent's board fitness value</param>
        /// <param name="pieceSample">0 for white 1 for black</param>
        /// <returns></returns>
        private Tuple<int, Tuple<int, int>> alphabeta(int[,] gameRoot, int level, int minOrMax, int parentScore, int pieceSample)
        {
            bool isWhite = pieceSample== 0;
            if (level == 0 || IsFinal(gameRoot))
                return new Tuple<int, Tuple<int, int>>(Eval(gameRoot, isWhite), null);
            int optVal = minOrMax * int.MaxValue;
            Tuple<int, int> optOp = null;
            foreach (var op in GetOps(gameRoot, isWhite))
            {
                int[,] newBoard = Apply(gameRoot, op, isWhite);
                Tuple<int, Tuple<int, int>> alphabetaRes = alphabeta(newBoard, level-1, -minOrMax, optVal, (pieceSample+1)%2);
                int val = alphabetaRes.Item1;
                Tuple<int, int> dummy = alphabetaRes.Item2;
                if (val * minOrMax > parentScore * minOrMax)
                    break;
            }
            return new Tuple<int, Tuple<int, int>>(optVal, optOp);
        }

        /// <summary>
        /// Returns true if the board is final false otherwise
        /// </summary>
        /// <param name="gameRoot">a 2D board with integer values: 0 for white 1 for black and -1 for empty tiles. First index for the column, second index for the line</param>
        /// <returns>true if the board is final false otherwise</returns>
        private bool IsFinal(int[,] gameRoot)
        {
            for (int y = 0; y < gameRoot.GetLength(0); y++)
            {
                for (int x = 0; x < gameRoot.GetLength(1); x++)
                {
                    if (gameRoot[x, y] == -1)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns board fitness value.
        /// </summary>
        /// <param name="gameRoot">a 2D board with integer values: 0 for white 1 for black and -1 for empty tiles. First index for the column, second index for the line</param>
        /// <param name="whiteTurn">true if white players turn false otherwise</param>
        /// <returns>board fitness value</returns>
        private int Eval(int[,] gameRoot, bool whiteTurn)
        {
            return whiteTurn ? GetWhiteScore() : GetBlackScore(); // TODO
        }

        /// <summary>
        /// Returns applicable operators for the given board.
        /// </summary>
        /// <param name="gameRoot">a 2D board with integer values: 0 for white 1 for black and -1 for empty tiles. First index for the column, second index for the line</param>
        /// <param name="whiteTurn">true if white players turn false otherwise</param>
        /// <returns>List of applicable operators</returns>
        private List<Tuple<int, int>> GetOps(int[,] gameRoot, bool whiteTurn)
        {
            // convert board (int) to "our" board
            // ShowPossibleMoves()
            // return slots containing preview pieces
            throw new NotImplementedException(); // TODO
        }

        /// <summary>
        /// Applies given move on the given board.
        /// </summary>
        /// <param name="gameRoot">a 2D board with integer values: 0 for white 1 for black and -1 for empty tiles. First index for the column, second index for the line</param>
        /// <param name="op">operator to apply</param>
        /// <param name="whiteTurn">true if white players turn false otherwise</param>
        /// <returns>modified 2D board with integer values</returns>
        private int[,] Apply(int[,] gameRoot, Tuple<int, int> op, bool whiteTurn)
        {
            // convert board (int) to "our" board
            // PosePiece() with op as position and whiteTurn for team
            // convert back the board to int array
            throw new NotImplementedException(); // TODO
        }

        /// <summary>
        /// Returns a reference to a 2D array with the board status
        /// </summary>
        /// <returns>The 7x9 tiles status</returns>
        public int[,] GetBoard()
        {
            int[,] boardInt = new int[GridHeight, GridWidth];
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    boardInt[y, x] = this[y, x] == null ? -1 :
                                     this[y, x].Team == Team.White ? 0 :
                                     1;
                }
            }
            return boardInt;
        }

        /// <summary>
        /// Returns the number of white tiles on the board
        /// </summary>
        /// <returns>white player's score</returns>
        public int GetWhiteScore()
        {
            int whiteScore = 0;
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    if (this[y, x] != null)
                        whiteScore += this[y, x].Team == Team.White ? 1 : 0;
                }
            }
            return whiteScore;
        }

        /// <summary>
        /// Returns the number of black tiles
        /// </summary>
        /// <returns>black player's score</returns>
        public int GetBlackScore()
        {
            int blackScore = 0;
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    if (this[y, x] != null)
                        blackScore += this[y, x].Team == Team.Black ? 1 : 0;
                }
            }
            return blackScore;
        }
        #endregion
    }
}
