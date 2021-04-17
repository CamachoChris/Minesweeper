using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using Minesweeper;
using System.Xml.Serialization;


namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for HighscoreWindow.xaml
    /// </summary>
    public partial class HighscoreWindow : Window
    {
        private const string HIGHSCOREFILEEASY = "hse.dat";
        private const string HIGHSCOREFILEMEDIUM = "hsm.dat";
        private const string HIGHSCOREFILEHARD = "hsh.dat";
        private const int MAXHIGHSCORELENGTH = 10;

        private readonly Highscore highscoreEasy;
        private readonly Highscore highscoreMedium;
        private readonly Highscore highscoreHard;

        private TextBlock[] textBoxMiddle;
        private TextBlock[] textBoxRight;

        public HighscoreWindow(int startScore) : this() 
        {
            if (startScore == 1)
                ShowHighscoreMedium();
            else if (startScore == 2)
                ShowHighscoreHard();
            else
                ShowHighscoreEasy();
        }

        public HighscoreWindow()
        {
            highscoreEasy = LoadEasyHighscoreFile();
            highscoreMedium = LoadMediumHighscoreFile();
            highscoreHard = LoadHardHighscoreFile();

            InitializeComponent();

            InitializeBlankScoreboard();
        }

        private void InitializeBlankScoreboard()
        {
            textBoxMiddle = new TextBlock[MAXHIGHSCORELENGTH];
            textBoxRight = new TextBlock[MAXHIGHSCORELENGTH];

            Label[] labels = new Label[MAXHIGHSCORELENGTH];
            for (int i = 0; i < MAXHIGHSCORELENGTH; i++)
            {
                labels[i] = new Label();

                Grid labelGrid = new Grid();
                labelGrid.Height = 20;

                ColumnDefinition columnLeft = new ColumnDefinition();
                columnLeft.Width = new GridLength(25);
                labelGrid.ColumnDefinitions.Add(columnLeft);

                ColumnDefinition columnMiddle = new ColumnDefinition();
                columnMiddle.Width = new GridLength(140);
                labelGrid.ColumnDefinitions.Add(columnMiddle);

                ColumnDefinition columnRight = new ColumnDefinition();
                columnRight.Width = new GridLength(30);
                labelGrid.ColumnDefinitions.Add(columnRight);

                highscoreStackPanel.Children.Add(labels[i]);
                if (i != 0)
                    labels[i].BorderThickness = new Thickness(0, 1, 0, 0);
                labels[i].BorderBrush = Brushes.Black;
                labels[i].Content = labelGrid;

                TextBox textBoxLeft = new TextBox();
                textBoxLeft.BorderThickness = new Thickness(0);
                Grid.SetColumn(textBoxLeft, 0);
                labelGrid.Children.Add(textBoxLeft);

                textBoxMiddle[i] = new TextBlock();
                Grid.SetColumn(textBoxMiddle[i], 1);
                labelGrid.Children.Add(textBoxMiddle[i]);

                textBoxRight[i] = new TextBlock();
                textBoxRight[i].HorizontalAlignment = HorizontalAlignment.Right;
                Grid.SetColumn(textBoxRight[i], 2);
                labelGrid.Children.Add(textBoxRight[i]);

                textBoxLeft.Margin = new Thickness(0, 2, 0, 0);
                textBoxMiddle[i].Margin = new Thickness(0, 2, 0, 0);
                textBoxRight[i].Margin = new Thickness(0, 2, 0, 0);

                textBoxLeft.Text = string.Format($"{i + 1}.");
            }
        }

        public static Highscore LoadEasyHighscoreFile()
        {
            return HighscoreFileHandler(HIGHSCOREFILEEASY);
        }

        public static Highscore LoadMediumHighscoreFile()
        {
            return HighscoreFileHandler(HIGHSCOREFILEMEDIUM);
        }

        public static Highscore LoadHardHighscoreFile()
        {
            return HighscoreFileHandler(HIGHSCOREFILEHARD);
        }

        private static Highscore HighscoreFileHandler(string filename)
        {
            if (File.Exists(filename))
            {
                return LoadHighscoreFile(filename);
            }
            else
            {
                return CreateHighscoreFile(filename);
            }
        }

        private static Highscore LoadHighscoreFile(string filename)
        {
            Highscore highscore = new Highscore();
            XmlSerializer ser = new XmlSerializer(typeof(Highscore));
            using (Stream s = File.OpenRead(filename))
            {
                highscore = (Highscore)ser.Deserialize(s);
            }
            return highscore;
        }

        private static Highscore CreateHighscoreFile(string filename)
        {
            Highscore highscore = new Highscore();
            SaveHighscoreFile(filename, highscore);

            return highscore;
        }

        public static void SaveEasyHighscoreFile(Highscore highscore)
        {
            SaveHighscoreFile(HIGHSCOREFILEEASY, highscore);
        }

        public static void SaveMediumHighscoreFile(Highscore highscore)
        {
            SaveHighscoreFile(HIGHSCOREFILEMEDIUM, highscore);
        }

        public static void SaveHardHighscoreFile(Highscore highscore)
        {
            SaveHighscoreFile(HIGHSCOREFILEHARD, highscore);
        }

        private static void SaveHighscoreFile(string filename, Highscore highscore)
        {
            XmlSerializer ser = new XmlSerializer(highscore.GetType());
            using (StreamWriter s = new StreamWriter(filename))
            {
                ser.Serialize(s, highscore);
            }
        }


        private void ShowHighscoreEasy()
        {
            easyButton.BorderThickness = new Thickness(0, 0, 0, 2);
            mediumButton.BorderThickness = new Thickness(0);
            hardButton.BorderThickness = new Thickness(0);
            for (int i = 0; i < 10; i++)
            {
                textBoxMiddle[i].Text = highscoreEasy.GetHighscoreList()[i].Name;
                textBoxRight[i].Text = string.Format("{0}", highscoreEasy.GetHighscoreList()[i].Score);
            }
        }

        private void ShowHighscoreMedium()
        {
            easyButton.BorderThickness = new Thickness(0);
            mediumButton.BorderThickness = new Thickness(0, 0, 0, 2);
            hardButton.BorderThickness = new Thickness(0);
            for (int i = 0; i < 10; i++)
            {
                textBoxMiddle[i].Text = highscoreMedium.GetHighscoreList()[i].Name;
                textBoxRight[i].Text = string.Format("{0}", highscoreMedium.GetHighscoreList()[i].Score);
            }

        }

        private void ShowHighscoreHard()
        {
            easyButton.BorderThickness = new Thickness(0);
            mediumButton.BorderThickness = new Thickness(0);
            hardButton.BorderThickness = new Thickness(0, 0, 0, 2);
            for (int i = 0; i < 10; i++)
            {
                textBoxMiddle[i].Text = highscoreHard.GetHighscoreList()[i].Name;
                textBoxRight[i].Text = string.Format("{0}", highscoreHard.GetHighscoreList()[i].Score);
            }
        }

        private void EasyButton_Click(object sender, RoutedEventArgs e)
        {
            ShowHighscoreEasy();
        }

        private void MediumButton_Click(object sender, RoutedEventArgs e)
        {
            ShowHighscoreMedium();
        }

        private void HardButton_Click(object sender, RoutedEventArgs e)
        {
            ShowHighscoreHard();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
