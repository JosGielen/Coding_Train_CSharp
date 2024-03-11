using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MineSweeper
{
    internal class Cell
    {
        private int PosX;
        private int PosY;
        private double mySize;
        private MainWindow myParent;
        private bool myGotBomb;
        private bool myRevealed;
        private int myBombNeighbours;
        private Button myBtn;

        public Cell(int X, int Y, bool gotbomb, double size, MainWindow parent)
        {
            PosX = X;
            PosY = Y;
            myGotBomb = gotbomb;
            mySize = size;
            myRevealed = false;
            myBombNeighbours = 0;
            myParent = parent;
            myBtn = new Button()
            {
                Width = mySize,
                Height = mySize,
                Content = "",
                FontSize = 18,
                Padding = new Thickness(0, 0, 0, 0),
                VerticalContentAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold,
                Background = Brushes.SlateBlue
            };
            myBtn.Click += Btn_Click;
        }

        public bool GotBomb
        {
            get { return myGotBomb; }
            set { myGotBomb = value; }
        }

        public bool Revealed
        {
            get { return myRevealed; }
            set { myRevealed = value; }
        }

        public int BombNeighbours
        {
            get { return myBombNeighbours; }
            set { myBombNeighbours = value; }
        }

        public Button Btn
        {
            get { return myBtn; }
            set { myBtn = value; }
        }

        public void Show(Canvas c)
        {
            Btn.SetValue(Canvas.LeftProperty, mySize * PosX);
            Btn.SetValue(Canvas.TopProperty, mySize * PosY);
            c.Children.Add(Btn);
        }

        public void Reveal()
        {
            myBtn.Background = Brushes.LightGray;
            if (myGotBomb)
            {
                //TODO: Show an image of a bomb
                Ellipse el = new Ellipse()
                {
                    Fill = Brushes.OrangeRed,
                    Width = mySize / 2,
                    Height = mySize / 2
                };
                myBtn.Background = Brushes.Yellow;
                myBtn.Content = el;
            }
            else
            {
                if (BombNeighbours == 0)
                {
                    myBtn.Content = "";
                }
                else
                {
                    if (BombNeighbours == 1)
                    {
                        myBtn.Foreground = Brushes.Blue;
                    }
                    else if (BombNeighbours == 2)
                    {
                        myBtn.Foreground = Brushes.Green;
                    }
                    else
                    {
                        myBtn.Foreground = Brushes.Red;
                    }
                    myBtn.Content = BombNeighbours.ToString();
                }
            }
            Revealed = true;
        }

        public void ShowDefused()
        {
            if (myGotBomb)
            {
                //TODO: Show an image of a defused bomb
                Ellipse el = new Ellipse()
                {
                    Fill = Brushes.Lime,
                    Width = mySize / 2,
                    Height = mySize / 2
                };
                myBtn.Background = Brushes.LightGray;
                myBtn.Content = el;
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (myParent.GameOver) return;
            Reveal();
            if (myGotBomb)
            {
                myParent.RevealAll();
            }
            else
            {
                if (BombNeighbours == 0)
                {
                    myParent.RevealNeighbours(PosX, PosY);
                }
                myParent.CheckWinning();
            }
        }
    }
}
