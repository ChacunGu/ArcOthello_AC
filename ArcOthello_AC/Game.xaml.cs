using System;
using System.Collections.Generic;
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
    }
}
