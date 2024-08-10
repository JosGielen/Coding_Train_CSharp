using System.IO.Packaging;
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

namespace Horizontal_Directional_Drilling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Obstacle[] rocks;
        private Rectangle begin;
        private Rectangle target;
        private Point drillPos;
        private double drillAngle;
        private int drillBias;
        private double drillspeed;
        private Line[] drill;
        private Polyline cable;
        private bool AppStarted;
        private bool Restart;

        private static Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Restart = false;
            Init();
        }

        private void Init()
        {
            canvas1.Children.Clear();
            AppStarted = false;
            drillBias = 1;
            double X, Y;
            double W, H;
            rocks = new Obstacle[20];
            //Create a river obstacle
            X = canvas1.ActualWidth / 2;
            Y = 0.0;
            W = canvas1.ActualWidth / 2;
            H = canvas1.ActualWidth / 4;
            rocks[0] = new Obstacle(new Point(X, Y), W, H)
            {
                Color = Brushes.Blue
            };
            rocks[0].Draw(canvas1);
            //Create rock obstacles (below the river depth)
            for (int I = 1; I < 18; I++)
            {
                W = 50 * Rnd.NextDouble() + 40;
                H = 50 * Rnd.NextDouble() + 40;
                X = 0.8 * canvas1.ActualWidth * Rnd.NextDouble() + 50;
                Y = 0.8 * canvas1.ActualHeight * Rnd.NextDouble() + canvas1.ActualWidth / 8 + H / 2;

                rocks[I] = new Obstacle(new Point(X, Y), W, H)
                {
                    Color = Brushes.Gray
                };
                rocks[I].Draw(canvas1);
            }
            W = 50 * Rnd.NextDouble() + 40;
            H = 50 * Rnd.NextDouble() + 40;
            X = 0.5 * canvas1.ActualWidth + 200 * Rnd.NextDouble() - 200;
            Y = canvas1.ActualWidth / 8 + H / 2;
            rocks[18] = new Obstacle(new Point(X, Y), W, H)
            {
                Color = Brushes.Gray
            };
            rocks[18].Draw(canvas1);
            W = 50 * Rnd.NextDouble() + 40;
            H = 50 * Rnd.NextDouble() + 40;
            X = 0.5 * canvas1.ActualWidth + 200 * Rnd.NextDouble() - 200;
            Y = canvas1.ActualWidth / 8 + H / 2;
            rocks[19] = new Obstacle(new Point(X, Y), W, H)
            {
                Color = Brushes.Gray
            };
            rocks[19].Draw(canvas1);
            //Create the start area
            target = new Rectangle()
            {
                Width = 40,
                Height = 20,
                Fill = Brushes.Green
            };
            target.SetValue(Canvas.LeftProperty, 0.85 * canvas1.ActualWidth);
            target.SetValue(Canvas.TopProperty, 0.0);
            canvas1.Children.Add(target);
            //Create the begin area
            begin = new Rectangle()
            {
                Width = 40,
                Height = 20,
                Fill = Brushes.Red
            };
            begin.SetValue(Canvas.LeftProperty, 0.1 * canvas1.ActualWidth);
            begin.SetValue(Canvas.TopProperty, 0.0);
            canvas1.Children.Add(begin);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!AppStarted)
            {
                if (Restart) Init();
                AppStarted = true;
                BtnStart.IsEnabled = false;
                drillPos = new Point(0.1 * canvas1.ActualWidth + 20, 0.0);
                drillAngle = Math.PI / 6;
                drillspeed = 2.0;
                drill = new Line[2];
                drill[0] = new Line()
                {
                    X1 = drillPos.X,
                    Y1 = drillPos.Y,
                    X2 = drillPos.X + 20 * Math.Cos(drillAngle),
                    Y2 = drillPos.Y + 20 * Math.Sin(drillAngle),
                    Stroke = Brushes.Red,
                    StrokeThickness = 3.0
                };
                drill[1] = new Line()
                {
                    X1 = drill[0].X2,
                    Y1 = drill[0].Y2,
                    X2 = drill[0].X2 + 20 * Math.Cos(drillAngle + drillBias * Math.PI / 6),
                    Y2 = drill[0].Y2 + 20 * Math.Sin(drillAngle + drillBias * Math.PI / 6),
                    Stroke = Brushes.Red,
                    StrokeThickness = 3.0
                };
                canvas1.Children.Add(drill[0]);
                canvas1.Children.Add(drill[1]);
                cable = new Polyline()
                {
                    Stroke = Brushes.White,
                    StrokeThickness = 2.0,
                };
                cable.Points.Add(drillPos);
                canvas1.Children.Add(cable);
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
        }

        private void UpdateDrill()
        {
            drill[0].X1 = drillPos.X;
            drill[0].Y1 = drillPos.Y;
            drill[0].X2 = drillPos.X + 20 * Math.Cos(drillAngle);
            drill[0].Y2 = drillPos.Y + 20 * Math.Sin(drillAngle);
            drill[1].X1 = drill[0].X2;
            drill[1].Y1 = drill[0].Y2;
            drill[1].X2 = drill[0].X2 + 10 * Math.Cos(drillAngle + drillBias * Math.PI / 6);
            drill[1].Y2 = drill[0].Y2 + 10 * Math.Sin(drillAngle + drillBias * Math.PI / 6);
            cable.Points.Add(drillPos);
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            drillPos += new Vector(drillspeed * Math.Cos(drillAngle), drillspeed * Math.Sin(drillAngle));
            drillAngle += drillBias * 0.05;
            Dispatcher.Invoke(Wait, System.Windows.Threading.DispatcherPriority.SystemIdle);
            UpdateDrill();
            CheckCollision();
            CheckWin();
        }

        private void CheckCollision()
        {
            for (int I = 0; I < rocks.Length; I++)
            {
                if (rocks[I].CheckCollision(new Point(drill[1].X2, drill[1].Y2)))
                {
                    GameEnd("Game Over. You failed to reach the target!");
                }
            }
        }

        private void GameEnd(string msg)
        {
            TextBox txt = new TextBox()
            {
                Width = canvas1.ActualWidth,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Text = msg,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Background = Brushes.Yellow
            };
            txt.SetValue(Canvas.LeftProperty, 0.0);
            txt.SetValue(Canvas.TopProperty, canvas1.ActualHeight / 2 - 15);
            canvas1.Children.Add(txt);
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            AppStarted = false;
            Restart = true;
            BtnStart.IsEnabled = true;
        }

        private void CheckWin()
        {
            double X = drill[1].X2;
            double Y = drill[1].Y2;
            if (X > 0.85 * canvas1.ActualWidth && X < 0.85 * canvas1.ActualWidth + 40 && Y <= 0) 
            {
                GameEnd("Congratulations! You reached the target.");
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Up:
                    drillBias = -1;
                    break;
                case Key.Down:
                    drillBias = 1;
                    break;
            }
        }

        private void Wait()
        {
            Thread.Sleep(100);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

    }
}