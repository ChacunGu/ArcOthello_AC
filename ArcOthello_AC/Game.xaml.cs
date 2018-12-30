using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArcOthello_AC
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : UserControl, IPlayable.IPlayable
    {
        private Player Player1 = new Player(Team.Black);
        private Player Player2 = new Player(Team.White);

        private Board board = new Board(Constants.GRID_WIDTH, Constants.GRID_HEIGHT);
        
        private Player CurrentPlayer;

        #region Timer

        Stopwatch stopWatch = new Stopwatch();

        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            CurrentPlayer.Time = CurrentPlayer.Time.Add(stopWatch.Elapsed);
            stopWatch.Restart();
        }

        #endregion


        public Game()
        {
            InitializeComponent();
            PieceList.DataContext = board;
            ScoreP1.DataContext = Player1;
            ScoreP2.DataContext = Player2;
            TimeP1.DataContext = Player1;
            TimeP2.DataContext = Player2;
            
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);

            Init();
        }

        private void Init()
        {
            CurrentPlayer = Player1;
            ShowPossibleMove();

            dispatcherTimer.Start();
            stopWatch.Start();
        }


        #region Game Saver
        private void Restart()
        {

        }

        private void Save()
        {

        }

        private void Load()
        {

        }
        #endregion

        #region Game Logic
        private void Board_Click(object sender, MouseButtonEventArgs e)
        {
            ItemsControl i = sender as ItemsControl;
            Point p = e.GetPosition(i);
            int x = (int)(p.X / i.ActualWidth * board.GridWidth);
            int y = (int)(p.Y / i.ActualHeight * board.GridHeight);
            if (board.PosePiece(y, x, CurrentPlayer.Team))
                EndTurn();
        }

        private void NextPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        }

        private void EndTurn()
        {
            ClearPreview();
            RecalculateScore();
            NextPlayer();
            ShowPossibleMove();
        }

        private void ClearPreview()
        {
            foreach (ObservableCollection<Piece> row in board.Pieces)
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

        private void RecalculateScore()
        {
            Player1.Score = 0;
            Player2.Score = 0;
            foreach(ObservableCollection<Piece> row in board.Pieces)
            {
                foreach(Piece p in row)
                {
                    if (p.Team == Player1.Team)
                        Player1.Score++;
                    else if (p.Team == Player2.Team)
                        Player2.Score++;
                }
            }
        }

        private void ShowPossibleMove()
        {
            board.ShowPossibleMove(CurrentPlayer.Team);
        }
        #endregion

        #region IPlayable Implementation
        /// <summary>
        /// Returns the IA's name
        /// </summary>
        /// <returns>IA's name</returns>
        public string GetName()
        {
            return "trouver_un_nom_pour_l_IA";
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
            return board.IsValid(row, column, isWhite ? Team.White : Team.Black);
        }

        /// <summary>
        /// Will update the board status if the move is valid and return true
        /// Will return false otherwise (board is unchanged)
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <param name="isWhite">true for white move, false for black move</param>
        /// <returns></returns>
        public bool PlayMove(int column, int line, bool isWhite)
        {
            return board.PosePiece(line, column, CurrentPlayer.Team);
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
            throw new NotImplementedException(); // TODO
        }

        /// <summary>
        /// Returns a reference to a 2D array with the board status
        /// </summary>
        /// <returns>The 7x9 tiles status</returns>
        public int[,] GetBoard()
        {
            int[,] boardInt = new int[board.GridHeight, board.GridWidth];
            for (int y=0; y<board.GridHeight; y++)
            {
                for (int x=0; x<board.GridWidth; x++)
                {
                    boardInt[y, x] = board[y, x] == null ? -1 :
                                     board[y, x].Team == Team.White ? 0 :
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
            return Player1.Score;
        }

        /// <summary>
        /// Returns the number of black tiles
        /// </summary>
        /// <returns>black player's score</returns>
        public int GetBlackScore()
        {
            return Player2.Score;
        }
        #endregion
    }
}
