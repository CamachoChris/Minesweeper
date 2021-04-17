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

namespace ViewMinesweeper
{
    public partial class MainWindow : Window
    {
        private MinesweeperMap _minesweeperMap;
        private CoordinateGrid[,] playfieldGrids;
        private bool gameRuns;
        private bool mineActivated;
        private bool gameResetted;
        private bool afterGame;

        private DispatcherTimer dispatcherTimer;
        private int seconds;
        int rankType;

        private int usersColumns;
        private int usersRows;
        private int usersMines;

        private const string version = "Version 0.0.7";
        private const string timeOfDevelopment = "April 2021";
        private const string developer = "Grimakar";
        private const int rowsHeight = 27;
        private const int columnsWidth = 27;
        private const double windowDifferenceWidth = 15.5;
        private const double windowDifferenceHeight = 37.7;
        private readonly int gameControlHeight;
        private readonly int menuBarHeight;
        private readonly double screenCenterHorizontal;
        private readonly double screenCenterVertical;

        private readonly BitmapImage bitmapMine;
        private readonly BitmapImage bitmapWarning;

        public MainWindow()
        {
            InitializeComponent();
            this.Show();
            screenCenterHorizontal = this.ActualWidth / 2 + this.Left;
            screenCenterVertical = this.ActualHeight / 2 + this.Top;
            gameControlHeight = (int)gameControlRow.ActualHeight;
            menuBarHeight = (int)menuBarRow.ActualHeight;

            bitmapMine = new BitmapImage();
            bitmapMine.BeginInit();
            bitmapMine.UriSource = new Uri("/images/bomb.png", UriKind.Relative);
            bitmapMine.DecodePixelWidth = 25;
            bitmapMine.EndInit();

            bitmapWarning = new BitmapImage();
            bitmapWarning.BeginInit();
            bitmapWarning.UriSource = new Uri("/images/warning.png", UriKind.Relative);
            bitmapWarning.DecodePixelWidth = 25;
            bitmapWarning.EndInit();

            InitializePlayfield();
        }

        /// <summary>
        /// Resets <c>playfield</c>, gets values from the textboxes and calls <c>MakeGrid()</c>, <c>AddElementsToGrid()</c> und <c>AdjustWindow()</c>.
        /// </summary>
        private void InitializePlayfield()
        {
            this.Hide();
            playfield.Children.Clear();
            playfield.ColumnDefinitions.Clear();
            playfield.RowDefinitions.Clear();

            _minesweeperMap = null;
            playfieldGrids = null;
            dispatcherTimer = null;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            seconds = 0;
            timer.Text = "0";

            EndInititialization();

            mineActivated = false;

            HandleUserInput();
            MakeGrid();
            AddElementsToGrid();
            AdjustWindow();
            mineCounter.Text = $"{usersMines}";
            this.Show();
            gameResetted = true;
            afterGame = false;
        }

        /// <summary>
        /// Part of Initialization. Defines columns and rows for <c>Grid playfield</c>.
        /// </summary>
        /// <remarks>columnsWidth and rowsHeight assign their GridLength.</remarks>
        private void MakeGrid()
        {
            ColumnDefinition[] columnsArray = new ColumnDefinition[usersColumns];
            for (int i = 0; i < columnsArray.Length; ++i)
            {
                columnsArray[i] = new ColumnDefinition();
                columnsArray[i].Width = new GridLength(columnsWidth);
                playfield.ColumnDefinitions.Add(columnsArray[i]);
            }

            RowDefinition[] rowsArray = new RowDefinition[usersRows];
            for (int i = 0; i < rowsArray.Length; ++i)
            {
                rowsArray[i] = new RowDefinition();
                rowsArray[i].Height = new GridLength(rowsHeight);
                playfield.RowDefinitions.Add(rowsArray[i]);
            }
        }

