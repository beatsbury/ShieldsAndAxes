using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;



namespace ShieldsNAxes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event EventHandler PlayerChanged;

        //public RoutedEventArgs e { get; set; }

        public int moves { get; set; }

        public VictoryWindow vic;

        internal enum Players { axesPlayer, shieldsPlayer }
        internal Players player { get; set; }

        internal enum PlayerChoice { singlePlayer, multiPlayer }
        internal PlayerChoice playerChoice;

        //private Player player;

        private List<Button> gameButtons;
        private List<Button> axes;
        private List<Button> shields;
        private List<Button> edges;
        private List<Button> corners;
        private List<Button> diagonalLR;
        private List<Button> diagonalRL;
        private Button center;

        //  abomination!
        private List<List<Button>> rows;
        private List<List<Button>> columns;
        private List<List<Button>> diagonals;

        public MainWindow()
        {
            Thread.Sleep(3000);
            InitializeComponent();
            BtnStart.Visibility = System.Windows.Visibility.Hidden;
        }

        protected virtual void OnPlayerChanged()
        {
            if (PlayerChanged != null)
            {
                PlayerChanged(this, EventArgs.Empty);
            }
        }

        // players choice
        private void btnSinglePlayer_Click(object sender, RoutedEventArgs e)
        {
            playerChoice = PlayerChoice.singlePlayer;
            PlayerMadeAChoice();
        }


        private void btnMultiPlayer_Click(object sender, RoutedEventArgs e)
        {
            playerChoice = PlayerChoice.multiPlayer;
            PlayerMadeAChoice();
        }

        private void PlayerMadeAChoice()
        {
            FlushButtons();
            BtnStart.Visibility = System.Windows.Visibility.Visible;
            btnMultiPlayer.Visibility = System.Windows.Visibility.Hidden;
            btnSinglePlayer.Visibility = System.Windows.Visibility.Hidden;
        }

        private void FlushButtons()
        {
            if (gameButtons != null)
            {
                foreach (var gb in gameButtons)
                {
                    gb.Content = "";
                } 
            }
        }

        //  dealing with starter button click
        private void StartClick(object sender, EventArgs e)
        {
            InitializeButtons();
            axes = new List<Button>();
            shields = new List<Button>();
            player = Players.axesPlayer;
            PlayerChanged += MainWindow_PlayerChanged;
            moves = 0;
        }

        //  event handler after player change
        private void MainWindow_PlayerChanged(object sender, EventArgs e)
        {
            if (player == Players.shieldsPlayer)
            {
                if (playerChoice == PlayerChoice.singlePlayer)
                {
                    ComputerMove();
                }
            };
        }

        //  dealing with game button click
        private void ButtonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            paintFigure(button);
            button.IsEnabled = false;
            if (IsWinner(player))
            {
                Victory(player);
                BoardLock();
            }
            else if (IsBoardFull())
            {
                Stalemate();
                BoardLock();
            }
            else
            {
                moves++;
                SwitchPlayer();
            }
        }

        //  initializing game buttons and clearing all content
        private void InitializeButtons()
        {
            //List<Button> buttons = new List<Button>();
            gameButtons = new List<Button>(9);
            rows = new List<List<Button>>(3);
            columns = new List<List<Button>>(3);
            diagonals = new List<List<Button>>(2);
            edges = new List<Button>(4);
            corners = new List<Button>(4);
            center = new Button();
            diagonalLR = new List<Button>(3);
            diagonalRL = new List<Button>(3);

            var row1 = new List<Button>(3);
            var row2 = new List<Button>(3);
            var row3 = new List<Button>(3);

            var column1 = new List<Button>(3);
            var column2 = new List<Button>(3);
            var column3 = new List<Button>(3);


            row1.Add(cell11);
            row1.Add(cell12);
            row1.Add(cell13);
            row2.Add(cell21);
            row2.Add(cell22);
            row2.Add(cell23);
            row3.Add(cell31);
            row3.Add(cell32);
            row3.Add(cell33);

            column1.Add(cell11);
            column1.Add(cell21);
            column1.Add(cell31);
            column2.Add(cell12);
            column2.Add(cell22);
            column2.Add(cell32);
            column3.Add(cell13);
            column3.Add(cell23);
            column3.Add(cell33);

            diagonalLR.Add(cell11);
            diagonalLR.Add(cell22);
            diagonalLR.Add(cell33);
            diagonalRL.Add(cell13);
            diagonalRL.Add(cell22);
            diagonalRL.Add(cell31);

            diagonals.Add(diagonalLR);
            diagonals.Add(diagonalRL);

            gameButtons.AddRange(row1);
            gameButtons.AddRange(row2);
            gameButtons.AddRange(row3);

            rows.Add(row1);
            rows.Add(row2);
            rows.Add(row3);

            columns.Add(column1);
            columns.Add(column2);
            columns.Add(column3);

            edges.Add(cell12);
            edges.Add(cell21);
            edges.Add(cell23);
            edges.Add(cell32);

            corners.Add(cell11);
            corners.Add(cell13);
            corners.Add(cell31);
            corners.Add(cell33);

            center = cell22;

            foreach (var button in gameButtons)
            {
                button.Content = "";
                button.IsEnabled = true;
            }
            //return buttons;
        }

        //  painting images on clicked buttons
        private void paintFigure(Button button)
        {
            if (player == Players.axesPlayer)
            {
                var axesImage = new Image();
                axesImage.Width = 110;
                Uri axesUri = new Uri("pack://application:,,,/Resources/Axes.png");
                var sourceImage = new BitmapImage(axesUri);
                axesImage.Source = sourceImage;
                button.Content = axesImage;
                //button.ToolTip = "X";
                axes.Add(button);
            }
            else
            {
                var shieldImage = new Image();
                shieldImage.Width = 110;
                Uri shieldUri = new Uri("pack://application:,,,/Resources/Shield.png");
                var sourceImage = new BitmapImage(shieldUri);
                shieldImage.Source = sourceImage;
                button.Content = shieldImage;
                //button.ToolTip = "O";
                shields.Add(button);
            }
        }

        //  switching players
        private void SwitchPlayer()
        {
            if (player == Players.axesPlayer)
            {
                player = Players.shieldsPlayer;
            }
            else
            {
                player = Players.axesPlayer;
            }
            MainWindow_PlayerChanged(this, EventArgs.Empty);
        }

        //  AI logic goes here
        private void ComputerMove()
        {
            if (moves == 1)
            {
                if (axes.Contains(center))
                {
                    ButtonClick(MoveChance(corners), EventArgs.Empty);
                }
                else
                {
                    ButtonClick(center, EventArgs.Empty);
                }
            }
            else if (moves == 2 && shields.Contains(center))
            {
                ButtonClick(MoveChance(edges), EventArgs.Empty);
            }
            else
            {
                if (AIAction(shields))
                {
                    return;
                }
                else if (AIAction(axes))
                {
                    return;
                }
                else
                {
                    ButtonClick(ButtonChance(), EventArgs.Empty);
                    return;
                }
            }
        }

        private bool AIAction(List<Button> figures)
        {
            foreach (var row in rows)
            {
                if (figures.Intersect(row).ToList().Count() == 2 && row.Except(axes).Except(shields).ToList().Count() == 1)
                {
                    ButtonClick(row.Except(axes).Except(shields).ToList()[0], EventArgs.Empty);
                    return true;
                }
            }
            foreach (var column in columns)
            {
                if (figures.Intersect(column).ToList().Count() == 2 && column.Except(axes).Except(shields).ToList().Count() == 1)
                {
                    ButtonClick(column.Except(axes).Except(shields).ToList()[0], EventArgs.Empty);
                    return true;
                }
            }
            foreach (var diagonal in diagonals)
            {
                if (figures.Intersect(diagonal).ToList().Count() == 2 && diagonal.Except(axes).Except(shields).ToList().Count() == 1)
                {
                    ButtonClick(diagonal.Except(axes).Except(shields).ToList()[0], EventArgs.Empty);
                    return true;
                }
            }

            return false;
        }

        private Button MoveChance(List<Button> buttonRange)
        {
            var chance = new Random().Next(100);
            if (chance <= 25)
            {
                return buttonRange[0];
            }
            else if (chance > 25 && chance <= 50)
            {
                return buttonRange[1];
            }
            else if (chance > 50 && chance <= 75)
            {
                return buttonRange[2];
            }
            else
            {
                return buttonRange[3];
            }
        }

        private Button ButtonChance()
        {
            var possibleButtons = (from button in gameButtons
                                   where button.IsEnabled == true
                                   select button).ToList();
            var chance = new Random().Next(possibleButtons.Count());
            return possibleButtons[chance];
        }

        //  celebrating victory
        private void Victory(Players player)
        {
            vic = new VictoryWindow();
            vic.Show();
            this.IsEnabled = false;
            if (player == Players.axesPlayer)
            {
                vic.victoryText.Content = "CONGRATULATIONS BERSERKR";
            }
            else
            {
                vic.victoryText.Content = "CONGRATULATIONS SHIELDBANGR";
            }
            vic.Closed += vic_Closed;
        }

        //  victory window closure event handler
        void vic_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = true;
        }

        //  stalemate situation responce
        private void Stalemate()
        {
            VictoryWindow vic = new VictoryWindow();
            vic.Show();
            this.IsEnabled = false;
            vic.victoryText.Content = "STALEMATE IS UPON US..";
            vic.Closed += vic_Closed;
        }

        //  is the board full?
        private bool IsBoardFull()
        {
            if ((axes.Count + shields.Count) >= 9)
            {
                return true;
            }
            else return false;
        }

        //  lock the board to prevent any interaction beside "START"
        private void BoardLock()
        {
            foreach (var button in gameButtons)
            {
                button.IsEnabled = false;
            }
            BtnStart.Visibility = System.Windows.Visibility.Hidden;
            btnMultiPlayer.Visibility = System.Windows.Visibility.Visible;
            btnSinglePlayer.Visibility = System.Windows.Visibility.Visible;
        }

        //  behold the game winning logic
        private bool IsWinner(Players player)
        {
            if (player == Players.axesPlayer)
            {
                return VictoryCheck(axes);
            }
            else
            {
                return VictoryCheck(shields);
            }
        }

        private bool VictoryCheck(List<Button> figures)
        {
            if (figures.Count >= 3)
            {
                //  full row victory logic
                //var countRows =
                //    from figure in figures
                //    group figure by figure.Name.Substring(4, 1) into rowGroup
                //    select new { rows = rowGroup.Key, rowCount = rowGroup.Count() };
                //foreach (var row in countRows)
                //{
                //    if (row.rowCount >= 3)
                //    {
                //        return true;
                //    }
                //}
                foreach (var row in rows)
                {
                    if (figures.Intersect(row).ToList().Count >= 3)
                    {
                        return true;
                    }
                }

                //  full column victory logic
                //var countColumns =
                //    from figure in figures
                //    group figure by figure.Name.Substring(5, 1) into columnGroup
                //    select new { columns = columnGroup.Key, columnCount = columnGroup.Count() };
                //foreach (var column in countColumns)
                //{
                //    if (column.columnCount >= 3)
                //    {
                //        return true;
                //    }
                //}
                foreach (var column in columns)
                {
                    if (figures.Intersect(column).ToList().Count >= 3)
                    {
                        return true;
                    }
                }

                //  diagonal victory logic (not very bright, not re-usable)
                if (figures.Intersect(diagonalLR).ToList().Count() == 3)
                {
                    return true;
                }
                if (figures.Intersect(diagonalRL).ToList().Count() == 3)
                {
                    return true;
                }
            }
            return false;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        //  handling borderless window dragging, just in case...
        //private void mainWindow_mouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ChangedButton == MouseButton.Left)
        //        this.DragMove();
        //}

    }
}
