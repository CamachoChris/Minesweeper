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
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel;

namespace Minesweeper
{
    public partial class EnterHighscoreNameWindow : Window
    {
        public int Ranktype;
        public int TimeNeeded;
        public event EventHandler ClosingHandler; //Eventhandler (closing) wird definiert

        public EnterHighscoreNameWindow()
        {
            InitializeComponent();
        }

        private void HandleButtonInput()
        {
            MessageBoxResult result = MessageBox.Show($@"Shall this be your name? {nameBox.Text}","Name correct?",MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Highscore highscore = new Highscore();
                switch (Ranktype)
                {
                    case 0:
                        highscore = HighscoreWindow.LoadEasyHighscoreFile();
                        break;
                    case 1:
                        highscore = HighscoreWindow.LoadMediumHighscoreFile();
                        break;
                    case 2:
                        highscore = HighscoreWindow.LoadHardHighscoreFile();
                        break;
                }
                highscore.Add(nameBox.Text, TimeNeeded);
                switch (Ranktype)
                {
                    case 0:
                        HighscoreWindow.SaveEasyHighscoreFile(highscore);
                        break;
                    case 1:
                        HighscoreWindow.SaveMediumHighscoreFile(highscore);
                        break;
                    case 2:
                        HighscoreWindow.SaveHardHighscoreFile(highscore);
                        break;
                }

                this.ClosingHandler(this, EventArgs.Empty); //Eventhandler (closing) wird aufgerufen.
                this.Close();
            }

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameBox.Text != "")
                HandleButtonInput();
        }

        private void NameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && nameBox.Text != "")
                HandleButtonInput();
        }

        private void EnterHighscoreNameWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {  
        }
    }
}
