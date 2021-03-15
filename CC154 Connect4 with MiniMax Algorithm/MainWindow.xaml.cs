using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Connect4
{
    public partial class MainWindow : Window
    {
        //Program members
        private bool AppLoaded = false;
        private bool GameStarted = false;
        private int RowNum;
        private int ColNum;
        private double CelWidth = 0.0;
        private double CelHeight = 0.0;
        private Marker[] Markers;
        private int PlayerCount;
        private int CurrentPlayer;
        private int StartingPlayer;
        private string Player1;
        private string Player2;
        private int myCol = 0;
        private GameMode myGameMode;
        private int myDepth;
        private readonly Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        #region "Window Events"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Default start with 1 player
            PlayerCount = 1;
            Player1 = "Player";
            Player2 = "Computer";
            //Default start with 6 rows and 7 columns in normal difficulty mode
            RowNum = 6;
            ColNum = 7;
            myGameMode = GameMode.Normal;
            myDepth = 3;
            Markers = new Marker[RowNum * ColNum];
            for (int I = 0; I < ColNum * RowNum; I++)
            {
                Markers[I] = new Marker();
            }
            Width = 60 * ColNum;
            Height = 60 * RowNum + 75;
            DrawGame();
            //Show a settings dialog to allow modifications
            ShowSettings();
            //Determine who does the first move.
            if (Rnd.Next(100) < 50)
            {
                StartingPlayer = 1;
            }
            else
            {
                StartingPlayer = 2;
            }
            CurrentPlayer = StartingPlayer;
            GameStarted = true;
            AppLoaded = true;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AppLoaded == true) DrawGame();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Canvas1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int Col = 0;
            int FreeIndex = -1;
            //Check if the game is started
            if (!GameStarted)
            {
                e.Handled = true;
                return;
            }
            //Determine the column where the player clicked
            for (int I = 0; I < Markers.Length; I++)
            {
                if (Markers[I].Shape.IsMouseOver)
                {
                    Col = Markers[I].Col;
                    FreeIndex = GetFreeIndex(Col);
                }
            }
            if (FreeIndex >= 0)
            {
                if (PlayerCount == 1)
                {
                    //Set the player marker
                    Markers[FreeIndex].PlayerNr = 1;
                    if (CheckVictory()) return;
                    //Get the computer move
                    Dispatcher.Invoke(GetBestMove, DispatcherPriority.ApplicationIdle);
                    FreeIndex = GetFreeIndex(myCol);
                    if (FreeIndex >= 0)
                    {
                        Markers[FreeIndex].PlayerNr = 2;
                    }
                    else
                    {
                        //The computer made an invalid move!!!
                    }
                    if (CheckVictory()) return;
                }
                else if (PlayerCount == 2)
                {
                    //Set the player marker
                    Markers[FreeIndex].PlayerNr = CurrentPlayer;
                    if (CheckVictory()) return;
                    //Switch to the next player
                    if (CurrentPlayer == 1)
                    {
                        TxtStatus.Text = Player2 + " please make your move.";
                        CurrentPlayer = 2;
                    }
                    else
                    {
                        TxtStatus.Text = Player1 + " please make your move.";
                        CurrentPlayer = 1;
                    }
                }
            }
            else
            {
                //The player made an invalid move!!!
            }
        }

        #endregion

        #region "Menu"

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            //TODO Save the current game state
        }

        private void MenuLoad_Click(object sender, RoutedEventArgs e)
        {
            //TODO Load a saved game
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MnuSettings_Click(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void MenuStartNew_Click(object sender, RoutedEventArgs e)
        {
            //Clear all markers
            for (int I = 0; I < ColNum * RowNum; I++)
            {
                Markers[I] = new Marker();
            }
            DrawGame();
            //The other player will now start the game
            if (StartingPlayer == 1)
            {
                StartingPlayer = 2;
            }
            else
            {
                StartingPlayer = 1;
            }
            CurrentPlayer = StartingPlayer;
            if (PlayerCount == 1)
            {
                if (CurrentPlayer == 2)
                {
                    //The Computer starts in a random column;
                    myCol = Rnd.Next(ColNum);
                    Markers[GetFreeIndex(myCol)].PlayerNr = 2;
                    CurrentPlayer = 1;
                }
                TxtStatus.Text = Player1 + " Please make your move.";
            }
            else
            {
                if (CurrentPlayer == 1)
                {
                    TxtStatus.Text = Player1 + " please make your move.";
                }
                else
                {
                    TxtStatus.Text = Player2 + " please make your move.";
                }
            }
            GameStarted = true;
        }

        private void MenuHint_Click(object sender, RoutedEventArgs e)
        {
            //TODO Display a hint.
        }

        private void MenuAnalyse_Click(object sender, RoutedEventArgs e)
        {
            //TODO Analyse the current game state.
        }

        #endregion

        #region "Business Methods"

        private void GetBestMove()
        {
            int FreeIndex;
            int score;
            int bestscore = int.MinValue;
            int bestcol = -1;
            Cursor = Cursors.Wait;
            for (int I = 0; I < ColNum; I++)
            {
                FreeIndex = GetFreeIndex(I);
                if (FreeIndex > 0)
                {
                    Markers[FreeIndex].PlayerNr = 2;
                    score = Minimax(myDepth, false);
                    if (score > bestscore)
                    {
                        bestscore = score;
                        bestcol = I;
                    }
                    Markers[FreeIndex].PlayerNr = 0;
                }
            }
            myCol = bestcol;
            Cursor = Cursors.Arrow;
        }

        private int Minimax(int depth, bool maximizing)
        {
            int FreeIndex;
            int Score;
            int Bestscore;
            int validmoves = 0;
            int result = 0;
            if (depth == 0 | Check4(1) > 0 | Check4(2) > 0)
            {
                //return the heuristic value of the game;
                result = 5000 * Check4(2) + 100 * Check3(2) + 10 * Check2(2) - 3000 * Check4(1) - 100 * Check3(1) - 10 * Check2(1);
                return result;
            }
            if (maximizing)
            {
                Bestscore = int.MinValue;
                for (int I = 0; I < ColNum; I++)
                {
                    FreeIndex = GetFreeIndex(I);
                    if (FreeIndex >= 0)
                    {
                        Markers[FreeIndex].PlayerNr = 2;  //Computer = maximizing;
                        Score = Minimax(depth - 1, false);
                        if (Score > Bestscore)
                        {
                            Bestscore = Score;
                        }
                        validmoves += 1;
                        Markers[FreeIndex].PlayerNr = 0;
                    }
                }
                if (validmoves > 0)
                {
                    result = Bestscore;
                }
                return result;
            }
            else
            {
                Bestscore = int.MaxValue;
                for (int I = 0; I < ColNum; I++)
                {
                    FreeIndex = GetFreeIndex(I);
                    if (FreeIndex >= 0)
                    {
                        Markers[FreeIndex].PlayerNr = 1; //Player = minimizing;
                        Score = Minimax(depth - 1, true);
                        if (Score < Bestscore)
                        {
                            Bestscore = Score;
                        }
                        validmoves += 1;
                        Markers[FreeIndex].PlayerNr = 0;
                    }
                }
                if (validmoves > 0)
                {
                    result = Bestscore;
                }
                return result;
            }
        }

        //'' <summary>
        //'' Check if one of the players made 4 in a Row
        //'' </summary>
        private bool CheckVictory()
        {
            int FreeCells = 0;
            for (int I = 0; I < Markers.Length; I++)
            {
                if (Markers[I].PlayerNr == 0) FreeCells += 1;
            }
            if (FreeCells == 0)
            {
                TxtStatus.Text = "This game is a draw.";
                GameStarted = false;
                return true;
            }
            if (PlayerCount == 1)
            {
                if (Check4(1) > 0)
                {
                    TxtStatus.Text = "Congratulations, You won!";
                    GameStarted = false;
                    return true;
                }
                if (Check4(2) > 0)
                {
                    TxtStatus.Text = "Sorry, You lost!";
                    GameStarted = false;
                    return true;
                }
            }
            if (PlayerCount == 2)
            {
                if (Check4(1) > 0)
                {
                    TxtStatus.Text = "Congratulations " + Player1.ToString() + ", You won!";
                    GameStarted = false;
                    return true;
                }
                if (Check4(2) > 0)
                {
                    TxtStatus.Text = "Congratulations " + Player2.ToString() + ", You won!";
                    GameStarted = false;
                    return true;
                }
            }
            return false;
        }

        //'' <summary>
        //'' Check for 4 in a Row
        //'' </summary>
        //'' <param name="player">The player number (1 or 2)</param>
        //'' <returns>The number of times 4 in a row was found</returns>
        private int Check4(int player)
        {
            int row;
            int col;
            int aantal = 0;
            for (int I = 0; I < Markers.Length; I++)
            {
                if (Markers[I].PlayerNr != player) continue;
                row = GetRow(I);
                col = GetCol(I);
                if (col > 2)
                {
                    //Check LEFT
                    if (Markers[GetIndex(row, col - 1)].PlayerNr == player && Markers[GetIndex(row, col - 2)].PlayerNr == player && Markers[GetIndex(row, col - 3)].PlayerNr == player) aantal += 1;
                    if (row > 2)
                    {
                        //Check LEFT UP
                        if (Markers[GetIndex(row - 1, col - 1)].PlayerNr == player && Markers[GetIndex(row - 2, col - 2)].PlayerNr == player && Markers[GetIndex(row - 3, col - 3)].PlayerNr == player) aantal += 1;
                    }
                }
                if (row > 2)
                {
                    //Check UP
                    if (Markers[GetIndex(row - 1, col)].PlayerNr == player && Markers[GetIndex(row - 2, col)].PlayerNr == player && Markers[GetIndex(row - 3, col)].PlayerNr == player) aantal += 1;
                }
                if (col < ColNum - 3)
                {
                    //Check RIGHT
                    if (Markers[GetIndex(row, col + 1)].PlayerNr == player && Markers[GetIndex(row, col + 2)].PlayerNr == player && Markers[GetIndex(row, col + 3)].PlayerNr == player) aantal += 1;
                    if (row > 2)
                    {
                        //Check RIGHT UP
                        if (Markers[GetIndex(row - 1, col + 1)].PlayerNr == player && Markers[GetIndex(row - 2, col + 2)].PlayerNr == player && Markers[GetIndex(row - 3, col + 3)].PlayerNr == player) aantal += 1;
                    }
                }
            }
            return aantal;
        }

        //'' <summary>
        //'' Check for 3 in a Row with the 4th place empty
        //'' </summary>
        //'' <param name="player">The player number (1 or 2)</param>
        //'' <returns>The number of times 3 in a row was found</returns>
        private int Check3(int player)
        {
            int row;
            int col;
            int aantal = 0;
            for (int I = 0; I < Markers.Length; I++)
            {
                if (Markers[I].PlayerNr != player) continue;
                row = GetRow(I);
                col = GetCol(I);
                if (col > 2)
                {
                    //Check LEFT
                    if (Markers[GetIndex(row, col - 1)].PlayerNr == player && Markers[GetIndex(row, col - 2)].PlayerNr == player && Markers[GetIndex(row, col - 3)].PlayerNr == 0) aantal += 1;
                    if (row > 2)
                    {
                        //Check LEFT UP
                        if (Markers[GetIndex(row - 1, col - 1)].PlayerNr == player && Markers[GetIndex(row - 2, col - 2)].PlayerNr == player && Markers[GetIndex(row - 3, col - 3)].PlayerNr == 0) aantal += 1;
                    }
                }
                if (row > 2)
                {
                    //Check UP
                    if (Markers[GetIndex(row - 1, col)].PlayerNr == player && Markers[GetIndex(row - 2, col)].PlayerNr == player && Markers[GetIndex(row - 3, col)].PlayerNr == 0) aantal += 1;
                }
                if (col < ColNum - 3)
                {
                    //Check RIGHT
                    if (Markers[GetIndex(row, col + 1)].PlayerNr == player && Markers[GetIndex(row, col + 2)].PlayerNr == player && Markers[GetIndex(row, col + 3)].PlayerNr == 0) aantal += 1;
                    if (row > 2)
                    {
                        //Check RIGHT UP
                        if (Markers[GetIndex(row - 1, col + 1)].PlayerNr == player && Markers[GetIndex(row - 2, col + 2)].PlayerNr == player && Markers[GetIndex(row - 3, col + 3)].PlayerNr == 0) aantal += 1;
                    }
                }
            }
            return aantal;
        }

        //'' <summary>
        //'' Check for 2 in a Row with the 3rd and 4th place empty
        //'' </summary>
        //'' <param name="player">The player number (1 or 2)</param>
        //'' <returns>The number of times 2 in a row was found</returns>
        private int Check2(int player)
        {
            int row;
            int col;
            int aantal = 0;
            for (int I = 0; I < Markers.Length; I++)
            {
                if (Markers[I].PlayerNr != player) continue;
                row = GetRow(I);
                col = GetCol(I);
                if (col > 2)
                {
                    //Check LEFT
                    if (Markers[GetIndex(row, col - 1)].PlayerNr == player && Markers[GetIndex(row, col - 2)].PlayerNr == 0 && Markers[GetIndex(row, col - 3)].PlayerNr == 0) aantal += 1;
                    if (row > 2)
                    {
                        //Check LEFT UP
                        if (Markers[GetIndex(row - 1, col - 1)].PlayerNr == player && Markers[GetIndex(row - 2, col - 2)].PlayerNr == 0 && Markers[GetIndex(row - 3, col - 3)].PlayerNr == 0) aantal += 1;
                    }
                }
                if (row > 2)
                {
                    //Check UP
                    if (Markers[GetIndex(row - 1, col)].PlayerNr == player && Markers[GetIndex(row - 2, col)].PlayerNr == 0 && Markers[GetIndex(row - 3, col)].PlayerNr == 0) aantal += 1;
                }
                if (col < ColNum - 3)
                {
                    //Check RIGHT
                    if (Markers[GetIndex(row, col + 1)].PlayerNr == player && Markers[GetIndex(row, col + 2)].PlayerNr == 0 && Markers[GetIndex(row, col + 3)].PlayerNr == 0) aantal += 1;
                    if (row > 2)
                    {
                        //Check RIGHT UP
                        if (Markers[GetIndex(row - 1, col + 1)].PlayerNr == player && Markers[GetIndex(row - 2, col + 2)].PlayerNr == 0 && Markers[GetIndex(row - 3, col + 3)].PlayerNr == 0) aantal += 1;
                    }
                }
            }
            return aantal;
        }

        #endregion

        #region "Utility methods"

        private void ShowSettings()
        {
            GameSettings gs;
            Settings gameData = new Settings()
            {
                PlayerCount = PlayerCount,
                PlayerName1 = Player1,
                PlayerName2 = Player2,
                GameSize = new Size(ColNum, RowNum),
                GameMode = myGameMode
            };
            gs = new GameSettings(gameData, this);
            gs.Left = Left + ActualWidth;
            gs.Top = Top;
            gs.ShowDialog();
            if (gs.DialogResult == true)
            {
                PlayerCount = gs.Playeraantal;
                Player1 = gs.Player1;
                Player2 = gs.Player2;
                RowNum = (int)(gs.GameSize.Height);
                ColNum = (int)(gs.GameSize.Width);
                myGameMode = gs.GameMode;
                if (myGameMode == GameMode.Easy) myDepth = 2;
                if (myGameMode == GameMode.Normal) myDepth = 3;
                if (myGameMode == GameMode.Hard) myDepth = 5;
                Markers = new Marker[RowNum * ColNum];
                for (int I = 0; I < ColNum * RowNum; I++)
                {
                    Markers[I] = new Marker();
                }
                Width = 60 * ColNum;
                Height = 60 * RowNum + 75;
                GameStarted = true;
                if (PlayerCount == 1)
                {
                    if (CurrentPlayer == 2)
                    {
                        //The Computer starts in a random column;
                        myCol = Rnd.Next(ColNum);
                        Markers[GetFreeIndex(myCol)].PlayerNr = 2;
                        CurrentPlayer = 1;
                    }
                    TxtStatus.Text = Player1 + " Please make your move.";
                }
                else
                {
                    if (CurrentPlayer == 1)
                    {
                        TxtStatus.Text = Player1 + " please make your move.";
                    }
                    else
                    {
                        TxtStatus.Text = Player2 + " please make your move.";
                    }
                }
            }
            DrawGame();
        }

        private void DrawGame()
        {
            Line gridLine;
            double VeldWidth;
            double VeldHeight;
            VeldWidth = Canvas1.ActualWidth;
            VeldHeight = Canvas1.ActualHeight;
            CelWidth = VeldWidth / ColNum;
            CelHeight = VeldHeight / RowNum;
            //Adjust the Markers
            Canvas1.Children.Clear();
            for (int I = 0; I < ColNum * RowNum; I++)
            {
                Markers[I].CelWidth = 0.9 * CelWidth;
                Markers[I].CelHeight = 0.9 * CelHeight;
                Markers[I].Row = (int)Math.Floor(I / (double)ColNum);
                Markers[I].Col = I % ColNum;
            }
            //Draw border of the canvas
            gridLine = new Line
            {
                Stroke = Brushes.LightGray,
                X1 = 1,
                Y1 = 0,
                X2 = VeldWidth,
                Y2 = 0
            };
            Canvas1.Children.Add(gridLine);
            gridLine = new Line
            {
                Stroke = Brushes.LightGray,
                X1 = VeldWidth,
                Y1 = 0,
                X2 = VeldWidth,
                Y2 = VeldHeight
            };
            Canvas1.Children.Add(gridLine);
            gridLine = new Line
            {
                Stroke = Brushes.LightGray,
                X1 = 1,
                Y1 = 0,
                X2 = 1,
                Y2 = VeldHeight
            };
            Canvas1.Children.Add(gridLine);
            gridLine = new Line
            {
                Stroke = Brushes.LightGray,
                X1 = 1,
                Y1 = VeldHeight,
                X2 = VeldWidth,
                Y2 = VeldHeight
            };
            Canvas1.Children.Add(gridLine);
            //Draw Vertical gridlines
            for (int I = 0; I < ColNum - 1; I++)
            {
                gridLine = new Line
                {
                    Stroke = Brushes.LightGray,
                    X1 = CelWidth * (I + 1),
                    Y1 = 0,
                    X2 = CelWidth * (I + 1),
                    Y2 = VeldHeight
                };
                Canvas1.Children.Add(gridLine);
            }
            //Draw Horizontal gridlines
            for (int I = 0; I < RowNum - 1; I++)
            {
                gridLine = new Line
                {
                    Stroke = Brushes.LightGray,
                    X1 = 0,
                    Y1 = CelHeight * (I + 1),
                    X2 = VeldWidth,
                    Y2 = CelHeight * (I + 1)
                };
                Canvas1.Children.Add(gridLine);
            }
            //Draw all markers
            for (int I = 0; I < ColNum * RowNum; I++)
            {
                double X = I % ColNum;
                double Y = Math.Truncate(I / (double)ColNum);
                Markers[I].Draw(Canvas1, (X + 0.05) * CelWidth, (Y + 0.05) * CelHeight);
            }
        }

        private int GetFreeIndex(int Col)
        {
            int index;
            if (Col < 0 | Col >= ColNum) return -1;
            for (int I = RowNum - 1; I >= 0; I--)
            {
                index = ColNum * I + Col;
                if (Markers[index].PlayerNr == 0) return index;
            }
            return -1;
        }

        private int GetRow(int Index)
        {
            return (int)(Math.Floor(Index / (double)ColNum));
        }

        private int GetCol(int Index)
        {
            return Index % ColNum;
        }

        private int GetIndex(int row, int col)
        {
            return row * ColNum + col;
        }

        #endregion 

    }
}
