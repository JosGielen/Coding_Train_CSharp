using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using JG_Math;

namespace Blobby
{
    public partial class MainWindow : Window
    {
        private delegate void WaitDelegate(int t);
        private int WaitTime = 50;
        private bool Rendering = false;
        private double AngleStep = Math.PI / 50;
        private double MaxRadius = 200.0;
        private double MinRadius = 100.0;
        private int Percent = 0;
        private bool useOpenSimplex = true;
        private Random Rnd = new Random();
        private Polygon poly;
        private double Xoff, Yoff;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            poly = new Polygon()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1.0,
            };
            Point Pt;
            for (double Angle = 0; Angle < 2 * Math.PI; Angle += AngleStep)
            {
                Pt = new Point()
                {
                    X = canvas1.ActualWidth / 2 + MaxRadius * Math.Cos(Angle),
                    Y = canvas1.ActualHeight / 2 + MaxRadius * Math.Sin(Angle)
                };
                poly.Points.Add(Pt);
            }
            poly.Fill = Brushes.Gray;
            canvas1.Children.Add(poly);
            Rendering = true;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (!Rendering) return;
            double XOff;
            double YOff;
            double Offx;
            double Offy;
            double newX;
            double newY;
            double PrevX = 0.0;
            double PrevY = 0.0;
            double F = 3.0; //Determines how fast the Noise changes
            Offx = Math.Cos(2 * Percent * Math.PI / 100);
            Offy = Math.Sin(2 * Percent * Math.PI / 100);
            for (int I = 0; I < poly.Points.Count; I++)
            {
                double Angle = 2 * Math.PI * I / (poly.Points.Count - 1);
                XOff = F * Math.Cos(Angle);
                YOff = F * Math.Sin(Angle);
                double R = (MaxRadius - MinRadius) * PerlinNoise.WideNoise2D(XOff + Offx, YOff + Offy, 1) + MinRadius;
                newX = R * Math.Cos(Angle) + canvas1.ActualWidth / 2;
                newY = R * Math.Sin(Angle) + canvas1.ActualHeight / 2;
                poly.Points[I] = new Point()
                {
                    X = newX,
                    Y = newY
                };
                PrevX = newX;
                PrevY = newY;
            }
            Percent += 1;
            Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), WaitTime);
        }

        private void Wait(int t)
        {
            Thread.Sleep(t);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}