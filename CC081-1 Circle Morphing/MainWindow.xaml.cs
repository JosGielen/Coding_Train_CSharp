using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Circle_Morphing
{
    public partial class MainWindow : Window
    {
        private List<Point> CircPath = new List<Point>();
        private List<Point> TriPath = new List<Point>();
        private int res = 40;
        private Polygon poly;
        private double LerpAngle;
        private double radius = 250;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double angle;
            Point V1;
            Point V2;
            double oneThird = 2 * Math.PI / 3;
            Point Center = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            for (int i = 0; i < 3; i++)
            {
                V1 = new Point(Center.X + radius * Math.Cos(i * oneThird - Math.PI / 2), Center.Y +  radius * Math.Sin(i * oneThird - Math.PI / 2));
                V2 = new Point(Center.X + radius * Math.Cos((i + 1) % 3 * oneThird - Math.PI / 2), Center.Y + radius * Math.Sin((i + 1) % 3 * oneThird - Math.PI / 2));
                for (int j = 0; j < res; j++)
                {
                    angle = (i + j / (double)res) * oneThird - Math.PI / 2;
                    CircPath.Add(new Point(Center.X + radius * Math.Cos(angle), Center.Y + radius * Math.Sin(angle)));
                    TriPath.Add(VectorLerp(V1, V2, j / (double)res));
                }
            }
            LerpAngle = 0;
            poly = new Polygon()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3.0
            };
            canvas1.Children.Add(poly);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }



        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            poly.Points.Clear();
            for (int i = 0; i < CircPath.Count; i++)
            {
                poly.Points.Add(VectorLerp(CircPath[i], TriPath[i], (Math.Sin(LerpAngle) + 1) / 2));
            }
            LerpAngle += Math.PI / 200;
            if (LerpAngle >= 2 * Math.PI )
            {
                LerpAngle = 0;
            }
        }

        private Point VectorLerp(Point v1, Point v2, double factor)
        {
            double X = v1.X + factor * (v2.X - v1.X);
            double Y = v1.Y + factor * (v2.Y - v1.Y);
            return new Point(X, Y);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}