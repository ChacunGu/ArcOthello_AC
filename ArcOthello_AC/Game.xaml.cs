using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ArcOthello_AC
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : UserControl
    {
        private Player Player1 = new Player(Team.Black);
        private Player Player2 = new Player(Team.White);

        private Board board = new Board(Constants.GRID_WIDTH, Constants.GRID_HEIGHT);
        
        private Player CurrentPlayer;

        public Game()
        {
            InitializeComponent();
            PieceList.DataContext = board;
            ScoreP1.DataContext = Player1;
            ScoreP2.DataContext = Player2;
            Init();
        }

        private void Init()
        {
            CurrentPlayer = Player1;
        }

        private void Restart()
        {

        }

        private void Save()
        {

        }

        private void Load()
        {

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(sender);
        }

        private void Board_Click(object sender, MouseButtonEventArgs e)
        {
            ItemsControl i = sender as ItemsControl;
            Point p = e.GetPosition(i);
            int x = (int)(p.X / i.ActualWidth * board.GridWidth);
            int y = (int)(p.Y / i.ActualHeight * board.GridHeight);
            if (board.PosePiece(y, x, CurrentPlayer.Team))
                NextPlayer();
        }

        private void NextPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
            RecalculateScore();
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
    }
}
