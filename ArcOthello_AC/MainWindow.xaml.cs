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
        #endregion

        #region Variables
        string filename;
        #endregion


        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        #endregion

        #region Commands
        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Start a new game
            filename = null;
            GameInstance.NewGame();
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // Can only save if a file has been selected and the board created
            e.CanExecute = filename != null && GameInstance.Board != null;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Override the current file with a new save
            GameInstance.Save(filename);
        }

        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GameInstance.Board != null;
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Open a save file dialog and save the game
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Save", // Default file name
                DefaultExt = ".oth", // Default file extension
                Filter = "Othello Save files (.oth)|*.oth" // Filter files by extension
            };

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                filename = dlg.FileName;
                GameInstance.Save(filename);
            }
        }
        
        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenSave();
        }

        private void UndoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GameInstance.History != null && GameInstance.History.Count() > 0;
        }

        private void UndoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GameInstance.Undo();
        }
        #endregion

        #region Helper Methods

        private void Exit(object sender, RoutedEventArgs e)
        {
            GameInstance.Exit(sender, e);
        }

        /// <summary>
        /// Show an open dialog and load the game if a file has been selected
        /// </summary>
        public void OpenSave()
        {
            GameInstance.PopupMenu.IsOpen = false;
            GameInstance.PopupEndGame.IsOpen = false;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Save", // Default file name
                DefaultExt = ".oth", // Default file extension
                Filter = "Othello Save files (.oth)|*.oth" // Filter files by extension
            };

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                filename = dlg.FileName;
                GameInstance.Load(filename);
            }
            else if(!GameInstance.IsGameOn)
                GameInstance.PopupMenu.IsOpen = true;
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
