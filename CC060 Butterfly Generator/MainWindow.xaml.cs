using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Butterfly_Generator
{
    public partial class MainWindow : Window
    {
        private double maxSize = 200;
        private double Xoff = 0;
        private Vector Center;
        private List<Vector> vecs = new List<Vector>();
        private List<Brush> my_Colors = new List<Brush>();
        Polygon poly1, poly2;
        private double wingflap;
        private double flapStep;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Center = new Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            my_Colors.Add(Brushes.Red); 
            my_Colors.Add(Brushes.Lime);
            my_Colors.Add(Brushes.Orange);
            my_Colors.Add(Brushes.SteelBlue);
            my_Colors.Add(Brushes.Violet);
            my_Colors.Add(Brushes.Plum);
            my_Colors.Add(Brushes.Goldenrod);
            GetFrontEdge();
            GetOuterEdge();
            MakeButterfly();
            DrawBody();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void DrawBody()
        {
            Ellipse ellipse = new Ellipse()
            {
                Width = 10,
                Height = maxSize / 1.5,
                Fill = Brushes.Black
            };
            ellipse.SetValue(Canvas.LeftProperty, Center.X - 4);
            ellipse.SetValue(Canvas.TopProperty, Center.Y - maxSize / 4);
            canvas1.Children.Add(ellipse);
            ellipse = new Ellipse()
            {
                Width = 8,
                Height = 8,
                Fill = Brushes.Black
            };
            ellipse.SetValue(Canvas.LeftProperty, Center.X - 3);
            ellipse.SetValue(Canvas.TopProperty, Center.Y - maxSize / 4 - 3);
            canvas1.Children.Add(ellipse);
            Line l = new Line()
            {
                X1 = Center.X,
                Y1 = Center.Y - maxSize / 4,
                X2 = Center.X - 20,
                Y2 = Center.Y - maxSize / 4 - 30,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            canvas1.Children.Add(l);
            l = new Line()
            {
                X1 = Center.X,
                Y1 = Center.Y - maxSize / 4,
                X2 = Center.X + 20,
                Y2 = Center.Y - maxSize / 4 - 30,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            canvas1.Children.Add(l);

        }

        private void GetFrontEdge()
        {
            Vector v;
            double forwardExtend = 120 * Rnd.NextDouble() + 20;
            double frontStraightness = 20 * Rnd.NextDouble() + 40;
            for (int i = 0; i < 10; i++)
            {
                v = new Vector(maxSize * i / 10, -maxSize / 4.0 + 10.0 - forwardExtend * Math.Sin(10.0 * i / frontStraightness));
                vecs.Add(v);
            }
        }

        private void GetOuterEdge()
        {
            double radius;
            Vector v;
            Xoff = 1000 * Rnd.NextDouble();
            Vector last = vecs.Last();
            double EdgeSize = Math.Sqrt((last.X * last.X) + (last.Y * last.Y));
            double Startangle = Vector.AngleBetween(new Vector(0, -1), last) - 90.0;
            double angleStep = (90 - Startangle) / 40;
            double radians;
            double beta = -Math.PI / 2;
            double minradius = 0.2 * Rnd.NextDouble() + 0.2;
            double EdgeReduction = 1 - (0.02 * Rnd.NextDouble() + 0.01);
            for ( double angle = Startangle; angle <= 90; angle += angleStep)
            {
                radians = Math.PI * angle / 180;
                beta += Math.PI / 40;
                radius = 0.5 * EdgeSize * (Math.Abs(Math.Sin(beta)) + (0.7 * PerlinNoise.Noise(Xoff) + 0.3) + minradius);
                if (radius > EdgeSize ) { radius = EdgeSize; }
                v = new Vector(radius * Math.Cos(radians), radius * Math.Sin(radians));
                vecs.Add(v);
                Xoff += 0.2;
                EdgeSize *= EdgeReduction;
            }
            vecs.Add(new Vector(0, - maxSize / 4));
        }

        private void MakeButterfly()
        {
            Brush br = my_Colors[Rnd.Next(7)];
            wingflap = 0.0;
            flapStep = 0.02;
            poly1 = new Polygon();
            for (int i = 0; i < vecs.Count - 1; i++)
            {
                poly1.Points.Add(new Point(Center.X + wingflap * vecs[i].X, Center.Y + vecs[i].Y));
            }
            poly1.Fill = br;
            poly1.Stroke = Brushes.Black;
            poly1.StrokeThickness = 1.0;
            canvas1.Children.Add(poly1);
            poly2 = new Polygon();
            for (int i = 0; i < vecs.Count - 1; i++)
            {
                poly2.Points.Add(new Point(Center.X - wingflap * vecs[i].X, Center.Y + vecs[i].Y));
            }
            poly2.Fill = br;
            poly2.Stroke = Brushes.Black;
            poly2.StrokeThickness = 1.0;
            poly2.StrokeLineJoin = PenLineJoin.Round;
            canvas1.Children.Add(poly2);
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            poly1.Points.Clear();
            for (int i = 0; i < vecs.Count - 1; i++)
            {
                poly1.Points.Add(new Point(Center.X + wingflap * vecs[i].X, Center.Y + vecs[i].Y));
            }
            poly2.Points.Clear();
            for (int i = 0; i < vecs.Count - 1; i++)
            {
                poly2.Points.Add(new Point(Center.X - wingflap * vecs[i].X, Center.Y + vecs[i].Y));
            }
            wingflap += flapStep;
            if (wingflap >= 1.0 || wingflap <= 0.0)
            {
                flapStep = -1 * flapStep;
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            canvas1.Children.Clear();
            vecs.Clear();
            GetFrontEdge();
            GetOuterEdge();
            MakeButterfly();
            DrawBody();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}