        /// <summary>
        /// Part of Initialization. Defines labels, viewboxes, buttons, grids, events and images.
        /// </summary>
        /// <remarks>Label is Child of Viewbox. Viewbox and Button are Children of Grid. The grids are Children of playfield.</remarks>
        private void AddElementsToGrid()
        {
            playfieldGrids = new CoordinateGrid[usersColumns, usersRows];

            Label[,] labels = new Label[usersColumns, usersRows];
            Viewbox[,] viewboxes = new Viewbox[usersColumns, usersRows];
            Button[,] buttons = new Button[usersColumns, usersRows];

            for (int row = 0; row < usersRows; ++row)
                for (int column = 0; column < usersColumns; ++column)
                {
                    labels[column, row] = new Label();
                    labels[column, row].VerticalAlignment = VerticalAlignment.Center;
                    labels[column, row].HorizontalAlignment = HorizontalAlignment.Center;
                    labels[column, row].HorizontalContentAlignment = HorizontalAlignment.Center;
                    labels[column, row].BorderThickness = new Thickness(0, 0, 0, 0);
                    labels[column, row].BorderBrush = Brushes.DarkCyan;
                    labels[column, row].Background = Brushes.LightBlue;
                    labels[column, row].Height = rowsHeight;
                    labels[column, row].Width = columnsWidth;

                    viewboxes[column, row] = new Viewbox();
                    viewboxes[column, row].Child = labels[column, row];
                    viewboxes[column, row].HorizontalAlignment = HorizontalAlignment.Center;
                    viewboxes[column, row].VerticalAlignment = VerticalAlignment.Center;
                    viewboxes[column, row].MouseDown += ViewBox_Doubleclick;

                    playfieldGrids[column, row] = new CoordinateGrid();
                    playfieldGrids[column, row].Children.Add(viewboxes[column, row]);
                    playfieldGrids[column, row].column = column;
                    playfieldGrids[column, row].row = row;

                    buttons[column, row] = new Button();
                    buttons[column, row].Height = rowsHeight;
                    buttons[column, row].Width = columnsWidth;
                    buttons[column, row].BorderThickness = new Thickness(0, 0, 1, 1);
                    buttons[column, row].BorderBrush = Brushes.LightGray;
                    buttons[column, row].Click += LeftMouseButton_ButtonClick;
                    buttons[column, row].MouseRightButtonDown += RightMouseButton_ButtonClick;
                    buttons[column, row].HorizontalContentAlignment = HorizontalAlignment.Center;

                    playfieldGrids[column, row].Children.Add(buttons[column, row]);

                    Grid.SetColumn(playfieldGrids[column, row], column);
                    Grid.SetRow(playfieldGrids[column, row], row);
                    playfield.Children.Add(playfieldGrids[column, row]);
                }
        }

        /// <summary>
        /// Part of Initialization. Centers the app and resizes the window and the playfield fitting to each other.
        /// </summary>
        private void AdjustWindow()
        {
            int playfieldWidth = usersColumns * columnsWidth;
            int playfieldHeight = usersRows * rowsHeight;
            double windowWidth = playfieldWidth + windowDifferenceWidth;
            double windowHeight = playfieldHeight + gameControlHeight + menuBarHeight + windowDifferenceHeight;
            double windowLeft = screenCenterHorizontal - windowWidth / 2;
            double windowTop = screenCenterVertical - windowHeight / 2;

            this.Width = windowWidth;
            this.Height = windowHeight;
            this.Left = windowLeft;
            this.Top = windowTop;
            playfield.Width = playfieldWidth;
            playfield.Height = playfieldHeight;
        }

        /// <summary>
        /// Returns the control <c>Label</c>, which are sorted at specific positions in <c>playfieldGrids</c>.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private Label GetLabelOfPlayfieldGrid(int column, int row)
        {
            Viewbox vb = (VisualTreeHelper.GetChild(playfieldGrids[column, row], 0) as Viewbox);
            var tmp = VisualTreeHelper.GetChild(vb, 0);
            return (VisualTreeHelper.GetChild(tmp, 0) as Label);
        }

        /// <summary>
        /// Returns the control <c>Button</c>, which are sorted at a specific positions in <c>playfieldGrids</c>.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private Button GetButtonOfPlayfieldGrid(int column, int row)
        {
            return VisualTreeHelper.GetChild(playfieldGrids[column, row], 1) as Button;
        }

        /// <summary>
        /// Checks, if user input is valid and handles it.
        /// </summary>
        private void HandleUserInput()
        {
            if (!int.TryParse(textBoxColumns.Text, out usersColumns))
            {
                MessageBox.Show(this, "Columns input error. Columns set to 10.");
                usersColumns = 10;
                textBoxColumns.Text = "10";
            }
            if (!int.TryParse(textBoxRows.Text, out usersRows))
            {
                MessageBox.Show(this, "Rows input error. Rows set to 10.");
                usersRows = 10;
                textBoxRows.Text = "10";
            }
            if (!int.TryParse(textBoxMines.Text, out usersMines))
            {
                MessageBox.Show(this, "Mines input error. Mines set to 9.");
                usersMines = 9;
                textBoxMines.Text = "9";
            }
            if (usersColumns > 50)
            {
                MessageBox.Show(this, "Too many columns. Maximum is 50.");
                usersColumns = 50;
                textBoxColumns.Text = "50";
            }
            if (usersRows > 30)
            {
                MessageBox.Show(this, "Too many rows. Maximum is 30.");
                usersRows = 30;
                textBoxRows.Text = "30";
            }
            if (usersColumns < 3)
            {
                MessageBox.Show(this, "Not enough columns. Minimum is 5.");
                usersColumns = 5;
                textBoxColumns.Text = "5";
            }
            if (usersRows < 3)
            {
                MessageBox.Show(this, "Not enough rows. Minimum is 3.");
                usersRows = 3;
                textBoxRows.Text = "3";
            }
            if (usersMines > usersColumns * usersRows)
            {
                MessageBox.Show(this, "Too many mines for playfield.");
                usersMines = (usersColumns * usersRows) - 1;
                textBoxMines.Text = string.Format($"{usersMines}");
            }
            if (usersMines < 1)
            {
                MessageBox.Show(this, "Not enough mines. Minimum is 1.");
                usersMines = 1;
                textBoxMines.Text = "1";
            }
        }

    }
}
