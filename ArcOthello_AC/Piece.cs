using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArcOthello_AC
{
    public class Piece : INotifyPropertyChanged
    {

        public int X { get; private set; }
        public int Y { get; private set; }

        private Team team;

        public Team Team
        {
            get { return team; }
            set {
                team = value;
                RaisePropertyChanged("team");
            }
        }


        public Piece(Team team, int x, int y)
        {
            Team = team;
            this.X = x;
            this.Y = y;
        }

        private void Flip()
        {
            Team = Team == Team.Black ? Team.White : Team.Black;
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
