using System.Diagnostics.Eventing.Reader;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pong
{
    public partial class MainWindow : Window
    {
        private bool GameStarted = false;
        private bool Playing = false;
        private Rectangle LeftPad;
        private Point LeftPadPosition; //Center of the Left Pad
        private Rectangle RightPad;
        private Point RightPadPosition; //Center of the Right Pad
        private double PadSpeed;
        private Ellipse Puck;
        private Point PuckPosition;  //Center of the Puck
        private Vector PuckDirection;
        private double PuckSpeed;
        private SevenSegmentDisplay LeftScore;
        private SevenSegmentDisplay RightScore;
        private Label MessageLbl;
        private int Counter;
        private SoundPlayer SP;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void Init()
        {
            canvas1.Children.Clear();
            Playing = false;
            //Set both Scores to 0
            LeftScore = new SevenSegmentDisplay(80.0, 130.0, canvas1.ActualWidth / 4 - 40, 30.0);
            LeftScore.Value = 0;
            LeftScore.SegmentColor = Brushes.White;
            LeftScore.IsTilted = false;
            LeftScore.Draw(canvas1);
            RightScore = new SevenSegmentDisplay(80.0, 130.0, 3 * canvas1.ActualWidth / 4 - 40, 30.0);
            RightScore.Value = 0;
            RightScore.SegmentColor = Brushes.White;
            RightScore.IsTilted = false;
            RightScore.Draw(canvas1);
            //Create the left Pad in the start Position
            LeftPadPosition = new Point(80.0, canvas1.ActualHeight / 2);
            LeftPad = new Rectangle()
            {
                Width = 18,
                Height = 100,
                Fill = Brushes.White,
                RadiusX = 8,
                RadiusY = 20
            };
            LeftPad.SetValue(Canvas.LeftProperty, LeftPadPosition.X - 9);
            LeftPad.SetValue(Canvas.TopProperty, LeftPadPosition.Y - 50);
            canvas1.Children.Add(LeftPad);
            //Create the right Pad in the start Position
            RightPadPosition = new Point(canvas1.ActualWidth - 80.0, canvas1.ActualHeight / 2);
            RightPad = new Rectangle()
            {
                Width = 18,
                Height = 100,
                Fill = Brushes.White,
                RadiusX = 8,
                RadiusY = 20
            };
            RightPad.SetValue(Canvas.LeftProperty, RightPadPosition.X - 9);
            RightPad.SetValue(Canvas.TopProperty, RightPadPosition.Y - 50);
            canvas1.Children.Add(RightPad);
            //Create the net line
            Line L = new Line()
            {
                X1 = canvas1.ActualWidth / 2,
                Y1 = 5,
                X2 = canvas1.ActualWidth / 2,
                Y2 = canvas1.ActualHeight - 5,
                Stroke = Brushes.White,
                StrokeDashArray = new DoubleCollection() { 10.0, 5.0 }
            };
            canvas1.Children.Add(L);
            //Create the Puck
            Puck = new Ellipse()
            {
                Width = 20,
                Height = 20,
                Stroke = Brushes.White,
                StrokeThickness = 2.0,
                Fill = Brushes.LightGray
            };
            PuckPosition = new Point(canvas1.ActualWidth / 2, 70);
            Puck.SetValue(Canvas.LeftProperty, PuckPosition.X - 10);
            Puck.SetValue(Canvas.TopProperty, PuckPosition.Y - 10);
            canvas1.Children.Add(Puck);
            PadSpeed = 4.0;
            PuckSpeed = 0.0;
            SP = new SoundPlayer(Environment.CurrentDirectory + "\\ding.wav");
            //Display the Start Message
            MessageLbl = new Label()
            {
                FontSize = 64,
                Background = Brushes.Black,
                Foreground = Brushes.White,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = string.Empty
            };
            canvas1.Children.Add(MessageLbl);
            ShowMessage("Press Space to start the Game.");
            Counter = 0;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            Counter++;
            if (Keyboard.IsKeyDown(Key.Space)) //Start the game with the Space bar
            {
                if (!GameStarted)
                {
                    GameStarted = true;
                    Init();
                }
                if (!Playing)
                {
                    Playing = true;
                    MessageLbl.Content = string.Empty;
                    PuckSpeed = 4.0;
                    if (Rnd.NextDouble() < 0.5)
                    {
                        PuckDirection = new Vector(-1, 0.5);
                    }
                    else
                    {
                        PuckDirection = new Vector(1, 0.5);
                    }
                }
            }
            //Move the paddles
            if (Keyboard.IsKeyDown(Key.A))
            {
                if (LeftPadPosition.Y > 50 + PadSpeed)
                {
                    LeftPadPosition.Y -= PadSpeed;
                }
            }
            if (Keyboard.IsKeyDown(Key.Q))
            {
                if (LeftPadPosition.Y < canvas1.ActualHeight - PadSpeed - 50)
                {
                    LeftPadPosition.Y += PadSpeed;
                }
            }
            if (Keyboard.IsKeyDown(Key.P))
            {
                if (RightPadPosition.Y > 50 + PadSpeed)
                {
                    RightPadPosition.Y -= PadSpeed;
                }
            }
            if (Keyboard.IsKeyDown(Key.M))
            {
                if (RightPadPosition.Y < canvas1.ActualHeight - PadSpeed - 50)
                {
                    RightPadPosition.Y += PadSpeed;
                }
            }
            //Update the Paddle Positions
            LeftPad.SetValue(Canvas.LeftProperty, LeftPadPosition.X - 9);
            LeftPad.SetValue(Canvas.TopProperty, LeftPadPosition.Y - 50);
            RightPad.SetValue(Canvas.LeftProperty, RightPadPosition.X - 9);
            RightPad.SetValue(Canvas.TopProperty, RightPadPosition.Y - 50);
            //Update the Puck Position
            PuckPosition += PuckSpeed * PuckDirection;
            Puck.SetValue(Canvas.LeftProperty, PuckPosition.X - 10);
            Puck.SetValue(Canvas.TopProperty, PuckPosition.Y - 10);
            //Check for Collision between Puck and Left Pad
            if (Math.Abs(PuckPosition.Y - LeftPadPosition.Y) <= 60 && Math.Abs(PuckPosition.X - LeftPadPosition.X) <= 19 + PuckSpeed )
            {
                //Change the puck direction when collided with the pad ends.
                double Offset = PuckPosition.Y - LeftPadPosition.Y;
                PuckDirection.Y += 0.02 * Offset;
                if (Math.Abs(PuckDirection.Y) > 1.3) { PuckDirection.Y = 1.3 * Math.Sign(PuckDirection.Y); }
                PuckDirection.X *= -1;
                PuckPosition.X += 2 * PuckSpeed;
                SP.Play();
            }
            //Check for Collision between Puck and Right Pad
            if (Math.Abs(PuckPosition.Y - RightPadPosition.Y) < 60 && Math.Abs(RightPadPosition.X - PuckPosition.X) <= 19 + PuckSpeed)
            {
                //Change the puck direction when collided with the pad ends.
                double Offset = PuckPosition.Y - RightPadPosition.Y;
                PuckDirection.Y += 0.02 * Offset;
                if (Math.Abs(PuckDirection.Y) > 1.3) { PuckDirection.Y = 1.3 * Math.Sign(PuckDirection.Y); }
                PuckDirection.X *= -1;
                PuckPosition.X -= 2 * PuckSpeed;
                SP.Play();
            }
            //Check for Collision between Puck and Top edge
            if (PuckPosition.Y < 9 + PuckSpeed)
            {
                PuckDirection.Y *= -1;
                PuckPosition.Y += 2 * PuckSpeed;
                SP.Play();
            }
            //Check for Collision between Puck and Bottom edge
            if (PuckPosition.Y > canvas1.ActualHeight - PuckSpeed - 9)
            {
                PuckDirection.Y *= -1;
                PuckPosition.Y -= 2 * PuckSpeed;
                SP.Play();
            }
            //Check for Puck crossing left edge 
            if (PuckPosition.X < 0)
            {
                RightScore.Value++;
                PuckSpeed = 0.0;
                PuckPosition = new Point(canvas1.ActualWidth / 2, 70);
                Puck.SetValue(Canvas.LeftProperty, PuckPosition.X - 10);
                Puck.SetValue(Canvas.TopProperty, PuckPosition.Y - 10);
                Playing = false;
                ShowMessage("Press Space bar to continue");
            }
            //Check for Puck crossing right edge
            if (PuckPosition.X > canvas1.ActualWidth)
            {
                LeftScore.Value++;
                PuckSpeed = 0.0;
                PuckPosition = new Point(canvas1.ActualWidth / 2, 70);
                Puck.SetValue(Canvas.LeftProperty, PuckPosition.X - 10);
                Puck.SetValue(Canvas.TopProperty, PuckPosition.Y - 10);
                Playing = false;
                ShowMessage("Press Space bar to continue");
            }
            //Check for Game End
            if (LeftScore.Value == 3 || RightScore.Value == 3)
            {
                ShowMessage("Game Over.  Press Space to start a new Game.");
                GameStarted = false;
            }
            //Update the PuckSpeed
            if (Playing && Counter % 300 == 0) 
            {
                PuckSpeed += 0.2;
            }
        }

        private void ShowMessage(string message)
        {
            MessageLbl.Content = message;
            double messageLength = 0.45 * MessageLbl.FontSize * message.Length;
            MessageLbl.SetValue(Canvas.LeftProperty, canvas1.ActualWidth / 2 - messageLength / 2);
            MessageLbl.SetValue(Canvas.TopProperty, canvas1.ActualHeight / 2 - 50);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}