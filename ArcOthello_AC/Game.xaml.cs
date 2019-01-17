using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    public partial class Game : UserControl, INotifyPropertyChanged
    {
        #region Properties
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Board Board { get; private set; }
        public bool IsGameOn { get; private set; }
        
        private Player CurrentPlayer;
        private bool playerPassed = false;
        private Stack<Board> history;
        #endregion


        #region Indexer
        public Stack<Board> History
        {
            get { return history; }
        }
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
        public void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void NewGame(object sender = null, RoutedEventArgs e = null)
        {
            PopupMenu.IsOpen = false;
            PopupEndGame.IsOpen = false;

            Init();
        }
        public void PopupLoadCommand(object sender, RoutedEventArgs e)
        {
            PopupMenu.IsOpen = false;
            PopupEndGame.IsOpen = false;
            ((MainWindow)Application.Current.MainWindow).OpenSave();
        }

        private void GoBackToMenu(object sender, RoutedEventArgs e)
        {
            PopupEndGame.IsOpen = false;
            PopupMenu.IsOpen = true;
        }

        private void PopupMenu_Loaded(object sender, RoutedEventArgs e)
        {
            PopupMenu.HorizontalOffset = (ActualWidth - PopupMenu_content.Width) / 2;
            PopupMenu.VerticalOffset = ActualHeight / 2;
        }

        private void PopupEndGame_Loaded(object sender, RoutedEventArgs e)
        {
            PopupEndGame.HorizontalOffset = (ActualWidth - PopupEndGame_content.Width) / 2;
            PopupEndGame.VerticalOffset = ActualHeight / 2;
        }
        #endregion


        public Game()
        {
            InitializeComponent();
            BoardGrid.DataContext = this;
            Player1 = new Player(Team.Black);
            Player2 = new Player(Team.White);
            PopupMenu.IsOpen = true;
            PopupEndGame.IsOpen = false;
        }

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

        public void Init()
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
        

        #region Game Saver
        public void Save(string path)
        {
            SerializeToXML(path);
        }

        public void Load(string path)
        {
            PopupMenu.IsOpen = false;
            PopupEndGame.IsOpen = false;
            DeserializeFromXML(path);
        }

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

        private void DeserializeFromXML(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                try
                {
                    Player tmpP1 = (Player)formatter.Deserialize(stream);
                    Player tmpP2 = (Player)formatter.Deserialize(stream);
                    Player tmpCurrentPlayer = CurrentPlayer = (bool)formatter.Deserialize(stream) == true ? Player1 : Player2;
                    Board tmpBoard = (Board)formatter.Deserialize(stream);
                    bool tmpPlayerPassed = (bool)formatter.Deserialize(stream);

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
        private void Board_Click(object sender, MouseButtonEventArgs e)
        {
            if (IsGameOn)
            {
                ItemsControl i = sender as ItemsControl;
                Point p = e.GetPosition(i);
                int x = (int)(p.X / i.ActualWidth * Board.GridWidth);
                int y = (int)(p.Y / i.ActualHeight * Board.GridHeight);
                bool isWhite = CurrentPlayer.Team == Team.White;

                if (Board.IsPlayable(x, y, isWhite))
                    history.Push(new Board(Board));
                if (Board.PlayMove(x, y, isWhite))
                    EndTurn();
            }
        }

        private void NextPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        }

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

        private bool CanCurrentPlayerPlay()
        {
            return Board.NumberPossibleMove(CurrentPlayer.Team) > 0;
        }

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
