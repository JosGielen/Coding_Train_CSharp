using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private Cell[] cells = new Cell[9];
        private Rectangle[] rects = new Rectangle[9];
        private string startplayer = "";
        private string currentPlayer = "";
        private int MoveNr = 0;
        Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Rnd.NextDouble() < 0.5)
            {
                startplayer = "X";
            }
            else
            {
                startplayer = "O";
            }
            NewGame();
        }

        private void NewGame()
        {
            for (int I = 0; I < 9; I++)
            {
                cells[I] = new Cell()
                {
                    content = I.ToString(),
                    isUsed = false
                };
            }
            MoveNr = 1;
            //Switch start player each game
            if (startplayer == "X")
            {
                startplayer = "O";
            }
            else
            {
                startplayer = "X";
            }
            if (startplayer == "X")
            {
                currentPlayer = "X";
                CalculateMove();
                TxtMessage.Text = "I started. Please make your move.";
            }
            else if (startplayer == "O")
            {
                currentPlayer = "O";
                TxtMessage.Text = "The Player starts. Please make your move.";
            }
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            canvas1.Children.Clear();
            DrawGrid();
            if (GetResult().Equals("O"))
            {
                Showresult("Congratulations, You Won!");
            }
            else if (GetResult().Equals("X"))
            {
                Showresult("Sorry, I Won!");
            }
            else if (MoveNr > 9)
            {
                Showresult("It ended in a Tie.");
            }
            else
            {
                if (currentPlayer == "X")
                {
                    CalculateMove();
                }
            }
        }

        private void DrawGrid()
        {
            Point center = new Point();
            double size = canvas1.ActualWidth / 5;
            Line l;
            Rectangle r;
            l = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3.0,
                X1 = 25,
                Y1 = 25,
                X2 = canvas1.ActualWidth - 25,
                Y2 = 25
            };
            canvas1.Children.Add(l);
            l = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3.0,
                X1 = 25,
                Y1 = (canvas1.ActualHeight - 50) / 3 + 25,
                X2 = canvas1.ActualWidth - 25,
                Y2 = (canvas1.ActualHeight - 50) / 3 + 25
            };
            canvas1.Children.Add(l);
            l = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3.0,
                X1 = 25,
                Y1 = 2 * (canvas1.ActualHeight - 50) / 3 + 25,
                X2 = canvas1.ActualWidth - 25,
                Y2 = 2 * (canvas1.ActualHeight - 50) / 3 + 25
            };
            canvas1.Children.Add(l);
            l = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3.0,
                X1 = 25,
                Y1 = canvas1.ActualHeight - 25,
                X2 = canvas1.ActualWidth - 25,
                Y2 = canvas1.ActualHeight - 25
            };
            canvas1.Children.Add(l);
            l = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3.0,
                X1 = 25,
                Y1 = 25,
                X2 = 25,
                Y2 = canvas1.ActualHeight - 25
            };
            canvas1.Children.Add(l);
            l = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3.0,
                X1 = (canvas1.ActualWidth - 50) / 3 + 25,
                Y1 = 25,
                X2 = (canvas1.ActualWidth - 50) / 3 + 25,
                Y2 = canvas1.ActualHeight - 25
            };
            canvas1.Children.Add(l);
            l = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3.0,
                X1 = 2 * (canvas1.ActualWidth - 50) / 3 + 25,
                Y1 = 25,
                X2 = 2 * (canvas1.ActualWidth - 50) / 3 + 25,
                Y2 = canvas1.ActualHeight - 25
            };
            canvas1.Children.Add(l);
            l = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3.0,
                X1 = canvas1.ActualWidth - 25,
                Y1 = 25,
                X2 = canvas1.ActualWidth - 25,
                Y2 = canvas1.ActualHeight - 25
            };
            canvas1.Children.Add(l);
            for (int I = 0; I < 9; I++)
            {
                center.X = (I % 3 + 0.5) * (canvas1.ActualWidth - 50) / 3 + 25;
                center.Y = (Math.Floor(I / 3.0) + 0.5) * (canvas1.ActualHeight - 50) / 3 + 25;
                r = new Rectangle()
                {
                    Width = size,
                    Height = size,
                    StrokeThickness = 0.0,
                    Fill = Brushes.White
                };
                r.SetValue(Canvas.LeftProperty, center.X - size / 2);
                r.SetValue(Canvas.TopProperty, center.Y - size / 2);
                rects[I] = r;
                canvas1.Children.Add(r);
                if (cells[I].content.Equals("O"))
                {
                    Ellipse el = new Ellipse()
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 3.0,
                        Width = size,
                        Height = size
                    };
                    el.SetValue(Canvas.LeftProperty, center.X - size / 2);
                    el.SetValue(Canvas.TopProperty, center.Y - size / 2);
                    canvas1.Children.Add(el);
                }
                else if (cells[I].content.Equals("X"))
                {
                    l = new Line()
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 3.0,
                        X1 = center.X - size / 2,
                        Y1 = center.Y - size / 2,
                        X2 = center.X + size / 2,
                        Y2 = center.Y + size / 2
                    };
                    canvas1.Children.Add(l);
                    l = new Line()
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 3.0,
                        X1 = center.X - size / 2,
                        Y1 = center.Y + size / 2,
                        X2 = center.X + size / 2,
                        Y2 = center.Y - size / 2
                    };
                    canvas1.Children.Add(l);
                }
            }
        }

        private void CalculateMove()
        {
            if (currentPlayer != "X") return;
            int corner;
            int selectedIndex = 0;
            if (MoveNr == 1)
            {
                //Always start in a corner;
                corner = Rnd.Next(4);
                if (corner == 0) selectedIndex = 0;
                if (corner == 1) selectedIndex = 2;
                if (corner == 2) selectedIndex = 6;
                if (corner == 3) selectedIndex = 8;
            }
            else if (MoveNr == 2 | MoveNr == 3)
            {
                //try the center.;
                if (!cells[4].isUsed)
                {
                    selectedIndex = 4;
                }
                else
                {
                    //if the player started in the center, use a corner
                    do
                    {
                        corner = Rnd.Next(4);
                        if (corner == 0) selectedIndex = 0;
                        if (corner == 1) selectedIndex = 2;
                        if (corner == 2) selectedIndex = 6;
                        if (corner == 3) selectedIndex = 8;
                    } while (cells[selectedIndex].isUsed);
                }
            }
            else
            {
                //try to make 3 in a row
                if (cells[0].content.Equals("X") & cells[0].Equals(cells[1]) & !cells[2].isUsed)
                {
                    selectedIndex = 2;
                }
                else if (cells[0].content.Equals("X") & cells[0].Equals(cells[2]) & !cells[1].isUsed)
                {
                    selectedIndex = 1;
                }
                else if (cells[1].content.Equals("X") & cells[1].Equals(cells[2]) & !cells[0].isUsed)
                {
                    selectedIndex = 0;
                }
                else if (cells[3].content.Equals("X") & cells[3].Equals(cells[4]) & !cells[5].isUsed)
                {
                    selectedIndex = 5;
                }
                else if (cells[3].content.Equals("X") & cells[3].Equals(cells[5]) & !cells[4].isUsed)
                {
                    selectedIndex = 4;
                }
                else if (cells[4].content.Equals("X") & cells[4].Equals(cells[5]) & !cells[3].isUsed)
                {
                    selectedIndex = 3;
                }
                else if (cells[6].content.Equals("X") & cells[6].Equals(cells[7]) & !cells[8].isUsed)
                {
                    selectedIndex = 8;
                }
                else if (cells[6].content.Equals("X") & cells[6].Equals(cells[8]) & !cells[7].isUsed)
                {
                    selectedIndex = 7;
                }
                else if (cells[7].content.Equals("X") & cells[7].Equals(cells[8]) & !cells[6].isUsed)
                {
                    selectedIndex = 6;
                }
                else if (cells[0].content.Equals("X") & cells[0].Equals(cells[3]) & !cells[6].isUsed)
                {
                    selectedIndex = 6;
                }
                else if (cells[0].content.Equals("X") & cells[0].Equals(cells[6]) & !cells[3].isUsed)
                {
                    selectedIndex = 3;
                }
                else if (cells[3].content.Equals("X") & cells[3].Equals(cells[6]) & !cells[0].isUsed)
                {
                    selectedIndex = 0;
                }
                else if (cells[1].content.Equals("X") & cells[1].Equals(cells[4]) & !cells[7].isUsed)
                {
                    selectedIndex = 7;
                }
                else if (cells[1].content.Equals("X") & cells[1].Equals(cells[7]) & !cells[4].isUsed)
                {
                    selectedIndex = 4;
                }
                else if (cells[4].content.Equals("X") & cells[4].Equals(cells[7]) & !cells[1].isUsed)
                {
                    selectedIndex = 1;
                }
                else if (cells[2].content.Equals("X") & cells[2].Equals(cells[5]) & !cells[8].isUsed)
                {
                    selectedIndex = 8;
                }
                else if (cells[2].content.Equals("X") & cells[2].Equals(cells[8]) & !cells[5].isUsed)
                {
                    selectedIndex = 5;
                }
                else if (cells[5].content.Equals("X") & cells[5].Equals(cells[8]) & !cells[2].isUsed)
                {
                    selectedIndex = 2;
                }
                else if (cells[0].content.Equals("X") & cells[0].Equals(cells[4]) & !cells[8].isUsed)
                {
                    selectedIndex = 8;
                }
                else if (cells[0].content.Equals("X") & cells[0].Equals(cells[8]) & !cells[4].isUsed)
                {
                    selectedIndex = 4;
                }
                else if (cells[4].content.Equals("X") & cells[4].Equals(cells[8]) & !cells[0].isUsed)
                {
                    selectedIndex = 0;
                }
                else if (cells[2].content.Equals("X") & cells[2].Equals(cells[4]) & !cells[6].isUsed)
                {
                    selectedIndex = 6;
                }
                else if (cells[2].content.Equals("X") & cells[2].Equals(cells[6]) & !cells[4].isUsed)
                {
                    selectedIndex = 4;
                }
                else if (cells[4].content.Equals("X") & cells[4].Equals(cells[6]) & !cells[2].isUsed)
                {
                    selectedIndex = 2;
                    //Prevent the player from making 3 in a row
                }
                else if (cells[0].content.Equals("O") & cells[0].Equals(cells[1]) & !cells[2].isUsed)
                {
                    selectedIndex = 2;
                }
                else if (cells[0].content.Equals("O") & cells[0].Equals(cells[2]) & !cells[1].isUsed)
                {
                    selectedIndex = 1;
                }
                else if (cells[1].content.Equals("O") & cells[1].Equals(cells[2]) & !cells[0].isUsed)
                {
                    selectedIndex = 0;
                }
                else if (cells[3].content.Equals("O") & cells[3].Equals(cells[4]) & !cells[5].isUsed)
                {
                    selectedIndex = 5;
                }
                else if (cells[3].content.Equals("O") & cells[3].Equals(cells[5]) & !cells[4].isUsed)
                {
                    selectedIndex = 4;
                }
                else if (cells[4].content.Equals("O") & cells[4].Equals(cells[5]) & !cells[3].isUsed)
                {
                    selectedIndex = 3;
                }
                else if (cells[6].content.Equals("O") & cells[6].Equals(cells[7]) & !cells[8].isUsed)
                {
                    selectedIndex = 8;
                }
                else if (cells[6].content.Equals("O") & cells[6].Equals(cells[8]) & !cells[7].isUsed)
                {
                    selectedIndex = 7;
                }
                else if (cells[7].content.Equals("O") & cells[7].Equals(cells[8]) & !cells[6].isUsed)
                {
                    selectedIndex = 6;
                }
                else if (cells[0].content.Equals("O") & cells[0].Equals(cells[3]) & !cells[6].isUsed)
                {
                    selectedIndex = 6;
                }
                else if (cells[0].content.Equals("O") & cells[0].Equals(cells[6]) & !cells[3].isUsed)
                {
                    selectedIndex = 3;
                }
                else if (cells[3].content.Equals("O") & cells[3].Equals(cells[6]) & !cells[0].isUsed)
                {
                    selectedIndex = 0;
                }
                else if (cells[1].content.Equals("O") & cells[1].Equals(cells[4]) & !cells[7].isUsed)
                {
                    selectedIndex = 7;
                }
                else if (cells[1].content.Equals("O") & cells[1].Equals(cells[7]) & !cells[4].isUsed)
                {
                    selectedIndex = 4;
                }
                else if (cells[4].content.Equals("O") & cells[4].Equals(cells[7]) & !cells[1].isUsed)
                {
                    selectedIndex = 1;
                }
                else if (cells[2].content.Equals("O") & cells[2].Equals(cells[5]) & !cells[8].isUsed)
                {
                    selectedIndex = 8;
                }
                else if (cells[2].content.Equals("O") & cells[2].Equals(cells[8]) & !cells[5].isUsed)
                {
                    selectedIndex = 5;
                }
                else if (cells[5].content.Equals("O") & cells[5].Equals(cells[8]) & !cells[2].isUsed)
                {
                    selectedIndex = 2;
                }
                else if (cells[0].content.Equals("O") & cells[0].Equals(cells[4]) & !cells[8].isUsed)
                {
                    selectedIndex = 8;
                }
                else if (cells[0].content.Equals("O") & cells[0].Equals(cells[8]) & !cells[4].isUsed)
                {
                    selectedIndex = 4;
                }
                else if (cells[4].content.Equals("O") & cells[4].Equals(cells[8]) & !cells[0].isUsed)
                {
                    selectedIndex = 0;
                }
                else if (cells[2].content.Equals("O") & cells[2].Equals(cells[4]) & !cells[6].isUsed)
                {
                    selectedIndex = 6;
                }
                else if (cells[2].content.Equals("O") & cells[2].Equals(cells[6]) & !cells[4].isUsed)
                {
                    selectedIndex = 4;
                }
                else if (cells[4].content.Equals("O") & cells[4].Equals(cells[6]) & !cells[2].isUsed)
                {
                    selectedIndex = 2;
                    //try to make 2 x 2 in a row
                }
                else if (cells[0].content.Equals("X") & !cells[1].isUsed & !cells[2].isUsed)
                {
                    selectedIndex = 2;
                }
                else if (cells[0].content.Equals("X") & !cells[3].isUsed & !cells[6].isUsed)
                {
                    selectedIndex = 6;
                }
                else if (cells[2].content.Equals("X") & !cells[1].isUsed & !cells[0].isUsed)
                {
                    selectedIndex = 0;
                }
                else if (cells[2].content.Equals("X") & !cells[5].isUsed & !cells[8].isUsed)
                {
                    selectedIndex = 8;
                }
                else if (cells[6].content.Equals("X") & !cells[3].isUsed & !cells[0].isUsed)
                {
                    selectedIndex = 0;
                }
                else if (cells[6].content.Equals("X") & !cells[7].isUsed & !cells[8].isUsed)
                {
                    selectedIndex = 8;
                }
                else if (cells[8].content.Equals("X") & !cells[5].isUsed & !cells[2].isUsed)
                {
                    selectedIndex = 2;
                }
                else if (cells[8].content.Equals("X") & !cells[7].isUsed & !cells[6].isUsed)
                {
                    selectedIndex = 6;
                }
                else
                {
                    do
                    {
                        selectedIndex = Rnd.Next(9);
                    } while (cells[selectedIndex].isUsed);
                }
            }
            cells[selectedIndex].content = "X";
            cells[selectedIndex].isUsed = true;
            MoveNr += 1;
            currentPlayer = "O";
        }

        private void Showresult(string result)
        {
            currentPlayer = "";
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            TxtMessage.Text = "Game Over: " + result + " Do you want to play again?";
            btnOK.Visibility = Visibility.Visible;
            btnCANCEL.Visibility = Visibility.Visible;
        }

        private string GetResult()
        {
            if (cells[0].Equals(cells[1]) & cells[0].Equals(cells[2]))
            {
                rects[0].Fill = Brushes.Red;
                rects[1].Fill = Brushes.Red;
                rects[2].Fill = Brushes.Red;
                return cells[0].content;
            }
            else if (cells[3].Equals(cells[4]) & cells[3].Equals(cells[5]))
            {
                rects[3].Fill = Brushes.Red;
                rects[4].Fill = Brushes.Red;
                rects[5].Fill = Brushes.Red;
                return cells[3].content;
            }
            else if (cells[6].Equals(cells[7]) & cells[6].Equals(cells[8]))
            {
                rects[6].Fill = Brushes.Red;
                rects[7].Fill = Brushes.Red;
                rects[8].Fill = Brushes.Red;
                return cells[6].content;
            }
            else if (cells[0].Equals(cells[3]) & cells[0].Equals(cells[6]))
            {
                rects[0].Fill = Brushes.Red;
                rects[3].Fill = Brushes.Red;
                rects[6].Fill = Brushes.Red;
                return cells[0].content;
            }
            else if (cells[1].Equals(cells[4]) & cells[1].Equals(cells[7]))
            {
                rects[1].Fill = Brushes.Red;
                rects[4].Fill = Brushes.Red;
                rects[7].Fill = Brushes.Red;
                return cells[1].content;
            }
            else if (cells[2].Equals(cells[5]) & cells[2].Equals(cells[8]))
            {
                rects[2].Fill = Brushes.Red;
                rects[5].Fill = Brushes.Red;
                rects[8].Fill = Brushes.Red;
                return cells[2].content;
            }
            else if (cells[0].Equals(cells[4]) & cells[0].Equals(cells[8]))
            {
                rects[0].Fill = Brushes.Red;
                rects[4].Fill = Brushes.Red;
                rects[8].Fill = Brushes.Red;
                return cells[0].content;
            }
            else if (cells[2].Equals(cells[4]) & cells[2].Equals(cells[6]))
            {
                rects[2].Fill = Brushes.Red;
                rects[4].Fill = Brushes.Red;
                rects[6].Fill = Brushes.Red;
                return cells[2].content;
            }
            return "T";
        }
        private void Window_MouseUp(Object sender, MouseButtonEventArgs e)
        {
            int playerSelectedIndex = 0;
            if (currentPlayer != "O") return;
            Point pt = e.GetPosition(canvas1);
            if (pt.X < 25 | pt.X > canvas1.ActualWidth - 25) return;
            if (pt.Y < 25 | pt.Y > canvas1.ActualHeight - 25) return;
            int col = (int)(Math.Floor((pt.X - 25) / ((canvas1.ActualWidth - 50) / 3)));
            int row = (int)(Math.Floor((pt.Y - 25) / ((canvas1.ActualHeight - 50) / 3)));
            playerSelectedIndex = 3 * row + col;
            if (!cells[playerSelectedIndex].isUsed)
            {
                cells[playerSelectedIndex].content = "O";
                cells[playerSelectedIndex].isUsed = true;
                MoveNr += 1;
                currentPlayer = "X";
                TxtMessage.Text = "Please make your move.";
            }
            else
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"c:\mywavfile.wav");
                player.Play();
            }
        }

        private void BtnOK_Click(Object sender, RoutedEventArgs e)
        {
            btnOK.Visibility = Visibility.Collapsed;
            btnCANCEL.Visibility = Visibility.Collapsed;
            NewGame();
        }

        private void BtnCANCEL_Click(Object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }

    public class Cell
    {
        public string content = "";
        public bool isUsed = false;

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Cell)) return false;
            return content.Equals(((Cell)obj).content);
        }
    }
}