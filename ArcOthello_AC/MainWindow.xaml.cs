using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        #region Properties
        private int scoreP1;

        public int ScoreP1
        {
            get { return scoreP1; }
            set {
                scoreP1 = value;
                RaisePropertyChanged("scoreP1");
            }
        }

        private int scoreP2;

        public int ScoreP2
        {
            get { return scoreP2; }
            set
            {
                scoreP2 = value;
                RaisePropertyChanged("scoreP2");
            }
        }

        public Game GameInstance { get; set; }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Init();
            
        }

        private void Init()
        {
            GameInstance = new Game();
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
