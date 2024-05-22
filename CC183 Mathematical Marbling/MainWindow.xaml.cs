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

namespace Mathematical_Marbling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Polygon> polygons = new List<Polygon>();
        private double r = 80;
        private int resolution = 200;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            Point p;
            double r;
            byte greyvalue;
            Color c;
            for (int i = 0; i < 50; i++)
            {
                p = new Point(canvas1.ActualWidth * Rnd.NextDouble(), canvas1.ActualHeight * Rnd.NextDouble());
                //p = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
                r = 40 * Rnd.NextDouble() + 30;
                //r = 40;
                greyvalue = (byte)(255 * Rnd.NextDouble());
                //c = Color.FromRgb(greyvalue, greyvalue, greyvalue);
                c = Color.FromRgb((byte)(255 * Rnd.NextDouble()), (byte)(255 * Rnd.NextDouble()), (byte)(255 * Rnd.NextDouble()));
                AddDrop(p, r, c);
            }
            TineLine(40, 8, new Point(canvas1.ActualWidth / 2, 0), new Vector(0, 1));
            TineLine(40, 8, new Point(0, canvas1.ActualHeight / 2), new Vector(1, 0));
            TineLine(40, 8, new Point(0, 0), new Vector(1, 1));
            TineLine(40, 8, new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2), new Vector(-1, 1));

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvas1.Children.Clear();
            Init();
        }

        private void AddDrop(Point center, double radius, Color c)
        {
            Vector v;
            Point p;
            double angle;
            Polygon poly = new Polygon()
            {
                Fill = new SolidColorBrush(c),
            };
            for (int i = 0; i < resolution; i++)
            {
                angle = 2 * Math.PI * i / resolution;
                poly.Points.Add(new Point(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle)));
            }
            for (int i = 0; i < polygons.Count; i++)
            {
                for (int j = 0; j < polygons[i].Points.Count; j++)
                {
                    p = polygons[i].Points[j];
                    v = p - center;
                    p = center + Math.Sqrt(1 + radius * radius / (v.Length * v.Length)) * v;
                    polygons[i].Points[j] = p;
                }
            }
            canvas1.Children.Add(poly);
            polygons.Add(poly);
        }

        private void TineLine(double z, double c, Point B, Vector M)
        {
            Vector PB;
            Point P;
            double d;
            double u = 1 / Math.Pow(2, 1 / c);
            Vector N = new Vector(-M.Y, M.X);
            M.Normalize();
            N.Normalize();
            for (int i = 0; i < polygons.Count; i++)
            {
                for (int j = 0; j < polygons[i].Points.Count; j++)
                {
                    P = polygons[i].Points[j];
                    PB = P - B;
                    d = Math.Abs(Vector.Multiply(PB, N));
                    polygons[i].Points[j] += z * Math.Pow(u, d) * M;
                }
            }
        }

        //private Vector Rotate(Vector v, double degrees)
        //{
        //    return new Vector(
        //        (float)(v.X * Math.Cos(degrees) - v.Y * Math.Sin(degrees)),
        //        (float)(v.X * Math.Sin(degrees) + v.Y * Math.Cos(degrees))
        //    );
        //}
    }
}