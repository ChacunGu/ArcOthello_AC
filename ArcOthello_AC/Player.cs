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
    [Serializable]
    public class Player : INotifyPropertyChanged
    {
        private int score;

        public int Score
        {
            get { return score; }
            set {
                score = value;
                RaisePropertyChanged("Score");
            }
        }

        public Team Team { get; private set; }

        private TimeSpan time;

        public TimeSpan Time
        {
            get { return time; }
            set {
                time = value;
                RaisePropertyChanged("Time");
            }
        }
        

        public Player(Team team)
        {
            this.Team = team;
            this.Score = 0;
            this.Time = new TimeSpan();
        }

        //public void UpdateScore(int newScore)
        //{
        //    score = newScore;
        //}

        public Point Play(Board board)
        {
            return new Point(0, 0);
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
    }
}
