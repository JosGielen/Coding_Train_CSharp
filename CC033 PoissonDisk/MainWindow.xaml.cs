using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PoissonDisk
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private delegate void RenderDelegate();
        private int WaitTime = 5;
        private bool AppLoaded = false;
        private bool AppRunning = false;
        private int FieldWidth = 0;
        private int FieldHeight = 0;
        private Point[,] Field;
        private bool[,] Filled;
        private int rows = 0;
        private int cols = 0;
        private List<Point> Active;
        private double r;
        private int k;
        private double w;
        private Random rnd;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Init()
        {
            int col;
            int row;
            Ellipse el;

            FieldWidth = (int)Canvas1.ActualWidth;
            FieldHeight = (int)Canvas1.ActualHeight;
            r = 8.0;
            k = 30;
            w = r / Math.Sqrt(2);
            rows = (int)(FieldHeight / w);
            cols = (int)(FieldWidth / w);
            Field = new Point[rows, cols];
            Filled = new bool[rows, cols];
            for (int I = 0; I < rows; I++)
            {
                for (int J = 0; J < cols; J++)
                {
                    Filled[I, J] = false;
                }
            }
            Active = new List<Point>();
            rnd = new Random();
            //Pick the first point
            Canvas1.Children.Clear();
            Point p = new Point(FieldWidth / 2, FieldHeight / 2);
            col = (int)(p.X / w);
            row = (int)(p.Y / w);
            Active.Add(p);
            Field[row, col] = p;
            Filled[row, col] = true;
            el = new Ellipse()
            {
                Width = 4,
                Height = 4,
                Fill = Brushes.Red
            };
            el.SetValue(Canvas.LeftProperty, p.X - 2);
            el.SetValue(Canvas.TopProperty, p.Y - 2);
            Canvas1.Children.Add(el);
        }

        #region "Window Events"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            AppLoaded = true;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Start();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AppLoaded)
            {
                AppRunning = false;
                Init();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AppRunning = false;
            Environment.Exit(0);
        }

        #endregion

        private void Start()
        {
            int col = 0;
            int row = 0;
            int index = 0;
            double angle;
            double dist;
            Point sample;
            double X = 0.0;
            double Y = 0.0;
            Point neighbor;
            bool NewOK = false;
            bool ActiveOK = false;
            Ellipse el;

            AppRunning = true;
            while (AppRunning)
            {
                if (Active.Count > 0)
                {
                    //Take a random point from the Active List
                    index = rnd.Next(Active.Count);
                    ActiveOK = false;
                    for (int N = 0; N <= k; N++)
                    {
                        //Make a new point at random location but between r and 2r from Active(Index) point
                        angle = 2 * Math.PI * rnd.NextDouble();
                        dist = r + r * rnd.NextDouble();
                        X = Active[index].X + dist * Math.Cos(angle);
                        Y = Active[index].Y + dist * Math.Sin(angle);
                        col = (int)(X / w);
                        row = (int)(Y / w);
                        if (col < 1 | col > cols - 2 | row < 1 | row > rows - 2) continue; //The new point must be inside the Canvas
                        sample = new Point(X, Y);
                        NewOK = true;
                        //Check the cells around the cell where the new point is
                        for (int I = -1; I <= 1; I++)
                        {
                            for (int J = -1; J <= 1; J++)
                            {
                                if (Filled[row + I, col + J])
                                {
                                    neighbor = Field[row + I, col + J];
                                    if (Distance(sample, neighbor) <= r)
                                    {
                                        NewOK = false;
                                    }
                                }
                            }
                        }
                        if (NewOK)
                        {
                            Active.Add(sample);
                            Field[row, col] = sample;
                            Filled[row, col] = true;
                            ActiveOK = true;
                            el = new Ellipse()
                            {
                                Width = 4,
                                Height = 4,
                                Fill = Brushes.Red
                            };
                            el.SetValue(Canvas.LeftProperty, sample.X - 2);
                            el.SetValue(Canvas.TopProperty, sample.Y - 2);
                            Canvas1.Children.Add(el);
                            //Exit For
                        }
                    }
                    if (!ActiveOK)
                    {
                        el = new Ellipse()
                        {
                            Width = 4,
                            Height = 4,
                            Fill = Brushes.Black
                        };
                        el.SetValue(Canvas.LeftProperty, Active[index].X - 2);
                        el.SetValue(Canvas.TopProperty, Active[index].Y - 2);
                        Canvas1.Children.Add(el);
                        Active.RemoveAt(index);
                    }
                }
                else
                {
                    AppRunning = false;
                }
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), WaitTime);
            }
        }

        private double Distance(Point p1, Point p2)
 {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
}

    private void Wait(int t)
    {
            Thread.Sleep(WaitTime);
    }


}
    }