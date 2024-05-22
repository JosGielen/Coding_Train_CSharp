using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snakes_and_Ladders
{
    public partial class MainWindow : Window
    {
        private List<Cell> cells;
        private List<Connection> connections;
        private int size = 10;
        private Settings gameSetting;
        private UserInput gameInput;
        private int my_PlayerCount;
        private string my_Player1Name;
        private string my_Player2Name;
        private Image player1;
        private Image player2;
        private double playerWidth;
        private double playerHeight;
        private int CurrentPlayer1Position;
        private int CurrentPlayer2Position;
        private int NewPlayer1Position;
        private int NewPlayer2Position;
        private bool Sliding;
        private int SlidePercentage;
        private Point SlideStart;
        private Point slideEnd;
        private bool ReturnFromEnd1;
        private bool ReturnFromEnd2;
        private bool my_UsePlayerDice;
        private bool my_ExactEnding;

        private bool animating;

        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Create the Cells
            cells = new List<Cell>();
            Size cellSize = new Size(canvas1.ActualWidth / size, canvas1.ActualHeight / size);
            Cell c;
            bool reverse = true;
            for (int row = 0; row < size; row++)
            {
                reverse = !reverse;
                for (int col = 0; col < size; col++)
                {
                    c = new Cell(row, col, cells.Count + 1, cellSize, reverse);
                    c.Draw(canvas1);
                    cells.Add(c);
                }
            }
            Connection con;
            connections = new List<Connection>();
            //Create the Ladders
            con = new Connection(cells[3], cells[13], true);
            connections.Add(con);
            con = new Connection(cells[8], cells[30], true);
            connections.Add(con);
            con = new Connection(cells[19], cells[36], true);
            connections.Add(con);
            con = new Connection(cells[27], cells[83], true);
            connections.Add(con);
            con = new Connection(cells[20], cells[41], true);
            connections.Add(con);
            con = new Connection(cells[35], cells[56], true);
            connections.Add(con);
            con = new Connection(cells[50], cells[72], true);
            connections.Add(con);
            con = new Connection(cells[70], cells[91], true);
            connections.Add(con);
            con = new Connection(cells[79], cells[98], true);
            connections.Add(con);
            //Create the Snakes
            con = new Connection(cells[10], cells[48], false);
            connections.Add(con);
            con = new Connection(cells[18], cells[60], false);
            connections.Add(con);
            con = new Connection(cells[23], cells[85], false);
            connections.Add(con);
            con = new Connection(cells[25], cells[46], false);
            connections.Add(con);
            con = new Connection(cells[40], cells[57], false);
            connections.Add(con);
            con = new Connection(cells[72], cells[92], false);
            connections.Add(con);
            con = new Connection(cells[63], cells[97], false);
            connections.Add(con);
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw(canvas1);
            }
            CurrentPlayer1Position = 1;
            CurrentPlayer2Position = 1;
            NewPlayer1Position = 1; ;
            NewPlayer2Position = 1;
            gameSetting = new Settings()
            {
                Left = Left + Width,
                Top = Top,
            };
            if (gameSetting.ShowDialog() == true)
            {
                my_PlayerCount = gameSetting.PlayerCount;
                my_Player1Name = gameSetting.Player1;
                my_Player2Name = gameSetting.Player2;
                my_UsePlayerDice = gameSetting.UsePlayerDice;
                my_ExactEnding = gameSetting.ExactEnding;
                gameInput = new UserInput(this)
                {
                    Left = Left + Width,
                    Top = Top,
                };
                gameInput.Show();
                //Place 2 player figures on cell 1
                BitmapImage pl1 = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\player1.gif"));
                playerWidth = pl1.PixelWidth;
                playerHeight = pl1.PixelHeight;
                player1 = new Image();
                player1.Source = pl1;
                BitmapImage pl2 = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\player2.gif"));
                player2 = new Image();
                player2.Source = pl2;
                player1.SetValue(Canvas.LeftProperty, cells[0].Center.X - playerWidth / 2 - playerWidth / 2);
                player1.SetValue(Canvas.TopProperty, cells[0].Center.Y - playerHeight / 2);
                canvas1.Children.Add(player1);
                player2.SetValue(Canvas.LeftProperty, cells[0].Center.X - playerWidth / 2 + playerWidth / 2);
                player2.SetValue(Canvas.TopProperty, cells[0].Center.Y - playerHeight / 2);
                canvas1.Children.Add(player2);
                animating = false;
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            else
            {
                Environment.Exit(0);
            }
        }

        public int PlayerCount
        {
            get { return my_PlayerCount; }
        }

        public string Player1
        {
            get { return my_Player1Name; }
        }

        public string Player2
        {
            get { return my_Player2Name; }
        }

        public bool UsePlayerDice
        {
            get { return my_UsePlayerDice; }
        }

        public void MovePlayer(int PlayerNr, int amount)
        {
            if (PlayerNr == 1)
            {
                NewPlayer1Position = CurrentPlayer1Position + amount;
            }
            else
            {
                NewPlayer2Position = CurrentPlayer2Position + amount;
            }
            animating = true;
        }

        private int CheckConnections(int pos)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].Ladder)
                {
                    if (connections[i].Start.Nr == pos) { return connections[i].End.Nr; }
                }
                else
                {
                    if (connections[i].End.Nr == pos) { return connections[i].Start.Nr; }
                }
            }
            return 0;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if (!animating) { return; }
            if (CurrentPlayer1Position != NewPlayer1Position)
            {
                if (Sliding)
                {
                    SlidePercentage += 20;
                    if (SlidePercentage < 100)
                    {
                        //Draw the player image along the slide
                        Point currentPos = SlideStart + SlidePercentage / 100.0 * (slideEnd - SlideStart);
                        player1.SetValue(Canvas.LeftProperty, currentPos.X - playerWidth / 2);
                        player1.SetValue(Canvas.TopProperty, currentPos.Y - playerHeight / 2);
                        Thread.Sleep(500);
                    }
                    else
                    {
                        CurrentPlayer1Position = NewPlayer1Position;
                        Sliding = false;
                        PlacePlayers();
                        return;
                    }
                }
                else
                {
                    if (ReturnFromEnd1)
                    {
                        CurrentPlayer1Position--;
                    }
                    else
                    {
                        CurrentPlayer1Position++;
                    }
                    if (CurrentPlayer1Position == 100)
                    {
                        ReturnFromEnd1 = true;
                        if (my_ExactEnding)
                        {
                            NewPlayer1Position = 200 - NewPlayer1Position;
                        }
                        else
                        {
                            //Player 1 won.
                            GameEnded(1);
                            animating=false;
                            return;
                        }
                    }
                    PlacePlayers();
                }
            }
            if (CurrentPlayer1Position == NewPlayer1Position)
            {
                if (CurrentPlayer1Position == 100)
                {
                    //Player 1 won.
                    GameEnded(1);
                    animating = false;
                    return;
                }
                ReturnFromEnd1 = false;
                int ChangedPosition = CheckConnections(NewPlayer1Position);
                if (ChangedPosition > 0)
                {
                    Sliding = true;
                    NewPlayer1Position = ChangedPosition;
                    SlideStart = cells[CurrentPlayer1Position - 1].Center;
                    slideEnd = cells[NewPlayer1Position - 1].Center;
                    SlidePercentage = 0;
                }
            }

            if (CurrentPlayer2Position != NewPlayer2Position)
            {
                if (Sliding)
                {
                    SlidePercentage += 20;
                    if (SlidePercentage < 100)
                    {
                        //Draw the player image along the slide
                        Point currentPos = SlideStart + SlidePercentage / 100.0 * (slideEnd - SlideStart);
                        player2.SetValue(Canvas.LeftProperty, currentPos.X - playerWidth / 2);
                        player2.SetValue(Canvas.TopProperty, currentPos.Y - playerHeight / 2);
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        CurrentPlayer2Position = NewPlayer2Position;
                        Sliding = false;
                        PlacePlayers();
                        return;
                    }
                }
                else
                {
                    if (ReturnFromEnd2)
                    {
                        CurrentPlayer2Position--;
                    }
                    else
                    {
                        CurrentPlayer2Position++;
                    }
                    if (CurrentPlayer2Position == 100)
                    {
                        ReturnFromEnd2 = true;
                        if (my_ExactEnding)
                        {
                            NewPlayer2Position = 200 - NewPlayer2Position;
                        }
                        else
                        {
                            //Player 2 won.
                            GameEnded(2);
                        }
                    }
                    PlacePlayers();
                }
            }
            if (CurrentPlayer2Position == NewPlayer2Position)
            {
                if (CurrentPlayer2Position == 100)
                {
                    //Player 2 won.
                    GameEnded(2);
                    animating = false;
                    return;
                }
                ReturnFromEnd2 = false;
                int ChangedPosition = CheckConnections(NewPlayer2Position);
                if (ChangedPosition > 0)
                {
                    Sliding = true;
                    NewPlayer2Position = ChangedPosition;
                    SlideStart = cells[CurrentPlayer2Position - 1].Center;
                    slideEnd = cells[NewPlayer2Position - 1].Center;
                    SlidePercentage = 0;
                }
            }
            if (CurrentPlayer1Position == NewPlayer1Position && CurrentPlayer2Position == NewPlayer2Position)
            {
                animating = false;
                gameInput.GetNextMove();
            }
        }

        private void GameEnded(int playerNr)
        {
            PlacePlayers();
            gameInput.Close();
            string player;
            if (playerNr == 1)
            {
                player = my_Player1Name;
            }
            else
            {
                player=my_Player2Name;
            }
            TextBox txt = new TextBox()
            {
                Width = canvas1.ActualWidth,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Text = "Congratulations " + player + ". You won this game.",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Background = Brushes.Yellow
            };
            txt.SetValue(Canvas.LeftProperty, 0.0);
            txt.SetValue(Canvas.TopProperty, canvas1.ActualHeight / 2 - 15);
            canvas1.Children.Add(txt);

        }

        public void PlacePlayers()
        {
            double Xoffset;
            if (CurrentPlayer1Position == CurrentPlayer2Position)
            {
                Xoffset = playerWidth / 2;
            }
            else
            {
                Xoffset = 0;
            }
            player1.SetValue(Canvas.LeftProperty, cells[CurrentPlayer1Position - 1].Center.X - playerWidth / 2 - Xoffset);
            player1.SetValue(Canvas.TopProperty, cells[CurrentPlayer1Position - 1].Center.Y - playerHeight / 2);
            player2.SetValue(Canvas.LeftProperty, cells[CurrentPlayer2Position - 1].Center.X - playerWidth / 2 + Xoffset);
            player2.SetValue(Canvas.TopProperty, cells[CurrentPlayer2Position - 1].Center.Y - playerHeight / 2);
            Thread.Sleep(500);
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (gameSetting != null)
            {
                gameSetting.Left = Left + Width;
                gameSetting.Top = Top;
            }
            if (gameInput != null)
            {
                gameInput.Left = Left + Width;
                gameInput.Top = Top;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}