using System;
using System.ComponentModel;

namespace ArcOthello_AC
{
    /// <summary>
    /// Class representing a player with his team, his score and the time he has spent playing
    /// </summary>
    [Serializable]
    public class Player : INotifyPropertyChanged
    {
        #region Properties
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
        #endregion

        #region Constructor
        public Player(Team team)
        {
            this.Team = team;
            this.Score = 0;
            this.Time = new TimeSpan();
        }
        #endregion

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
