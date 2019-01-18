using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ArcOthello_AC
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : UserControl, INotifyPropertyChanged
    {
        #region Properties
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Board Board { get; private set; }
        public bool IsGameOn { get; private set; }

        public Stack<Board> History
        {
            get { return history; }
        }

        #endregion

        #region Variables
        private Player CurrentPlayer;
        private bool playerPassed = false;
        private Stack<Board> history;
        #endregion


        #region Timer
        Stopwatch stopWatch = new Stopwatch();

        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            CurrentPlayer.Time = CurrentPlayer.Time.Add(stopWatch.Elapsed);
            stopWatch.Restart();
        }
        #endregion

        #region GUI events
        /// <summary>
        /// Shutdown the application when called
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event param</param>
        public void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Create a new game
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event param</param>
        public void NewGame(object sender = null, RoutedEventArgs e = null)
        {
            PopupMenu.IsOpen = false;
            PopupEndGame.IsOpen = false;

            Init();
        }

        /// <summary>
        /// Load a game from the popup window
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event param</param>
        public void PopupLoadCommand(object sender, RoutedEventArgs e)
        {
            PopupMenu.IsOpen = false;
            PopupEndGame.IsOpen = false;
            ((MainWindow)Application.Current.MainWindow).OpenSave();
        }

        /// <summary>
        /// When the game has ended and we have decided to go back to the menu, close the end game popup and open the menu popup
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event param</param>
        private void GoBackToMenu(object sender, RoutedEventArgs e)
        {
            PopupEndGame.IsOpen = false;
            PopupMenu.IsOpen = true;
        }

        /// <summary>
        /// When the popup is loaded, set its position
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event param</param>
        private void PopupMenu_Loaded(object sender, RoutedEventArgs e)
        {
            PopupMenu.HorizontalOffset = (ActualWidth - PopupMenu_content.Width) / 2;
            PopupMenu.VerticalOffset = ActualHeight / 2;
        }

        /// <summary>
        /// When the end game popup is loaded, set its position
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event param</param>
        private void PopupEndGame_Loaded(object sender, RoutedEventArgs e)
        {
            PopupEndGame.HorizontalOffset = (ActualWidth - PopupEndGame_content.Width) / 2;
            PopupEndGame.VerticalOffset = ActualHeight / 2;
        }
        #endregion

        #region Constructor
        public Game()
        {
            InitializeComponent();
            BoardGrid.DataContext = this;
            Player1 = new Player(Team.Black);
            Player2 = new Player(Team.White);
            PopupMenu.IsOpen = true;
            PopupEndGame.IsOpen = false;
        }
        #endregion

        #region HelperMethods
        /// <summary>
        /// Init the data contexts and some game variables
        /// </summary>
        private void InitContext()
        {
            PieceList.DataContext = Board;

            ScoreP1.DataContext = Player1;
            ScoreP2.DataContext = Player2;

            TimeP1.DataContext = Player1;
            TimeP2.DataContext = Player2;

            history = new Stack<Board>();

            RaisePropertyChanged("Player1");
            RaisePropertyChanged("Player2");

            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            dispatcherTimer.Start();
            stopWatch.Start();
        }

        /// <summary>
        /// Initialise the players and the game board
        /// </summary>
        private void Init()
        {
            Player1 = new Player(Team.Black);
            Player2 = new Player(Team.White);
            CurrentPlayer = Player1;

            Board = new Board(Constants.GRID_WIDTH, Constants.GRID_HEIGHT);

            InitContext();

            IsGameOn = true;
            playerPassed = false;

            ShowPossibleMove();
            RecalculateScore();
        }
        #endregion

        #region Game Saver
        /// <summary>
        /// Save the game at the specified path
        /// </summary>
        /// <param name="path">The path to save the game</param>
        public void Save(string path)
        {
            SerializeToXML(path);
        }

        /// <summary>
        /// Load the specified game file
        /// </summary>
        /// <param name="path">The path of the game to load</param>
        public void Load(string path)
        {
            PopupMenu.IsOpen = false;
            PopupEndGame.IsOpen = false;
            DeserializeFromXML(path);
        }

        /// <summary>
        /// Serialise the game to a file at the specified path
        /// </summary>
        /// <param name="path">The path of the file</param>
        private void SerializeToXML(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, Player1);
                formatter.Serialize(stream, Player2);
                formatter.Serialize(stream, CurrentPlayer.Team == Team.Black ? true : false);
                formatter.Serialize(stream, Board);
                formatter.Serialize(stream, playerPassed);
            }
        }

        /// <summary>
        /// Deserialize the file and load the game
        /// </summary>
        /// <param name="path">The path of the save file</param>
        private void DeserializeFromXML(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                try
                {
                    // The temporary variable are here to avoid setting game variables to some value that might be incorrect if the file is corrupted
                    Player tmpP1 = (Player)formatter.Deserialize(stream);
                    Player tmpP2 = (Player)formatter.Deserialize(stream);
                    Player tmpCurrentPlayer = CurrentPlayer = (bool)formatter.Deserialize(stream) == true ? Player1 : Player2;
                    Board tmpBoard = (Board)formatter.Deserialize(stream);
                    bool tmpPlayerPassed = (bool)formatter.Deserialize(stream);

                    // From here, we can say that the file is correct
                    Player1 = tmpP1;
                    Player2 = tmpP2;
                    CurrentPlayer = tmpCurrentPlayer;
                    Board = tmpBoard;
                    playerPassed = tmpPlayerPassed;

                    IsGameOn = true;
                    InitContext();
                }
                catch(SerializationException e)
                {
                    MessageBoxResult result = MessageBox.Show("There is a problem with your file",
                                          "Problem",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                }
            }
        }
        #endregion

        #region Undo
        /// <summary>
        /// Undo the previous move
        /// </summary>
        public void Undo()
        {
            if (IsGameOn)
            {
                Board backupBoard = history.Pop();

                // set the board's content
                for (int y = 0; y < Board.GridHeight; y++)
                {
                    for (int x = 0; x < Board.GridWidth; x++)
                    {
                        Board.SetPiece(y, x, backupBoard[y, x]);
                    }
                }
                
                RecalculateScore(); // update score
                NextPlayer(); // change player
            }
        }
        #endregion

        #region Game Logic
        /// <summary>
        /// When there is a click on the board, pose a piece if the position is correct
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event param</param>
        private void Board_Click(object sender, MouseButtonEventArgs e)
        {
            if (IsGameOn)
            {
                // Get the position on the board
                ItemsControl i = sender as ItemsControl;
                Point p = e.GetPosition(i);
                int x = (int)(p.X / i.ActualWidth * Board.GridWidth);
                int y = (int)(p.Y / i.ActualHeight * Board.GridHeight);
                bool isWhite = CurrentPlayer.Team == Team.White;
                
                // Check if the move is correct and pose the piece
                if (Board.IsPlayable(x, y, isWhite))
                    history.Push(new Board(Board));
                if (Board.PlayMove(x, y, isWhite))
                    EndTurn();
            }
        }

        /// <summary>
        /// Used to switch from player 1 to player 2 and vice versa
        /// </summary>
        private void NextPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        }

        /// <summary>
        /// When a turn has ended, clear the preview and switch players 
        /// </summary>
        private void EndTurn()
        {
            Board.ClearPreview();
            RecalculateScore();
            NextPlayer();

            if (CanCurrentPlayerPlay())
            {
                playerPassed = false;
                ShowPossibleMove();
            }
            else
            {
                if (playerPassed)
                    EndGame();
                else
                {
                    playerPassed = true;
                    EndTurn();
                }
            }
        }

        /// <summary>
        /// Check whether the current player can play
        /// </summary>
        /// <returns>True if the player can play, false otherwise </returns>
        private bool CanCurrentPlayerPlay()
        {
            return Board.NumberPossibleMove(CurrentPlayer.Team) > 0;
        }

        /// <summary>
        /// Called when the game has ended and show the winner
        /// </summary>
        private void EndGame()
        {
            IsGameOn = false;
            Board = null;
            history = new Stack<Board>();
            PopupEndGame.IsOpen = true;
            PopupEndGame_WinnerName.Text = (Player1.Score > Player2.Score ? "Nvidia wins the game" : 
                                            Player1.Score == Player2.Score ? "Draw !" :
                                                                             "AMD wins the game");
        }
                
        /// <summary>
        /// Calculate the new score and update the display
        /// </summary>
        private void RecalculateScore()
        {
            Player1.Score = 0;
            Player2.Score = 0;
            foreach(ObservableCollection<Piece> row in Board.Pieces)
            {
                foreach(Piece p in row)
                {
                    if (p.Team == Player1.Team)
                        Player1.Score++;
                    else if (p.Team == Player2.Team)
                        Player2.Score++;
                }
            }
            RaisePropertyChanged("Player1");
            RaisePropertyChanged("Player2");
        }

        /// <summary>
        /// Set the preview on the possible moves
        /// </summary>
        private void ShowPossibleMove()
        {
            Board.ShowPossibleMove(CurrentPlayer.Team);
        }
        #endregion

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
